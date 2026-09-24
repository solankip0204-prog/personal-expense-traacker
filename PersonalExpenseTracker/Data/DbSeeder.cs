using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Data
{
    // Seeds the database with demo data so the project can be shown/tested immediately.
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // NOTE: The database itself is created/updated by context.Database.Migrate()
            // in Program.cs (via EF Core migrations). This method only inserts seed data.

            // ---------- Admin ----------
            if (!context.Admins.Any())
            {
                context.Admins.Add(new Admin
                {
                    FullName = "System Administrator",
                    Email = "admin@gmail.com",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    CreatedDate = DateTime.Now
                });
                context.SaveChanges();
            }

            // ---------- Default Categories (system-wide, UserId = null) ----------
            if (!context.Categories.Any())
            {
                var expenseCategoryNames = new[]
                {
                    "Food", "Transportation", "Shopping", "Education", "Entertainment",
                    "Healthcare", "Bills", "Rent", "Travel", "Other"
                };
                var incomeCategoryNames = new[]
                {
                    "Salary", "Freelance", "Business", "Investment", "Scholarship", "Other"
                };

                foreach (var name in expenseCategoryNames)
                {
                    context.Categories.Add(new Category
                    {
                        CategoryName = name,
                        CategoryType = CategoryType.Expense,
                        UserId = null
                    });
                }
                foreach (var name in incomeCategoryNames)
                {
                    context.Categories.Add(new Category
                    {
                        CategoryName = name,
                        CategoryType = CategoryType.Income,
                        UserId = null
                    });
                }
                context.SaveChanges();
            }

            // ---------- Sample Users ----------
            if (!context.Users.Any())
            {
                var sampleUsers = new List<User>
                {
                    new() { FullName = "Aarav Sharma", Email = "aarav.sharma@example.com", PhoneNumber = "9876500001", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Priya Patel", Email = "priya.patel@example.com", PhoneNumber = "9876500002", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Rohan Mehta", Email = "rohan.mehta@example.com", PhoneNumber = "9876500003", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Ananya Iyer", Email = "ananya.iyer@example.com", PhoneNumber = "9876500004", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Vikram Singh", Email = "vikram.singh@example.com", PhoneNumber = "9876500005", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Sneha Reddy", Email = "sneha.reddy@example.com", PhoneNumber = "9876500006", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Karan Gupta", Email = "karan.gupta@example.com", PhoneNumber = "9876500007", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Isha Nair", Email = "isha.nair@example.com", PhoneNumber = "9876500008", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Aditya Kulkarni", Email = "aditya.kulkarni@example.com", PhoneNumber = "9876500009", PasswordHash = PasswordHasher.HashPassword("Password@123") },
                    new() { FullName = "Meera Joshi", Email = "meera.joshi@example.com", PhoneNumber = "9876500010", PasswordHash = PasswordHasher.HashPassword("Password@123") }
                };
                context.Users.AddRange(sampleUsers);
                context.SaveChanges();
            }

            // ---------- Sample Transactions & Budgets (for first 2 demo users) ----------
            if (!context.Expenses.Any() && !context.Incomes.Any())
            {
                var users = context.Users.OrderBy(u => u.UserId).Take(3).ToList();
                var expenseCats = context.Categories.Where(c => c.CategoryType == CategoryType.Expense).ToList();
                var incomeCats = context.Categories.Where(c => c.CategoryType == CategoryType.Income).ToList();

                Category ExpCat(string name) => expenseCats.First(c => c.CategoryName == name);
                Category IncCat(string name) => incomeCats.First(c => c.CategoryName == name);

                var today = DateTime.Today;

                foreach (var user in users)
                {
                    // Income entries
                    context.Incomes.Add(new Income
                    {
                        UserId = user.UserId, CategoryId = IncCat("Salary").CategoryId,
                        Amount = 40000, Source = "Monthly Salary",
                        IncomeDate = new DateTime(today.Year, today.Month, 1),
                        Description = "Monthly salary credit", Notes = ""
                    });
                    context.Incomes.Add(new Income
                    {
                        UserId = user.UserId, CategoryId = IncCat("Freelance").CategoryId,
                        Amount = 5000, Source = "Freelance Project",
                        IncomeDate = today.AddDays(-10),
                        Description = "Freelance web design work", Notes = ""
                    });

                    // Expense entries - realistic Indian amounts
                    var expenseSeed = new (string cat, decimal amt, string desc, PaymentMethod pm, int daysAgo)[]
                    {
                        ("Food", 250, "Lunch at restaurant", PaymentMethod.UPI, 1),
                        ("Food", 800, "Weekly groceries", PaymentMethod.Cash, 4),
                        ("Transportation", 120, "Auto fare", PaymentMethod.UPI, 2),
                        ("Transportation", 1500, "Fuel for bike", PaymentMethod.DebitCard, 8),
                        ("Shopping", 1500, "New clothes", PaymentMethod.CreditCard, 6),
                        ("Education", 2000, "Online course fee", PaymentMethod.BankTransfer, 12),
                        ("Entertainment", 400, "Movie tickets", PaymentMethod.UPI, 3),
                        ("Healthcare", 600, "Pharmacy purchase", PaymentMethod.Cash, 15),
                        ("Bills", 1200, "Electricity bill", PaymentMethod.BankTransfer, 5),
                        ("Rent", 8000, "Monthly room rent", PaymentMethod.BankTransfer, 1),
                    };

                    foreach (var e in expenseSeed)
                    {
                        context.Expenses.Add(new Expense
                        {
                            UserId = user.UserId,
                            CategoryId = ExpCat(e.cat).CategoryId,
                            Amount = e.amt,
                            Description = e.desc,
                            ExpenseDate = today.AddDays(-e.daysAgo),
                            PaymentMethod = e.pm,
                            Notes = ""
                        });
                    }

                    // Budgets for current month
                    context.Budgets.Add(new Budget { UserId = user.UserId, CategoryId = ExpCat("Food").CategoryId, Amount = 5000, Month = today.Month, Year = today.Year });
                    context.Budgets.Add(new Budget { UserId = user.UserId, CategoryId = ExpCat("Transportation").CategoryId, Amount = 2000, Month = today.Month, Year = today.Year });
                    context.Budgets.Add(new Budget { UserId = user.UserId, CategoryId = ExpCat("Shopping").CategoryId, Amount = 3000, Month = today.Month, Year = today.Year });
                }

                context.SaveChanges();
            }
        }
    }
}
