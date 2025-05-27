using System.ComponentModel.DataAnnotations;

namespace Banco.Models.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [StringLength(200, ErrorMessage = "A descrição não pode exceder 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Conta de Origem")]
        public string? FromAccount { get; set; }

        [Required(ErrorMessage = "Conta de Destino é obrigatória")]
        [Display(Name = "Conta de Destino")]
        public string? ToAccount { get; set; }

        public string Type { get; set; } = string.Empty;
    }
}