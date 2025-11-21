using FinTrack.Data;
using FinTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Services
{
    public class AccountService(FinTrackContext context)
    {
        private readonly FinTrackContext _context = context;

        public async Task<decimal> GetSaldoAsync(int contaId, string usuarioId)
        {
            var conta = await _context.Contas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == contaId && c.UsuarioId == usuarioId);

            if (conta == null)
                return 0;

            var totalTransacoes = await _context.Transacoes
                .Where(t => t.ContaId == contaId && t.UsuarioId == usuarioId)
                .SumAsync(t => t.Tipo == Transacao.TipoTransacao.Entrada ? t.Valor : -t.Valor);

            return conta.SaldoInicial + totalTransacoes;
        }
    }
}
