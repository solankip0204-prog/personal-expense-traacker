using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleUser)]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? month, int? year)
        {
            int userId = CurrentUser.GetUserId(User);
            int selectedMonth = month ?? DateTime.Today.Month;
            int selectedYear = year ?? DateTime.Today.Year;

            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId && b.Month == selectedMonth && b.Year == selectedYear)
                .ToListAsync();

            // Calculate amount spent per category for the selected month/year
            foreach (var budget in budgets)
            {
                budget.SpentAmount = await _context.Expenses
                    .Where(e => e.UserId == userId && e.CategoryId == budget.CategoryId
                                && e.ExpenseDate.Month == selectedMonth && e.ExpenseDate.Year == selectedYear)
                    .SumAsync(e => (decimal?)e.Amount) ?? 0;
            }

            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;
            return View(budgets);
        }

        public async Task<IActionResult> Create()
        {
            var model = new BudgetViewModel { Categories = await GetExpenseCategories() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetExpenseCategories();
                return View(model);
            }

            int userId = CurrentUser.GetUserId(User);

            bool duplicate = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId && b.CategoryId == model.CategoryId &&
                b.Month == model.Month && b.Year == model.Year);

            if (duplicate)
            {
                ModelState.AddModelError(string.Empty, "A budget for this category and month already exists. Please edit it instead.");
                model.Categories = await GetExpenseCategories();
                return View(model);
            }

            var budget = new Budget
            {
                UserId = userId,
                CategoryId = model.CategoryId,
                Amount = model.Amount,
                Month = model.Month,
                Year = model.Year,
                CreatedDate = DateTime.Now
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Budget created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);
            if (budget == null) return NotFound();

            var model = new BudgetViewModel
            {
                BudgetId = budget.BudgetId,
                CategoryId = budget.CategoryId,
                Amount = budget.Amount,
                Month = budget.Month,
                Year = budget.Year,
                Categories = await GetExpenseCategories()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BudgetViewModel model)
        {
            if (id != model.BudgetId) return NotFound();

            int userId = CurrentUser.GetUserId(User);
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);
            if (budget == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Categories = await GetExpenseCategories();
                return View(model);
            }

            budget.CategoryId = model.CategoryId;
            budget.Amount = model.Amount;
            budget.Month = model.Month;
            budget.Year = model.Year;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Budget updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var budget = await _context.Budgets.Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);
            if (budget == null) return NotFound();

            return View(budget);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.BudgetId == id && b.UserId == userId);
            if (budget == null) return NotFound();

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Budget deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Category>> GetExpenseCategories()
        {
            int userId = CurrentUser.GetUserId(User);
            return await _context.Categories
                .Where(c => c.CategoryType == CategoryType.Expense && (c.UserId == null || c.UserId == userId))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }
    }
}
