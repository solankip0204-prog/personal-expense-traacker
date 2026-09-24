using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalExpenseTracker.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required, Range(0.01, 10000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, Range(1, 12)]
        public int Month { get; set; }

        [Required, Range(2000, 2100)]
        public int Year { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [NotMapped]
        public decimal SpentAmount { get; set; }

        [NotMapped]
        public decimal RemainingAmount => Amount - SpentAmount;

        [NotMapped]
        public double PercentageUsed => Amount == 0 ? 0 : Math.Round((double)(SpentAmount / Amount) * 100, 1);
    }
}
