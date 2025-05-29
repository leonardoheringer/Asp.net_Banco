using System.ComponentModel.DataAnnotations;

namespace DigiBank.Models
{
    public enum TipoConta
    {
        [Display(Name = "Conta Corrente")]
        Corrente,

        [Display(Name = "Conta Poupança")]
        Poupanca,

        [Display(Name = "Conta Investimento")]
        Investimento
    }
}