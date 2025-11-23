using System.Globalization;
using FinTrack.Data;
using FinTrack.Models;
using FinTrack.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC normal (sem Authorize global)
builder.Services.AddControllersWithViews();

// Razor Pages — necessário para Identity UI
builder.Services.AddRazorPages();

// DB
builder.Services.AddDbContext<FinTrackContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinTrackConnection")));

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Identity + UI padrão
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<FinTrackContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Cookie aponta para Login do Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});


// Cultura PT-BR
var defaultCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture);
    options.SupportedCultures = new[] { defaultCulture };
    options.SupportedUICultures = new[] { defaultCulture };
});

var app = builder.Build();

app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await DataSeeder.SeedAsync(app.Services);

// Necessário para Identity UI
app.MapRazorPages();

app.Run();
