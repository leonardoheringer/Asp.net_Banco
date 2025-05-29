using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigiBank.Models
{
    public class Transacao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tipo de Transação")]
        public TipoTransacao Tipo { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [Required]
        [Display(Name = "Data/Hora")]
        public DateTime Data { get; set; } = DateTime.Now;

        [StringLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres")]
        public string Descricao { get; set; }

        // Chave estrangeira
        [Display(Name = "Conta")]
        public int ContaId { get; set; }

        // Navegação
        [ForeignKey("ContaId")]
        public virtual Conta Conta { get; set; }
    }
}