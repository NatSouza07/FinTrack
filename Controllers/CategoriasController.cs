using FinTrack.Data;
using FinTrack.Models;
using FinTrack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinTrack.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly FinTrackContext _context;

        public CategoriasController(FinTrackContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? tipoId)
        {
            ViewData["Tipos"] = Enum.GetValues(typeof(TipoCategoria))
                .Cast<TipoCategoria>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                }).ToList();

            var categorias = _context.Categorias.AsQueryable();

            if (tipoId.HasValue && Enum.IsDefined(typeof(TipoCategoria), tipoId.Value))
            {
                categorias = categorias.Where(c => c.Tipo == (TipoCategoria)tipoId.Value);
                ViewData["SelectedTipoId"] = tipoId.Value;
            }

            return View(await categorias.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Categoria inválida.";
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoria não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            var existe = await _context.Categorias
                .AnyAsync(c => c.Nome == categoria.Nome && c.Tipo == categoria.Tipo);

            if (existe)
                ModelState.AddModelError("Nome", "Já existe uma categoria com este Nome e Tipo.");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(categoria);
            }

            try
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria criada com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Erro ao criar a categoria.";
                return View(categoria);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Categoria inválida.";
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoria não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                TempData["Error"] = "ID inconsistente.";
                return RedirectToAction(nameof(Index));
            }

            var existe = await _context.Categorias
                .AnyAsync(c => c.Id != categoria.Id &&
                               c.Nome == categoria.Nome &&
                               c.Tipo == categoria.Tipo);

            if (existe)
                ModelState.AddModelError("Nome", "Já existe uma categoria com este Nome e Tipo.");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(categoria);
            }

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria atualizada!";
            }
            catch
            {
                TempData["Error"] = "Erro ao atualizar a categoria.";
                return View(categoria);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Categoria inválida.";
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoria não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoria não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoria removida com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Erro ao remover a categoria.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
