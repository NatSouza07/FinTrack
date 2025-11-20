using FinTrack.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        ILogger<ChangePasswordModel> logger
    ) : PageModel

    {
        private readonly UserManager<Usuario> _userManager = userManager;
        private readonly SignInManager<Usuario> _signInManager = signInManager;
        private readonly ILogger<ChangePasswordModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Senha atual")]
            public string OldPassword { get; set; } = string.Empty;

            [Required]
            [StringLength(100, MinimumLength = 6,
                ErrorMessage = "A senha deve ter no mínimo {2} e no máximo {1} caracteres.")]
            [DataType(DataType.Password)]
            [Display(Name = "Nova senha")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar nova senha")]
            [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação não conferem.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            if (!await _userManager.HasPasswordAsync(user))
                return RedirectToPage("./SetPassword");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound($"Não foi possível carregar o usuário '{_userManager.GetUserId(User)}'.");

            var result = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Usuário alterou a senha com sucesso.");

            StatusMessage = "Sua senha foi alterada com sucesso.";

            return RedirectToPage();
        }
    }
}
