using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banco.Models.AccountModels
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [ForeignKey("FromAccount")]
        public int FromAccountId { get; set; }

        [ForeignKey("ToAccount")]
        public int? ToAccountId { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        // Navigation properties
        public virtual Account FromAccount { get; set; } = null!;
        public virtual Account? ToAccount { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Payment,
        Fee
    }
}