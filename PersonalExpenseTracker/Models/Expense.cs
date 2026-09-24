using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalExpenseTracker.Models
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        UPI,
        BankTransfer,
        Other
    }

    public class Expense
    {
        public int ExpenseId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required, Range(0.01, 10000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, StringLength(250)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
