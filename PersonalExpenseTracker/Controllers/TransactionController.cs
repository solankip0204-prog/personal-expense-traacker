using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Controllers
{
    // A single combined view of both Income and Expense records for the logged-in user.
    public class TransactionRow
    {
        public string Type { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; }
    }

    [Authorize(Roles = AuthConstants.RoleUser)]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string type, int? categoryId,
            DateTime? startDate, DateTime? endDate, string sortBy)
        {
            int userId = CurrentUser.GetUserId(User);

            var expenses = await _context.Expenses.Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .Select(e => new TransactionRow
                {
                    Type = "Expense",
                    CategoryName = e.Category.CategoryName,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.ExpenseDate,
                    PaymentMethod = e.PaymentMethod.ToString()
                }).ToListAsync();

            var incomes = await _context.Incomes.Include(i => i.Category)
                .Where(i => i.UserId == userId)
                .Select(i => new TransactionRow
                {
                    Type = "Income",
                    CategoryName = i.Category.CategoryName,
                    Amount = i.Amount,
                    Description = i.Description ?? i.Source,
                    Date = i.IncomeDate,
                    PaymentMethod = "-"
                }).ToListAsync();

            var all = expenses.Concat(incomes).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(type))
                all = all.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
                all = all.Where(t => t.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || t.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (categoryId.HasValue)
            {
                var catName = await _context.Categories.Where(c => c.CategoryId == categoryId.Value)
                    .Select(c => c.CategoryName).FirstOrDefaultAsync();
                if (catName != null) all = all.Where(t => t.CategoryName == catName);
            }

            if (startDate.HasValue) all = all.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue) all = all.Where(t => t.Date <= endDate.Value);

            all = sortBy switch
            {
                "amount_asc" => all.OrderBy(t => t.Amount),
                "amount_desc" => all.OrderByDescending(t => t.Amount),
                "date_asc" => all.OrderBy(t => t.Date),
                _ => all.OrderByDescending(t => t.Date)
            };

            ViewBag.Categories = await _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Search = search;
            ViewBag.Type = type;
            ViewBag.CategoryId = categoryId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;

            return View(all.ToList());
        }
    }
}
