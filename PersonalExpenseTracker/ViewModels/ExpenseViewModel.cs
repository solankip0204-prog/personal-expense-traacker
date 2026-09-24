using System.ComponentModel.DataAnnotations;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.ViewModels
{
    public class ExpenseViewModel
    {
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Expense date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Expense Date")]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select a payment method.")]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public List<Category> Categories { get; set; } = new();
    }
}
