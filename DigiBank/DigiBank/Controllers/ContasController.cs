using System.Diagnostics;
using DigiBank.Models;
using DigiBank.Services;
using DigiBank.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace DigiBank.Controllers
{
    public class ContasController : Controller
    {
        private readonly ContaService _contaService;
        private readonly ClienteService _clienteService;
        private readonly ILogger<ContasController> _logger;

        public ContasController(
            ContaService contaService,
            ClienteService clienteService,
            ILogger<ContasController> logger)
        {
            _contaService = contaService;
            _clienteService = clienteService;
            _logger = logger;
        }

        // GET: Contas por Cliente
        public async Task<IActionResult> Index(int clienteId)
        {
            try
            {
                if (!await _clienteService.ClienteExists(clienteId))
                {
                    return NotFound();
                }

                ViewData["ClienteId"] = clienteId;
                var contas = await _contaService.ObterContasPorCliente(clienteId);
                return View(contas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao listar contas do cliente {clienteId}");
                return RedirectToAction(nameof(Error), new { message = "Ocorreu um erro ao carregar as contas." });
            }
        }

        // GET: Contas/Details/5
        public async Task<IActionResult> Details(string numeroConta)
        {
            if (string.IsNullOrEmpty(numeroConta))
            {
                return NotFound();
            }

            try
            {
                var conta = await _contaService.ObterContaCompleta(numeroConta);
                if (conta == null)
                {
                    return NotFound();
                }

                return View(conta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao detalhar conta {numeroConta}");
                return RedirectToAction(nameof(Error), new { message = "Ocorreu um erro ao carregar os detalhes da conta." });
            }
        }

        // GET: Contas/Create
        public async Task<IActionResult> Create(int clienteId)
        {
            if (!await _clienteService.ClienteExists(clienteId))
            {
                return NotFound();
            }

            ViewData["ClienteId"] = clienteId;
            return View();
        }

        // POST: Contas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Tipo,ClienteId")] Conta conta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _contaService.CriarConta(conta);
                    return RedirectToAction(nameof(Index), new { clienteId = conta.ClienteId });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Erro ao criar conta");
                    ModelState.AddModelError("", "Não foi possível criar a conta. Tente novamente.");
                }
            }

            ViewData["ClienteId"] = conta.ClienteId;
            return View(conta);
        }

        // GET: Contas/Deposito/5
        public async Task<IActionResult> Deposito(string numeroConta)
        {
            return await OperacaoView(numeroConta, "deposito");
        }

        // GET: Contas/Saque/5
        public async Task<IActionResult> Saque(string numeroConta)
        {
            return await OperacaoView(numeroConta, "saque");
        }

        private async Task<IActionResult> OperacaoView(string numeroConta, string tipoOperacao)
        {
            if (string.IsNullOrEmpty(numeroConta))
            {
                return NotFound();
            }

            try
            {
                var conta = await _contaService.ObterContaPorNumero(numeroConta);
                if (conta == null)
                {
                    return NotFound();
                }

                var viewModel = new OperacaoBancariaViewModel
                {
                    NumeroConta = conta.Numero,
                    TipoOperacao = tipoOperacao,
                    NomeCliente = conta.Cliente.Nome
                };

                return View("Operacao", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao preparar operação {tipoOperacao} na conta {numeroConta}");
                return RedirectToAction(nameof(Error), new { message = $"Ocorreu um erro ao preparar a operação de {tipoOperacao}." });
            }
        }

        // POST: Contas/Operacao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Operacao(OperacaoBancariaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    OperacaoResult resultado;

                    switch (model.TipoOperacao.ToLower())
                    {
                        case "deposito":
                            resultado = await _contaService.Depositar(model.NumeroConta, model.Valor);
                            break;
                        case "saque":
                            resultado = await _contaService.Sacar(model.NumeroConta, model.Valor);
                            break;
                        default:
                            return BadRequest();
                    }

                    if (resultado.Sucesso)
                    {
                        return RedirectToAction(nameof(Details), new { numeroConta = model.NumeroConta });
                    }

                    ModelState.AddModelError(string.Empty, resultado.Mensagem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro durante operação {model.TipoOperacao} na conta {model.NumeroConta}");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro durante a operação. Tente novamente.");
                }
            }

            return View("Operacao", model);
        }

        // GET: Contas/Extrato/5
        public async Task<IActionResult> Extrato(string numeroConta)
        {
            if (string.IsNullOrEmpty(numeroConta))
            {
                return NotFound();
            }

            try
            {
                var extrato = await _contaService.ObterExtrato(numeroConta);
                if (extrato == null)
                {
                    return NotFound();
                }

                ViewData["NumeroConta"] = numeroConta;
                return View(extrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao gerar extrato da conta {numeroConta}");
                return RedirectToAction(nameof(Error), new { message = "Ocorreu um erro ao gerar o extrato." });
            }
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