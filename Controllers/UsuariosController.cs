using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var usuarios = _userManager.Users.ToList();
            return View(usuarios);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                TempData["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "O usuário informado não existe.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            ViewBag.UserRole = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string nomeCompleto, string role)
        {
            if (id == null)
            {
                TempData["Error"] = "Usuário inválido.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                usuario.NomeCompleto = nomeCompleto;
                await _userManager.UpdateAsync(usuario);

                var rolesAtuais = await _userManager.GetRolesAsync(usuario);
                await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);

                if (!string.IsNullOrEmpty(role))
                    await _userManager.AddToRoleAsync(usuario, role);

                TempData["Success"] = "Usuário atualizado com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Ocorreu um erro ao atualizar o usuário.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                TempData["Error"] = "Usuário inválido.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                TempData["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var email = usuario.Email?.ToLower();
            if (email == "admin@fintrack.com")
            {
                TempData["Warning"] = "O administrador principal não pode ser excluído.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _userManager.DeleteAsync(usuario);
                TempData["Success"] = "Usuário removido com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Não foi possível excluir o usuário.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
