using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleUser)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int userId = CurrentUser.GetUserId(User);
            var today = DateTime.Today;

            var totalIncome = await _context.Incomes.Where(i => i.UserId == userId).SumAsync(i => (decimal?)i.Amount) ?? 0;
            var totalExpenses = await _context.Expenses.Where(e => e.UserId == userId).SumAsync(e => (decimal?)e.Amount) ?? 0;

            var currentMonthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId && e.ExpenseDate.Month == today.Month && e.ExpenseDate.Year == today.Year)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;

            var incomeCount = await _context.Incomes.CountAsync(i => i.UserId == userId);
            var expenseCount = await _context.Expenses.CountAsync(e => e.UserId == userId);

            var recentExpenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.ExpenseId)
                .Take(5)
                .Select(e => new RecentTransactionViewModel
                {
                    Type = "Expense",
                    CategoryName = e.Category.CategoryName,
                    Amount = e.Amount,
                    Description = e.Description,
                    Date = e.ExpenseDate
                }).ToListAsync();

            var recentIncomes = await _context.Incomes
                .Include(i => i.Category)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.IncomeDate).ThenByDescending(i => i.IncomeId)
                .Take(5)
                .Select(i => new RecentTransactionViewModel
                {
                    Type = "Income",
                    CategoryName = i.Category.CategoryName,
                    Amount = i.Amount,
                    Description = i.Description,
                    Date = i.IncomeDate
                }).ToListAsync();

            var model = new DashboardViewModel
            {
                UserName = User.Identity?.Name ?? "User",
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                CurrentMonthExpenses = currentMonthExpenses,
                TransactionCount = incomeCount + expenseCount,
                RecentTransactions = recentExpenses.Concat(recentIncomes)
                    .OrderByDescending(t => t.Date)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }
    }
}
