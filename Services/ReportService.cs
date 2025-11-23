using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinTrack.Data;
using FinTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Services
{
    public class ReportService : IReportService
    {
        private readonly FinTrackContext _context;

        public ReportService(FinTrackContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyTotalsDto>> GetMonthlyTotalsAsync(string userId, int year)
        {
            var items = await _context.Transacoes
                .Where(t => t.UsuarioId == userId && t.Data.Year == year)
                .GroupBy(t => t.Data.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Entrada = g.Where(t => t.Tipo == Transacao.TipoTransacao.Entrada)
                               .Sum(t => (decimal?)t.Valor) ?? 0,

                    Saida = g.Where(t => t.Tipo == Transacao.TipoTransacao.Saida)
                             .Sum(t => (decimal?)t.Valor) ?? 0
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 12)
                .Select(m =>
                {
                    var found = items.FirstOrDefault(x => x.Month == m);
                    return new MonthlyTotalsDto(
                        m,
                        Entrada: found?.Entrada ?? 0,
                        Saida: found?.Saida ?? 0
                    );
                });

            return result;
        }

        public async Task<IEnumerable<CategoryTotalsDto>> GetCategoryTotalsAsync(string userId, int year, int month)
        {
            var items = await _context.Transacoes
                .Where(t => t.UsuarioId == userId && t.Data.Year == year && t.Data.Month == month)
                .GroupBy(t => t.Categoria.Nome)
                .Select(g => new CategoryTotalsDto(
                    g.Key,
                    g.Sum(t => (decimal?)t.Valor) ?? 0
                ))
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            return items;
        }
    }
}
