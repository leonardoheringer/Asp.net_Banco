using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiBank.Models
{
    public class Conta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Número da Conta")]
        [StringLength(10, ErrorMessage = "O número da conta deve ter 10 caracteres")]
        public string Numero { get; set; } = GerarNumeroConta();

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = 0;

        [Required]
        [Display(Name = "Tipo de Conta")]
        public TipoConta Tipo { get; set; }

        [Display(Name = "Data de Abertura")]
        public DateTime DataAbertura { get; set; } = DateTime.Now;

        // Chave estrangeira
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        // Navegação
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        private static string GerarNumeroConta()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}