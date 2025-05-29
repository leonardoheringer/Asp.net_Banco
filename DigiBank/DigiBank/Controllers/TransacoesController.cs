using System.Diagnostics;
using DigiBank.Models;
using DigiBank.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigiBank.Controllers
{
    public class TransacoesController : Controller
    {
        private readonly ContaService _contaService;
        private readonly ILogger<TransacoesController> _logger;

        public TransacoesController(ContaService contaService, ILogger<TransacoesController> logger)
        {
            _contaService = contaService;
            _logger = logger;
        }

        // GET: Transacoes/Transferencia
        public async Task<IActionResult> Transferencia()
        {
            return View();
        }

        // POST: Transacoes/Transferencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferencia(TransferenciaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _contaService.Transferir(
                        model.ContaOrigem,
                        model.ContaDestino,
                        model.Valor);

                    if (resultado.Sucesso)
                    {
                        return RedirectToAction(nameof(Details), "Contas", new { numeroConta = model.ContaOrigem });
                    }

                    ModelState.AddModelError(string.Empty, resultado.Mensagem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro na transferência entre {model.ContaOrigem} e {model.ContaDestino}");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro durante a transferência. Tente novamente.");
                }
            }

            return View(model);
        }

        private IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}