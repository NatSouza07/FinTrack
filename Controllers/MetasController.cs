using System;
using System.Linq;
using System.Threading.Tasks;
using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    [Authorize]
    public class MetasController : Controller
    {
        private readonly FinTrackContext _context;
        private readonly UserManager<Usuario> _userManager;

        public MetasController(FinTrackContext context, UserManager<Usuario> userManager)
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
            if (usuarioId is null) return Unauthorized();

            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var meta in metas)
            {
                decimal progresso = await _context.Transacoes
                    .Where(t => t.UsuarioId == usuarioId &&
                                 t.Tipo == Transacao.TipoTransacao.Entrada &&
                                 t.Data.Month == meta.Mes &&
                                 t.Data.Year == meta.Ano)
                    .SumAsync(t => (decimal?)t.Valor) ?? 0;

                meta.ProgressoCalculado = progresso;
                meta.Porcentagem = meta.ValorMeta == 0 ? 0 : (int)((progresso / meta.ValorMeta) * 100);
            }

            return View(metas);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            decimal progresso = await _context.Transacoes
                .Where(t => t.UsuarioId == usuarioId &&
                             t.Tipo == Transacao.TipoTransacao.Entrada &&
                             t.Data.Month == meta.Mes &&
                             t.Data.Year == meta.Ano)
                .SumAsync(t => (decimal?)t.Valor) ?? 0;

            meta.ProgressoCalculado = progresso;
            meta.Porcentagem = meta.ValorMeta == 0 ? 0 : (int)((progresso / meta.ValorMeta) * 100);

            return View(meta);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meta meta)
        {
            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            ModelState.Remove(nameof(Meta.ValorMeta));
            ModelState.Remove(nameof(Meta.UsuarioId));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(meta);
            }

            try
            {
                meta.UsuarioId = usuarioId;
                _context.Add(meta);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Meta criada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao criar meta: {ex.Message}";
                return View(meta);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(meta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meta meta)
        {
            if (id != meta.Id)
            {
                TempData["Error"] = "Meta inválida.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            var metaDb = await _context.Metas.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (metaDb == null)
            {
                TempData["Error"] = "Você não tem permissão para editar esta meta.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove(nameof(Meta.ValorMeta));
            ModelState.Remove(nameof(Meta.UsuarioId));

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                return View(meta);
            }

            try
            {
                meta.UsuarioId = usuarioId;
                _context.Update(meta);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Meta atualizada com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao atualizar meta: {ex.Message}";
                return View(meta);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(meta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _GetUserId();
            if (usuarioId is null) return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
            {
                TempData["Error"] = "Meta não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Metas.Remove(meta);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Meta excluída com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao excluir meta: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}