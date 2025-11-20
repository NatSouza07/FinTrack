// Licensed under the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using FinTrack.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;

        public PersonalDataModel(
            UserManager<Usuario> userManager,
            ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // Adicionei a propriedade TempData que sua view/partial espera
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        // (Opcional) handler de download - exemplo simples
        public async Task<IActionResult> OnPostDownloadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            // Aqui você implementaria a geração/retorno do arquivo com os dados do usuário.
            // Exemplo: gerar CSV/JSON e retornar File(...).
            StatusMessage = "Arquivo com seus dados foi gerado (exemplo).";
            return RedirectToPage();
        }
    }
}
