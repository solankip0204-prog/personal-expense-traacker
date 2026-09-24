using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.Models
{
    public enum CategoryType
    {
        Income,
        Expense
    }

    // A category used to classify income or expense transactions.
    public class Category
    {
        public int CategoryId { get; set; }

        // Nullable: default/system categories have no owning user (UserId == null).
        public int? UserId { get; set; }
        public User User { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
