using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinTrack.Models;

namespace FinTrack.Data;

public class FinTrackContext(DbContextOptions<FinTrackContext> options)
    : IdentityDbContext<Usuario>(options)
{
    public DbSet<Conta> Contas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<TipoPagamento> TiposPagamento { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Meta> Metas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Transacao>()
            .HasOne(t => t.Conta)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Categoria>()
            .HasIndex(c => new { c.Nome, c.Tipo })
            .IsUnique();

        builder.Entity<TipoPagamento>()
            .HasIndex(tp => tp.Nome)
            .IsUnique();

        builder.Entity<Transacao>()
            .HasOne(t => t.TipoPagamento)
            .WithMany()
            .HasForeignKey(t => t.TipoPagamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
