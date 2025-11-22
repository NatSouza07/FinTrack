using FinTrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly FinTrackContext _context;

        public RelatoriosController(FinTrackContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MensalJson(int ano)
        {
            var valores = new decimal[12];

            var dados = await _context.Transacoes
                .Where(t => t.Data.Year == ano)
                .GroupBy(t => t.Data.Month)
                .Select(g => new { Mes = g.Key, Total = g.Sum(t => t.Valor) })
                .ToListAsync();

            foreach (var item in dados)
                valores[item.Mes - 1] = item.Total;

            return Json(valores);
        }

        [HttpGet]
        public async Task<IActionResult> CategoriasJson(DateTime inicio, DateTime fim)
        {
            var dados = await _context.Transacoes
                .Where(t => t.Data >= inicio && t.Data <= fim)
                .GroupBy(t => t.Categoria.Nome)
                .Select(g => new
                {
                    label = g.Key,
                    value = g.Sum(t => t.Valor)
                })
                .ToListAsync();

            return Json(dados);
        }

        [HttpGet]
        public async Task<IActionResult> Pivot(int ano)
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Nome)
                .Select(c => c.Nome)
                .ToListAsync();

            var meses = Enumerable.Range(1, 12);

            var matriz = new List<object>();
            foreach (var categoria in categorias)
            {
                var linha = new Dictionary<string, object>
                {
                    { "categoria", categoria }
                };

                foreach (var mes in meses)
                {
                    var total = await _context.Transacoes
                        .Where(t => t.Categoria.Nome == categoria &&
                                    t.Data.Year == ano &&
                                    t.Data.Month == mes)
                        .SumAsync(t => (decimal?)t.Valor) ?? 0;

                    linha.Add($"m{mes}", total);
                }

                matriz.Add(linha);
            }

            return Json(matriz);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PivotView()
        {
            return View();
        }
    }
}
