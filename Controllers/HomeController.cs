using System;
using System.Linq;
using System.Threading.Tasks;
using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class HomeController : Controller
{
    private readonly FinTrackContext _context;
    private readonly UserManager<Usuario> _userManager;

    public HomeController(FinTrackContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public class DashboardViewModel
    {
        public decimal TotalSaldo { get; set; }
        public decimal ReceitasMes { get; set; }
        public decimal DespesasMes { get; set; }
        public int MetasAtingidas { get; set; }
        public decimal[] MensalValues { get; set; }
        public string[] MensalLabels { get; set; }
        public string[] CategoriaLabels { get; set; }
        public decimal[] CategoriaValues { get; set; }
        public object[] UltimasTransacoes { get; set; }
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null)
            return Unauthorized();

        var now = DateTime.Now;
        var anoAtual = now.Year;
        var mesAtual = now.Month;

        var contas = await _context.Contas
            .Where(c => c.UsuarioId == userId)
            .ToListAsync();

        decimal saldoInicialTotal = contas.Sum(c => c.SaldoInicial);

        var transacoesUsuario = _context.Transacoes
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == userId);

        var transacoesLista = await transacoesUsuario
            .OrderByDescending(t => t.Data)
            .Take(10)
            .Select(t => new
            {
                t.Id,
                t.Data,
                t.Valor,
                Categoria = t.Categoria != null ? t.Categoria.Nome : "(Sem categoria)",
                Tipo = t.Tipo.ToString()
            })
            .ToListAsync();

        var receitasMes = await transacoesUsuario
            .Where(t => t.Data.Year == anoAtual &&
                        t.Data.Month == mesAtual &&
                        t.Tipo == Transacao.TipoTransacao.Entrada)
            .SumAsync(t => (decimal?)t.Valor) ?? 0m;

        var despesasMes = await transacoesUsuario
            .Where(t => t.Data.Year == anoAtual &&
                        t.Data.Month == mesAtual &&
                        t.Tipo == Transacao.TipoTransacao.Saida)
            .SumAsync(t => (decimal?)t.Valor) ?? 0m;

        decimal[] mensalValues = new decimal[12];
        string[] mensalLabels = Enumerable.Range(1, 12)
            .Select(m => System.Globalization.CultureInfo.CurrentCulture
                .DateTimeFormat.GetAbbreviatedMonthName(m))
            .ToArray();

        for (int m = 1; m <= 12; m++)
        {
            mensalValues[m - 1] = await transacoesUsuario
                .Where(t => t.Data.Year == anoAtual && t.Data.Month == m)
                .SumAsync(t => (decimal?)t.Valor) ?? 0m;
        }

        var categorias = await transacoesUsuario
            .Where(t => t.Data.Year == anoAtual)
            .GroupBy(t => t.Categoria != null ? t.Categoria.Nome : "(Sem categoria)")
            .Select(g => new { Name = g.Key, Sum = g.Sum(x => x.Valor) })
            .OrderByDescending(g => Math.Abs(g.Sum))
            .Take(8)
            .ToListAsync();

        var categoriaLabels = categorias.Select(c => c.Name).ToArray();
        var categoriaValues = categorias.Select(c => Math.Abs(c.Sum)).ToArray();

        var metasAtingidas = 0;

        var metas = await _context.Metas
            .Where(m => m.UsuarioId == userId && m.Ano == anoAtual)
            .ToListAsync();

        foreach (var meta in metas)
        {
            var totalMes = await transacoesUsuario
                .Where(t => t.Tipo == Transacao.TipoTransacao.Entrada &&
                            t.Data.Month == meta.Mes &&
                            t.Data.Year == meta.Ano)
                .SumAsync(t => (decimal?)t.Valor) ?? 0m;

            if (totalMes >= meta.ValorMeta)
                metasAtingidas++;
        }

        var totalMovimentos = await transacoesUsuario
            .SumAsync(t => (decimal?)t.Valor) ?? 0m;

        var model = new DashboardViewModel
        {
            TotalSaldo = saldoInicialTotal + totalMovimentos,
            ReceitasMes = receitasMes,
            DespesasMes = Math.Abs(despesasMes),
            MetasAtingidas = metasAtingidas,
            MensalValues = mensalValues,
            MensalLabels = mensalLabels,
            CategoriaLabels = categoriaLabels,
            CategoriaValues = categoriaValues,
            UltimasTransacoes = transacoesLista.ToArray()
        };

        return View(model);
    }
}
