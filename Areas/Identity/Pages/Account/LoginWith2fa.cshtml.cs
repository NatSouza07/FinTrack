#nullable disable
using FinTrack.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        ILogger<LoginWith2faModel> logger
    ) : PageModel

    {
        private readonly SignInManager<Usuario> _signInManager = signInManager;
        private readonly UserManager<Usuario> _userManager = userManager;
        private readonly ILogger<LoginWith2faModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(7, MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Código do autenticador")]
            public string TwoFactorCode { get; set; }

            [Display(Name = "Lembrar este dispositivo")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("Unable to load two-factor authentication user.");

            ReturnUrl = returnUrl ?? "/";
            RememberMe = rememberMe;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Page();

            returnUrl ??= Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("Unable to load two-factor authentication user.");

            var code = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(code, rememberMe, Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2FA.", user.Id);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }

            _logger.LogWarning("Invalid authenticator code for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, "Código inválido.");
            return Page();
        }
    }
}
