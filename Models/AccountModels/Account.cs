using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banco.Models.AccountModels
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;

        // Navigation property for outgoing transactions
        public virtual ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();

        // Navigation property for incoming transactions
        public virtual ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
    }
}