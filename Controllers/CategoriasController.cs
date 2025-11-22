using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinTrack.Data;
using FinTrack.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace FinTrack.Controllers
{
    // Adicione [Authorize] se for necessário que o usuário esteja logado
    // [Authorize] 
    public class CategoriasController : Controller
    {
        private readonly FinTrackContext _context;

        public CategoriasController(FinTrackContext context)
        {
            // Injeção de dependência do contexto do banco
            _context = context;
        }

        // GET: Categorias (Index com Filtro por Tipo)
        public async Task<IActionResult> Index(int? tipoId)
        {
            // Prepara a lista de Tipos (Receita/Despesa) para o Dropdown de filtro na View
            ViewData["Tipos"] = Enum.GetValues(typeof(TipoCategoria))
                .Cast<TipoCategoria>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = t.ToString()
                }).ToList();

            var categorias = _context.Categorias.AsQueryable();

            // Aplica o filtro se o tipoId for fornecido e for um valor válido do Enum
            if (tipoId.HasValue && Enum.IsDefined(typeof(TipoCategoria), tipoId.Value))
            {
                var tipoFiltro = (TipoCategoria)tipoId.Value;
                categorias = categorias.Where(c => c.Tipo == tipoFiltro);

                // Salva o ID selecionado para manter o filtro ativo na View
                ViewData["SelectedTipoId"] = tipoId.Value;
            }

            // Executa a consulta e retorna a lista filtrada ou completa
            return View(await categorias.ToListAsync());
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tipo")] Categoria categoria)
        {
            // --- VERIFICAÇÃO DE DUPLICIDADE (Nome + Tipo) ---
            // Verifica se já existe uma categoria com o mesmo Nome E Tipo
            var existeDuplicata = await _context.Categorias
                .AnyAsync(c => c.Nome == categoria.Nome && c.Tipo == categoria.Tipo);

            if (existeDuplicata)
            {
                // Se duplicado, adiciona um erro ao ModelState, impedindo o salvamento
                ModelState.AddModelError("Nome", "Já existe uma categoria com este Nome e Tipo (Receita/Despesa).");
            }
            // --------------------------------------------------

            if (ModelState.IsValid) // Valida Data Annotations e o erro de duplicidade acima
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Trata erro de concorrência: verifica se o item ainda existe
                    if (!_context.Categorias.Any(e => e.Id == categoria.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Delete/5 (Confirmação)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Busca a categoria para exibir na tela de confirmação
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5 (Execução da Exclusão)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}