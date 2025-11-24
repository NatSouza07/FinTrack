using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly FinTrackContext _context;
        private readonly UserManager<Usuario> _userManager;

        public RelatoriosController(FinTrackContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // PÁGINA PRINCIPAL DE RELATÓRIOS (GRÁFICOS)

        public IActionResult Index()
        {
            return View();
        }

        // MÉTODO JSON → MonthlyTotalsJson

        [HttpGet]
        public async Task<IActionResult> MonthlyTotalsJson(int year)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
                return Unauthorized();

            var dados = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId && t.Data.Year == year)
                .GroupBy(t => t.Data.Month)
                .Select(g => new
                {
                    month = g.Key,
                    entrada = g.Where(t => t.Tipo == Transacao.TipoTransacao.Entrada)
                               .Sum(t => (decimal?)t.Valor) ?? 0,
                    saida = g.Where(t => t.Tipo == Transacao.TipoTransacao.Saida)
                             .Sum(t => (decimal?)t.Valor) ?? 0
                })
                .OrderBy(x => x.month)
                .ToListAsync();

            return Json(dados);
        }


        // MÉTODO JSON → CategoryTotalsJson
        [HttpGet]
        public async Task<IActionResult> CategoryTotalsJson(int year, int month)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
                return Unauthorized();

            var dados = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId &&
                            t.Data.Year == year &&
                            t.Data.Month == month)
                .GroupBy(t => t.Categoria.Nome)
                .Select(g => new
                {
                    categoria = g.Key,
                    total = g.Sum(t => (decimal?)t.Valor) ?? 0
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            return Json(dados);
        }

        // AGRUPAMENTO POR CATEGORIA

        public IActionResult Agrupamento()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CategoriasJson(DateTime inicio, DateTime fim)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
                return Unauthorized();

            if (inicio > fim)
                return BadRequest("A data inicial não pode ser maior que a data final.");

            var dados = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId &&
                            t.Data >= inicio &&
                            t.Data <= fim)
                .GroupBy(t => t.Categoria.Nome)
                .Select(g => new
                {
                    categoria = g.Key,
                    total = g.Sum(t => (decimal?)t.Valor) ?? 0
                })
                .OrderByDescending(x => x.total)
                .ToListAsync();

            return Json(dados);
        }

        // 3) PIVOT (CATEGORIA × MÊS)

        public IActionResult PivotView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Pivot(int ano)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
                return Unauthorized();

            var categorias = await _context.Categorias
                .OrderBy(c => c.Nome)
                .Select(c => c.Nome)
                .ToListAsync();

            var meses = Enumerable.Range(1, 12);

            var matriz = new List<Dictionary<string, object>>();

            foreach (var categoria in categorias)
            {
                var linha = new Dictionary<string, object>
                {
                    { "categoria", categoria }
                };

                foreach (var mes in meses)
                {
                    var total = await _context.Transacoes
                        .Where(t => t.UsuarioId == usuarioId &&
                                    t.Categoria.Nome == categoria &&
                                    t.Data.Year == ano &&
                                    t.Data.Month == mes)
                        .SumAsync(t => (decimal?)t.Valor) ?? 0;

                    linha.Add($"m{mes}", total);
                }

                matriz.Add(linha);
            }

            return Json(matriz);
        }
    }
}
