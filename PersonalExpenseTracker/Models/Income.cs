using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalExpenseTracker.Models
{
    public class Income
    {
        public int IncomeId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required, Range(0.01, 10000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, StringLength(150)]
        public string Source { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IncomeDate { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
