#nullable disable

using System.Text;
using System.Threading.Tasks;
using FinTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace FinTrack.Areas.Identity.Pages.Account
{
    public class ConfirmEmailChangeModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public ConfirmEmailChangeModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
        {
            if (userId == null || email == null || code == null)
                return RedirectToPage("/Index");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Unable to load user with ID '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);

            if (!result.Succeeded)
            {
                StatusMessage = "Erro ao alterar o e-mail.";
                return Page();
            }

            var setUserName = await _userManager.SetUserNameAsync(user, email);
            if (!setUserName.Succeeded)
            {
                StatusMessage = "Erro ao atualizar o nome de usuário.";
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Seu e-mail foi alterado com sucesso.";

            return Page();
        }
    }
}
