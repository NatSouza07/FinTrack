using FinTrack.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class EmailModel(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        IEmailSender emailSender
    ) : PageModel
    {
        private readonly UserManager<Usuario> _userManager = userManager;
        private readonly SignInManager<Usuario> _signInManager = signInManager;
        private readonly IEmailSender _emailSender = emailSender;

        public string Email { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Novo e-mail")]
            public string NewEmail { get; set; } = string.Empty;
        }

        private async Task LoadAsync(Usuario user)
        {
            var email = await _userManager.GetEmailAsync(user) ?? string.Empty;

            Email = email;
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            Input = new InputModel
            {
                NewEmail = email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var currentEmail = await _userManager.GetEmailAsync(user) ?? string.Empty;

            if (Input.NewEmail != currentEmail)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    pageName: "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId, email = Input.NewEmail, code = encodedCode },
                    protocol: Request.Scheme
                );

                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirmar alteração de e-mail",
                    $"Confirme a alteração do seu e-mail clicando no link: <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clique aqui</a>."
                );

                StatusMessage = "Um link de confirmação foi enviado para o novo e-mail.";
                return RedirectToPage();
            }

            StatusMessage = "O e-mail informado é igual ao atual.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user) ?? string.Empty;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                pageName: "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code = encodedCode },
                protocol: Request.Scheme
            );

            await _emailSender.SendEmailAsync(
                email,
                "Confirmar e-mail",
                $"Finalize a verificação do seu e-mail clicando aqui: <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>confirmar e-mail</a>."
            );

            StatusMessage = "E-mail de verificação enviado.";
            return RedirectToPage();
        }
    }
}
