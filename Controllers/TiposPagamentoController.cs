using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    [Authorize]
    public class TiposPagamentoController : Controller
    {
        private readonly FinTrackContext _context;

        public TiposPagamentoController(FinTrackContext context)
        {
            _context = context;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var tipos = await _context.TiposPagamento.ToListAsync();
            return View(tipos);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TiposPagamento.FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // CREATE (GET)

        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoPagamento tipo)
        {
            if (!ModelState.IsValid)
                return View(tipo);

            _context.TiposPagamento.Add(tipo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET()

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TiposPagamento.FindAsync(id);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // EDIT (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoPagamento tipo)
        {
            if (id != tipo.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(tipo);

            _context.Update(tipo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET)

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipo = await _context.TiposPagamento.FirstOrDefaultAsync(t => t.Id == id);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        // DELETE (POST)

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipo = await _context.TiposPagamento.FindAsync(id);
            if (tipo == null) return NotFound();

            _context.TiposPagamento.Remove(tipo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
