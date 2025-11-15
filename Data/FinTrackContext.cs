using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Data
{
    public class FinTrackContext : IdentityDbContext
    {
        public FinTrackContext(DbContextOptions<FinTrackContext> options)
            : base(options)
        {
        }

        // DbSets serão adicionados depois (Contas, Categorias, Transacoes, etc.)
    }
}
