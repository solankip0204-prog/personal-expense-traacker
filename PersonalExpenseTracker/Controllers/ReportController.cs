using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleUser)]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Monthly Summary page: pick a month/year, see totals.
        public async Task<IActionResult> MonthlySummary(int? month, int? year)
        {
            int userId = CurrentUser.GetUserId(User);
            int selectedMonth = month ?? DateTime.Today.Month;
            int selectedYear = year ?? DateTime.Today.Year;

            var incomeTotal = await _context.Incomes
                .Where(i => i.UserId == userId && i.IncomeDate.Month == selectedMonth && i.IncomeDate.Year == selectedYear)
                .SumAsync(i => (decimal?)i.Amount) ?? 0;

            var monthExpenses = await _context.Expenses
                .Where(e => e.UserId == userId && e.ExpenseDate.Month == selectedMonth && e.ExpenseDate.Year == selectedYear)
                .ToListAsync();

            var summary = new MonthlySummaryViewModel
            {
                Month = selectedMonth,
                Year = selectedYear,
                TotalIncome = incomeTotal,
                TotalExpenses = monthExpenses.Sum(e => e.Amount),
                NumberOfExpenses = monthExpenses.Count,
                HighestExpense = monthExpenses.Count > 0 ? monthExpenses.Max(e => e.Amount) : 0,
                AverageExpense = monthExpenses.Count > 0 ? Math.Round(monthExpenses.Average(e => e.Amount), 2) : 0
            };

            return View(summary);
        }

        // Expense Report page: category breakdown + charts (Chart.js reads real DB data via JSON).
        public async Task<IActionResult> ExpenseReport(int? month, int? year)
        {
            int userId = CurrentUser.GetUserId(User);
            int selectedMonth = month ?? DateTime.Today.Month;
            int selectedYear = year ?? DateTime.Today.Year;

            var categoryTotals = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId && e.ExpenseDate.Month == selectedMonth && e.ExpenseDate.Year == selectedYear)
                .GroupBy(e => e.Category.CategoryName)
                .Select(g => new CategoryReportItem { CategoryName = g.Key, TotalAmount = g.Sum(e => e.Amount) })
                .OrderByDescending(c => c.TotalAmount)
                .ToListAsync();

            // Last 6 months income vs expense, for the bar chart
            var labels = new List<string>();
            var incomeSeries = new List<decimal>();
            var expenseSeries = new List<decimal>();

            var cursor = new DateTime(selectedYear, selectedMonth, 1).AddMonths(-5);
            for (int i = 0; i < 6; i++)
            {
                var monthIncome = await _context.Incomes
                    .Where(inc => inc.UserId == userId && inc.IncomeDate.Month == cursor.Month && inc.IncomeDate.Year == cursor.Year)
                    .SumAsync(inc => (decimal?)inc.Amount) ?? 0;

                var monthExpense = await _context.Expenses
                    .Where(exp => exp.UserId == userId && exp.ExpenseDate.Month == cursor.Month && exp.ExpenseDate.Year == cursor.Year)
                    .SumAsync(exp => (decimal?)exp.Amount) ?? 0;

                labels.Add(cursor.ToString("MMM yyyy"));
                incomeSeries.Add(monthIncome);
                expenseSeries.Add(monthExpense);

                cursor = cursor.AddMonths(1);
            }

            var model = new ReportViewModel
            {
                Summary = new MonthlySummaryViewModel { Month = selectedMonth, Year = selectedYear },
                ExpenseByCategory = categoryTotals,
                ChartMonthLabels = labels,
                ChartMonthlyIncome = incomeSeries,
                ChartMonthlyExpense = expenseSeries
            };

            return View(model);
        }
    }
}
