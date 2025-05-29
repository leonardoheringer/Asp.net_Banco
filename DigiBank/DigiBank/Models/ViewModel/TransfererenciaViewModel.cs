using System.ComponentModel.DataAnnotations;

namespace DigiBank.ViewModels
{
    public class TransferenciaViewModel
    {
        [Required(ErrorMessage = "A conta de origem é obrigatória")]
        [Display(Name = "Conta de Origem")]
        public string ContaOrigem { get; set; }

        [Required(ErrorMessage = "A conta de destino é obrigatória")]
        [Display(Name = "Conta de Destino")]
        [NotEqual("ContaOrigem", ErrorMessage = "A conta de destino não pode ser igual à conta de origem")]
        public string ContaDestino { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string Descricao { get; set; }

        // Para exibição apenas
        public string NomeClienteOrigem { get; set; }
        public string NomeClienteDestino { get; set; }
    }
}