using System.ComponentModel.DataAnnotations;

namespace DigiBank.Models
{
    public enum TipoTransacao
    {
        [Display(Name = "Depósito")]
        Deposito,

        [Display(Name = "Saque")]
        Saque,

        [Display(Name = "Transferência Enviada")]
        TransferenciaEnviada,

        [Display(Name = "Transferência Recebida")]
        TransferenciaRecebida,

        [Display(Name = "Pagamento")]
        Pagamento,

        [Display(Name = "Rendimento")]
        Rendimento
    }
}