using FinTrack.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<Usuario> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
                throw new InvalidOperationException("Não é possível desativar o 2FA pois ele não está habilitado.");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            var disableResult = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (!disableResult.Succeeded)
                throw new InvalidOperationException("Erro inesperado ao desativar o 2FA.");

            _logger.LogInformation(
                "Usuário com ID '{UserId}' desativou a autenticação 2FA.",
                _userManager.GetUserId(User)
            );

            StatusMessage = "A autenticação em duas etapas (2FA) foi desativada. Você pode reativá-la configurando um app autenticador.";

            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}
