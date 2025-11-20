// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using FinTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class ExternalLoginsModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IUserStore<Usuario> _userStore;

        public ExternalLoginsModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IUserStore<Usuario> userStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
        }

        /// <summary>
        /// Logins externos já conectados à conta.
        /// </summary>
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        /// <summary>
        /// Provedores disponíveis para adicionar.
        /// </summary>
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        /// <summary>
        /// Mostra o botão de remover login externo.
        /// </summary>
        public bool ShowRemoveButton { get; set; }

        /// <summary>
        /// Mensagem de status exibida no topo.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            // Logins atuais
            CurrentLogins = await _userManager.GetLoginsAsync(user);

            // Provedores ainda não vinculados
            OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            // Verificar se o usuário tem senha local
            string passwordHash = null;
            if (_userStore is IUserPasswordStore<Usuario> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            // Só permite remover login externo se houver uma senha OU mais de um login externo
            ShowRemoveButton = passwordHash != null || CurrentLogins.Count > 1;

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                StatusMessage = "Não foi possível remover o login externo.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Login externo removido com sucesso.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
        {
            // Garante limpeza do cookie externo anterior
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl,
                _userManager.GetUserId(User)
            );

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");

            var userId = await _userManager.GetUserIdAsync(user);
            var info = await _signInManager.GetExternalLoginInfoAsync(userId);

            if (info == null)
                throw new InvalidOperationException("Erro inesperado ao carregar informações de login externo.");

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                StatusMessage = "Não foi possível adicionar o login externo. Ele pode já estar associado a outra conta.";
                return RedirectToPage();
            }

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "Login externo adicionado com sucesso.";
            return RedirectToPage();
        }
    }
}
