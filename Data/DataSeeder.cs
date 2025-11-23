using FinTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<FinTrackContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            string adminEmail = "admin@fintrack.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Usuario
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NomeCompleto = "Administrador FinTrack"
                };

                await userManager.CreateAsync(adminUser, adminPassword);
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (!context.Categorias.Any())
            {
                List<Categoria> categorias =
                [
                    new() { Nome = "Salário", Tipo = TipoCategoria.Receita },
                    new() { Nome = "Investimentos", Tipo = TipoCategoria.Receita },
                    new() { Nome = "Alimentação", Tipo = TipoCategoria.Despesa },
                    new() { Nome = "Transporte", Tipo = TipoCategoria.Despesa },
                    new() { Nome = "Lazer", Tipo = TipoCategoria.Despesa }
                ];

                context.Categorias.AddRange(categorias);
                await context.SaveChangesAsync();
            }

            if (!context.TiposPagamento.Any())
            {
                List<TipoPagamento> tipos =
                [
                    new() { Nome = "Dinheiro" },
                    new() { Nome = "Cartão de Crédito" },
                    new() { Nome = "Cartão de Débito" },
                    new() { Nome = "PIX" }
                ];

                context.TiposPagamento.AddRange(tipos);
                await context.SaveChangesAsync();
            }

            if (!context.Contas.Any(c => c.UsuarioId == adminUser!.Id))
            {
                var conta = new Conta
                {
                    Nome = "Carteira",
                    SaldoInicial = 0,
                    UsuarioId = adminUser.Id
                };

                context.Contas.Add(conta);
                await context.SaveChangesAsync();
            }
        }
    }
}
