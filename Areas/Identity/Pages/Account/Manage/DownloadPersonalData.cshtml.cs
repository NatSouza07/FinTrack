using FinTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<Usuario> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            _logger.LogInformation(
                "Usuário com ID '{UserId}' solicitou o download dos seus dados pessoais.",
                _userManager.GetUserId(User)
            );

            var personalData = new Dictionary<string, string>();

            var personalDataProps = typeof(Usuario)
                .GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var prop in personalDataProps)
            {
                personalData.Add(prop.Name, prop.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
                personalData.Add(
                    $"{login.LoginProvider} - Chave do Provedor Externo",
                    login.ProviderKey
                );
            }

            var authKey = await _userManager.GetAuthenticatorKeyAsync(user);
            personalData.Add("Authenticator Key", authKey ?? "null");

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=DadosPessoais.json");

            return new FileContentResult(
                JsonSerializer.SerializeToUtf8Bytes(personalData),
                "application/json"
            );
        }
    }
}
