using System.ComponentModel.DataAnnotations;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.ViewModels
{
    public class IncomeViewModel
    {
        public int IncomeId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Source is required.")]
        [StringLength(150)]
        public string Source { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Income date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Income Date")]
        public DateTime IncomeDate { get; set; } = DateTime.Today;

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public List<Category> Categories { get; set; } = new();
    }
}
