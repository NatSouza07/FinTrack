#nullable disable

using System.Text;
using System.Threading.Tasks;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<Usuario> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }
        public bool DisplayConfirmAccountLink { get; set; }
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
                return RedirectToPage("/Index");

            returnUrl ??= Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound($"Unable to load user with email '{email}'.");

            Email = email;
            DisplayConfirmAccountLink = true;

            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    null,
                    new { area = "Identity", userId, code, returnUrl },
                    Request.Scheme);
            }

            return Page();
        }
    }
}
