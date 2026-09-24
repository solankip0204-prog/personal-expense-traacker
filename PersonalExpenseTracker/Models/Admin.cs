using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.Models
{
    // Represents an administrator account. Seeded, not self-registered.
    public class Admin
    {
        public int AdminId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
