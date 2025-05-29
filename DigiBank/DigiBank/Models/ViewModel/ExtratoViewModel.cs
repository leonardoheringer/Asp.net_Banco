using DigiBank.Models;
using System.Collections.Generic;

namespace DigiBank.ViewModels
{
    public class ExtratoViewModel
    {
        public string NumeroConta { get; set; }
        public string NomeCliente { get; set; }
        public decimal SaldoAtual { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public List<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}