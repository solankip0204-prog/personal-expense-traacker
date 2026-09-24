using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleAdmin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalIncomeTransactions = await _context.Incomes.CountAsync();
            ViewBag.TotalExpenseTransactions = await _context.Expenses.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalTransactionAmount = (await _context.Incomes.SumAsync(i => (decimal?)i.Amount) ?? 0)
                                              + (await _context.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0);
            ViewBag.TotalExpensesAmount = await _context.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0;

            return View();
        }

        // ---------------- User Management ----------------

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.CreatedDate,
                    TransactionCount = _context.Expenses.Count(e => e.UserId == u.UserId)
                                       + _context.Incomes.Count(i => i.UserId == u.UserId)
                })
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.ExpenseCount = await _context.Expenses.CountAsync(e => e.UserId == id);
            ViewBag.IncomeCount = await _context.Incomes.CountAsync(i => i.UserId == id);
            ViewBag.TotalExpenses = await _context.Expenses.Where(e => e.UserId == id).SumAsync(e => (decimal?)e.Amount) ?? 0;
            ViewBag.TotalIncome = await _context.Incomes.Where(i => i.UserId == id).SumAsync(i => (decimal?)i.Amount) ?? 0;

            // Note: password hash is intentionally never passed to the view.
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user); // cascades to their expenses/incomes/budgets/categories
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(nameof(Users));
        }

        // ---------------- Category Management ----------------

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.CategoryType,
                    IsSystemDefault = c.UserId == null,
                    TransactionCount = _context.Expenses.Count(e => e.CategoryId == c.CategoryId)
                                       + _context.Incomes.Count(i => i.CategoryId == c.CategoryId)
                })
                .OrderBy(c => c.CategoryType).ThenBy(c => c.CategoryName)
                .ToListAsync();

            return View(categories);
        }

        public IActionResult CreateCategory()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            ModelState.Remove(nameof(Category.User));
            if (!ModelState.IsValid) return View(model);

            var category = new Category
            {
                CategoryName = model.CategoryName.Trim(),
                CategoryType = model.CategoryType,
                UserId = null // system-wide category
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category created successfully.";
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category model)
        {
            if (id != model.CategoryId) return NotFound();
            ModelState.Remove(nameof(Category.User));
            if (!ModelState.IsValid) return View(model);

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.CategoryName = model.CategoryName.Trim();
            category.CategoryType = model.CategoryType;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            bool inUse = await _context.Expenses.AnyAsync(e => e.CategoryId == id)
                         || await _context.Incomes.AnyAsync(i => i.CategoryId == id)
                         || await _context.Budgets.AnyAsync(b => b.CategoryId == id);

            if (inUse)
            {
                TempData["ErrorMessage"] = "This category cannot be deleted because it is used by existing transactions.";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Categories));
        }

        // ---------------- Transaction Management ----------------

        public async Task<IActionResult> Transactions()
        {
            var expenses = await _context.Expenses.Include(e => e.Category).Include(e => e.User)
                .Select(e => new AdminTransactionRow
                {
                    Id = e.ExpenseId,
                    Type = "Expense",
                    UserName = e.User.FullName,
                    CategoryName = e.Category.CategoryName,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.ExpenseDate,
                    PaymentMethod = e.PaymentMethod.ToString()
                }).ToListAsync();

            var incomes = await _context.Incomes.Include(i => i.Category).Include(i => i.User)
                .Select(i => new AdminTransactionRow
                {
                    Id = i.IncomeId,
                    Type = "Income",
                    UserName = i.User.FullName,
                    CategoryName = i.Category.CategoryName,
                    Amount = i.Amount,
                    Description = i.Description ?? i.Source,
                    Date = i.IncomeDate,
                    PaymentMethod = "-"
                }).ToListAsync();

            var all = expenses.Concat(incomes).OrderByDescending(t => t.Date).ToList();
            return View(all);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(int id, string type)
        {
            if (type == "Expense")
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense != null)
                {
                    _context.Expenses.Remove(expense);
                    await _context.SaveChangesAsync();
                }
            }
            else if (type == "Income")
            {
                var income = await _context.Incomes.FindAsync(id);
                if (income != null)
                {
                    _context.Incomes.Remove(income);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Transaction deleted successfully.";
            return RedirectToAction(nameof(Transactions));
        }

        // ---------------- Reports ----------------

        public async Task<IActionResult> Reports()
        {
            var categoryTotals = await _context.Expenses
                .Include(e => e.Category)
                .GroupBy(e => e.Category.CategoryName)
                .Select(g => new { CategoryName = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderByDescending(g => g.Total)
                .ToListAsync();

            ViewBag.CategoryLabels = categoryTotals.Select(c => c.CategoryName).ToList();
            ViewBag.CategoryTotals = categoryTotals.Select(c => c.Total).ToList();

            return View();
        }
    }

    public class AdminTransactionRow
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
    }
}
