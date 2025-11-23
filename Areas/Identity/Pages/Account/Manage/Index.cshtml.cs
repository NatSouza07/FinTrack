using FinTrack.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinTrack.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public IndexModel(UserManager<Usuario> userManager,
                          SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [StringLength(50)]
            public string Nickname { get; set; }

            [Phone]
            public string PhoneNumber { get; set; }

            public IFormFile FotoPerfil { get; set; }
        }

        private async Task LoadAsync(Usuario user)
        {
            Username = user.Email;

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                Nickname = user.Nickname
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (Input.Nickname != user.Nickname)
                user.Nickname = Input.Nickname;

            if (Input.PhoneNumber != user.PhoneNumber)
                user.PhoneNumber = Input.PhoneNumber;

            if (Input.FotoPerfil != null)
            {
                var fileName = $"{user.Id}.png";
                var folder = Path.Combine("wwwroot", "img", "perfis");
                Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await Input.FotoPerfil.CopyToAsync(stream);

                user.FotoPerfil = $"/img/perfis/{fileName}";
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            StatusMessage = "Informações atualizadas com sucesso!";
            return RedirectToPage();
        }
    }
}
