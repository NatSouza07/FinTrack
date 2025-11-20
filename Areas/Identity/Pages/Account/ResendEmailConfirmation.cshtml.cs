#nullable disable
using FinTrack.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(
            UserManager<Usuario> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "E-mail de verificação enviado. Verifique sua caixa de entrada.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                null,
                new { userId, code },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirme seu e-mail",
                $"Confirme sua conta clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            ModelState.AddModelError(string.Empty, "E-mail de verificação enviado. Verifique sua caixa de entrada.");
            return Page();
        }
    }
}
