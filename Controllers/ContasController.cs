using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    [Authorize]
    public class ContasController : Controller
    {
        private readonly FinTrackContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ContasController(
            FinTrackContext context,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string _GetUserId()
        {
            return _userManager.GetUserId(User);
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            var contas = await _context.Contas
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            return View(contas);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conta conta)
        {
            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            if (ModelState.ContainsKey(nameof(Conta.SaldoInicial)))
            {
                ModelState.Remove(nameof(Conta.SaldoInicial));
            }

            if (ModelState.ContainsKey(nameof(Conta.UsuarioId)))
            {
                ModelState.Remove(nameof(Conta.UsuarioId));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Existem erros de validação. Verifique os campos.";
                return View(conta);
            }

            conta.UsuarioId = usuarioId;

            _context.Add(conta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro fatal ao salvar: {ex.Message}";
                return View(conta);
            }

            TempData["Success"] = "Conta criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Conta conta)
        {
            if (id != conta.Id)
                return NotFound();

            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            if (ModelState.ContainsKey(nameof(Conta.UsuarioId)))
            {
                ModelState.Remove(nameof(Conta.UsuarioId));
            }

            if (!ModelState.IsValid)
                return View(conta);

            conta.UsuarioId = usuarioId;

            try
            {
                _context.Update(conta);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contas.Any(e => e.Id == id && e.UsuarioId == usuarioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao atualizar a conta: {ex.Message}";
                return View(conta);
            }

            TempData["Success"] = "Conta atualizada!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _GetUserId();
            if (usuarioId is null)
                return Unauthorized();

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return Unauthorized();

            _context.Contas.Remove(conta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao excluir a conta: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Conta excluída!";
            return RedirectToAction(nameof(Index));
        }
    }
}