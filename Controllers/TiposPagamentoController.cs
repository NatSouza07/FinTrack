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

        public async Task<IActionResult> Index()
        {
            var tipos = await _context.TiposPagamento.ToListAsync();
            return View(tipos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Tipo de pagamento inválido.";
                return RedirectToAction(nameof(Index));
            }

            var tipo = await _context.TiposPagamento.FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null)
            {
                TempData["Error"] = "Tipo de pagamento não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoPagamento tipo)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(tipo);
            }

            try
            {
                _context.TiposPagamento.Add(tipo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de pagamento criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Erro ao criar o tipo de pagamento.";
                return View(tipo);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            var tipo = await _context.TiposPagamento.FindAsync(id);

            if (tipo == null)
            {
                TempData["Error"] = "Tipo de pagamento não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoPagamento tipo)
        {
            if (id != tipo.Id)
            {
                TempData["Error"] = "ID inconsistente.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(tipo);
            }

            try
            {
                _context.Update(tipo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de pagamento atualizado!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Erro ao atualizar o tipo de pagamento.";
                return View(tipo);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID inválido.";
                return RedirectToAction(nameof(Index));
            }

            var tipo = await _context.TiposPagamento.FirstOrDefaultAsync(t => t.Id == id);

            if (tipo == null)
            {
                TempData["Error"] = "Tipo de pagamento não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipo = await _context.TiposPagamento.FindAsync(id);

            if (tipo == null)
            {
                TempData["Error"] = "O tipo de pagamento não existe.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.TiposPagamento.Remove(tipo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tipo de pagamento removido com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Erro ao remover o tipo de pagamento.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
