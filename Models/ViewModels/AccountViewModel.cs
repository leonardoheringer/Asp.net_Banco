using System.ComponentModel.DataAnnotations;

namespace Banco.Models.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número da Conta")]
        public string AccountNumber { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        [Display(Name = "Saldo")]
        public decimal Balance { get; set; }

        [Display(Name = "Titular")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }
    }
}