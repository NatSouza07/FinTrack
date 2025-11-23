using FinTrack.Data;
using FinTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Controllers
{
    [Authorize]
    public class TransacoesController : Controller
    {
        private readonly FinTrackContext _context;
        private readonly UserManager<Usuario> _userManager;

        public TransacoesController(FinTrackContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task PopularSelectListsAsync(string usuarioId, Transacao transacao = null)
        {
            ViewBag.Contas = new SelectList(
                await _context.Contas.Where(c => c.UsuarioId == usuarioId).ToListAsync(),
                "Id",
                "Nome",
                transacao?.ContaId
            );

            ViewBag.Categorias = new SelectList(
                await _context.Categorias.ToListAsync(),
                "Id",
                "Nome",
                transacao?.CategoriaId
            );

            ViewBag.TiposPagamento = new SelectList(
                await _context.TiposPagamento.ToListAsync(),
                "Id",
                "Nome",
                transacao?.TipoPagamentoId
            );

            ViewBag.TiposTransacao = new SelectList(
                Enum.GetValues(typeof(Transacao.TipoTransacao))
            );
        }

        public async Task<IActionResult> Index(
            DateTime? dataInicio,
            DateTime? dataFim,
            int? categoriaId,
            int? contaId,
            int? tipoPagamentoId,
            Transacao.TipoTransacao? tipo)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null) return Unauthorized();

            var query = _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .Include(t => t.TipoPagamento)
                .Where(t => t.UsuarioId == usuarioId)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(t => t.Data >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(t => t.Data <= dataFim.Value);

            if (categoriaId.HasValue)
                query = query.Where(t => t.CategoriaId == categoriaId.Value);

            if (contaId.HasValue)
                query = query.Where(t => t.ContaId == contaId.Value);

            if (tipoPagamentoId.HasValue)
                query = query.Where(t => t.TipoPagamentoId == tipoPagamentoId.Value);

            if (tipo.HasValue)
                query = query.Where(t => t.Tipo == tipo.Value);

            await PopularSelectListsAsync(usuarioId);

            var transacoes = await query
                .OrderByDescending(t => t.Data)
                .ToListAsync();

            return View(transacoes);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Transação não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _userManager.GetUserId(User);

            var transacao = await _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .Include(t => t.TipoPagamento)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                TempData["Error"] = "A transação selecionada não existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(transacao);
        }

        public async Task<IActionResult> Create()
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null) return Unauthorized();

            await PopularSelectListsAsync(usuarioId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transacao transacao)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId is null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha todos os campos obrigatórios.";
                await PopularSelectListsAsync(usuarioId, transacao);
                return View(transacao);
            }

            try
            {
                transacao.UsuarioId = usuarioId;
                _context.Add(transacao);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Transação criada com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Erro ao salvar a transação.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Transação inválida.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _userManager.GetUserId(User);

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                TempData["Error"] = "Transação não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            await PopularSelectListsAsync(usuarioId, transacao);

            return View(transacao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transacao transacao)
        {
            if (id != transacao.Id)
            {
                TempData["Error"] = "Transação inválida.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _userManager.GetUserId(User);

            var transacaoDb = await _context.Transacoes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacaoDb == null)
            {
                TempData["Error"] = "Você não tem permissão para editar esta transação.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Preencha os campos corretamente.";
                await PopularSelectListsAsync(usuarioId, transacao);
                return View(transacao);
            }

            try
            {
                transacao.UsuarioId = usuarioId;
                _context.Update(transacao);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Transação atualizada!";
            }
            catch
            {
                TempData["Error"] = "Erro ao atualizar a transação.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Transação não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioId = _userManager.GetUserId(User);

            var transacao = await _context.Transacoes
                .Include(t => t.Conta)
                .Include(t => t.Categoria)
                .Include(t => t.TipoPagamento)
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                TempData["Error"] = "A transação informada não existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(transacao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (transacao == null)
            {
                TempData["Error"] = "Transação não encontrada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Transacoes.Remove(transacao);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Transação removida com sucesso!";
            }
            catch
            {
                TempData["Error"] = "Erro ao excluir transação.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
