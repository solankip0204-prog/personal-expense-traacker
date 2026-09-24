using System.ComponentModel.DataAnnotations;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.ViewModels
{
    public class BudgetViewModel
    {
        public int BudgetId { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; } = DateTime.Today.Month;

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.Today.Year;

        public List<Category> Categories { get; set; } = new();
    }
}
