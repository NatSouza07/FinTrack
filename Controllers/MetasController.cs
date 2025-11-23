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

        // INDEX
        public async Task<IActionResult> Index()
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            var metas = await _context.Metas
                .Where(m => m.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var meta in metas)
            {
                decimal progresso = await _context.Transacoes
                    .Where(t => t.UsuarioId == usuarioId &&
                                // ajusta conforme seu modelo: aqui assume que Transacao possui enum/tipo que indica entrada/saida
                                // se seu modelo não tiver essa propriedade, remova essa condição
                                t.Tipo == Transacao.TipoTransacao.Entrada &&
                                t.Data.Month == meta.Mes &&
                                t.Data.Year == meta.Ano)
                    .SumAsync(t => (decimal?)t.Valor) ?? 0;

                // popula propriedades auxiliares (use os nomes reais do seu model)
                meta.ProgressoCalculado = progresso;
                meta.Porcentagem = meta.ValorMeta == 0 ? 0 : (int)((progresso / meta.ValorMeta) * 100);
            }

            return View(metas);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return NotFound();

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

        // CREATE (GET)
        public IActionResult Create() => View();

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meta meta)
        {
            if (!ModelState.IsValid)
                return View(meta);

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            meta.UsuarioId = usuarioId;

            _context.Add(meta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return NotFound();

            return View(meta);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meta meta)
        {
            if (id != meta.Id)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(meta);

            meta.UsuarioId = usuarioId;

            _context.Update(meta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return NotFound();

            return View(meta);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null)
                return Unauthorized();

            var meta = await _context.Metas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (meta == null)
                return Unauthorized();

            _context.Metas.Remove(meta);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
