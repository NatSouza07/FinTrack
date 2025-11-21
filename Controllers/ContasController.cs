using FinTrack.Data;
using FinTrack.Models;
using FinTrack.Services;
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
        private readonly AccountService _accountService;

        public ContasController(
            FinTrackContext context,
            UserManager<Usuario> userManager,
            AccountService accountService)
        {
            _context = context;
            _userManager = userManager;
            _accountService = accountService;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var usuarioId = _userManager.GetUserId(User);

            var contas = await _context.Contas
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            // Aplicar cálculo de saldo em cada conta
            foreach (var conta in contas)
            {
                conta.SaldoInicial = await _accountService.GetSaldoAsync(conta.Id, usuarioId!);
            }

            return View(contas);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            // Calcular saldo atualizado
            conta.SaldoInicial = await _accountService.GetSaldoAsync(conta.Id, usuarioId!);

            return View(conta);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Conta conta)
        {
            if (!ModelState.IsValid)
                return View(conta);

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            conta.UsuarioId = usuarioId;

            _context.Add(conta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Conta conta)
        {
            if (id != conta.Id)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);

            var contaDb = await _context.Contas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (contaDb == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(conta);

            conta.UsuarioId = usuarioId!;

            _context.Update(conta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return NotFound();

            return View(conta);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var conta = await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conta == null)
                return Unauthorized();

            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
