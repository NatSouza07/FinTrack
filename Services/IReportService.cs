using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinTrack.Services
{
    public interface IReportService
    {
        // Retorna total por mês (1..12) para um ano: [{ month: 1, entrada: 123.45, saida: 67.89 }, ...]
        Task<IEnumerable<MonthlyTotalsDto>> GetMonthlyTotalsAsync(string userId, int year);

        // Retorna total por categoria para um mês/ano: [{ categoria: "Alimentação", total: 123.45 }, ...]
        Task<IEnumerable<CategoryTotalsDto>> GetCategoryTotalsAsync(string userId, int year, int month);
    }

    public record MonthlyTotalsDto(int Month, decimal Entrada, decimal Saida);
    public record CategoryTotalsDto(string Categoria, decimal Total);
}
