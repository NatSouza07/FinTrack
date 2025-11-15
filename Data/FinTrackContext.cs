using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinTrack.Models;

namespace FinTrack.Data
{
    public class FinTrackContext : IdentityDbContext<Usuario>
    {
        public FinTrackContext(DbContextOptions<FinTrackContext> options)
            : base(options)
        {
        }

        public DbSet<Conta> Contas { get; set; }
        /*public DbSet<Categoria> Categorias { get; set; }
        public DbSet<TipoPagamento> TiposPagamento { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Meta> Metas { get; set; }*/

    }
}
