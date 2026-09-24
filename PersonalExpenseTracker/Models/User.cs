using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.Models
{
    // Represents a registered user of the application.
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        // Stores the hashed password only. Never store plain text passwords.
        [Required]
        public string PasswordHash { get; set; }

        [Required, StringLength(15)]
        public string PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Income> Incomes { get; set; } = new List<Income>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
