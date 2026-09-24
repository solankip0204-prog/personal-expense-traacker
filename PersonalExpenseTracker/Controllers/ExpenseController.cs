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
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expense
        // Supports search (description/category) and filters (category, date range, payment method, amount range)
        public async Task<IActionResult> Index(string search, int? categoryId, DateTime? startDate, DateTime? endDate,
            PaymentMethod? paymentMethod, decimal? minAmount, decimal? maxAmount, string sortBy)
        {
            int userId = CurrentUser.GetUserId(User);

            var query = _context.Expenses.Include(e => e.Category).Where(e => e.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => e.Description.Contains(search) || e.Category.CategoryName.Contains(search));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(e => e.ExpenseDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(e => e.ExpenseDate <= endDate.Value);
            }
            if (paymentMethod.HasValue)
            {
                query = query.Where(e => e.PaymentMethod == paymentMethod.Value);
            }
            if (minAmount.HasValue)
            {
                query = query.Where(e => e.Amount >= minAmount.Value);
            }
            if (maxAmount.HasValue)
            {
                query = query.Where(e => e.Amount <= maxAmount.Value);
            }

            query = sortBy switch
            {
                "amount_asc" => query.OrderBy(e => e.Amount),
                "amount_desc" => query.OrderByDescending(e => e.Amount),
                "date_asc" => query.OrderBy(e => e.ExpenseDate),
                _ => query.OrderByDescending(e => e.ExpenseDate)
            };

            ViewBag.Categories = await _context.Categories
                .Where(c => c.CategoryType == CategoryType.Expense && (c.UserId == null || c.UserId == userId))
                .OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.MinAmount = minAmount;
            ViewBag.MaxAmount = maxAmount;
            ViewBag.SortBy = sortBy;

            var expenses = await query.ToListAsync();
            return View(expenses);
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var expense = await _context.Expenses.Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);

            if (expense == null) return NotFound();
            return View(expense);
        }

        // GET: Expense/Create
        public async Task<IActionResult> Create()
        {
            var model = new ExpenseViewModel { Categories = await GetExpenseCategories() };
            return View(model);
        }

        // POST: Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetExpenseCategories();
                return View(model);
            }

            int userId = CurrentUser.GetUserId(User);
            var expense = new Expense
            {
                UserId = userId,
                CategoryId = model.CategoryId,
                Amount = model.Amount,
                Description = model.Description,
                ExpenseDate = model.ExpenseDate,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes,
                CreatedDate = DateTime.Now
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Expense added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            var model = new ExpenseViewModel
            {
                ExpenseId = expense.ExpenseId,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                Description = expense.Description,
                ExpenseDate = expense.ExpenseDate,
                PaymentMethod = expense.PaymentMethod,
                Notes = expense.Notes,
                Categories = await GetExpenseCategories()
            };

            return View(model);
        }

        // POST: Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseViewModel model)
        {
            if (id != model.ExpenseId) return NotFound();

            int userId = CurrentUser.GetUserId(User);
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Categories = await GetExpenseCategories();
                return View(model);
            }

            expense.Amount = model.Amount;
            expense.CategoryId = model.CategoryId;
            expense.Description = model.Description;
            expense.ExpenseDate = model.ExpenseDate;
            expense.PaymentMethod = model.PaymentMethod;
            expense.Notes = model.Notes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Expense updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var expense = await _context.Expenses.Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpenseId == id && e.UserId == userId);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Expense deleted successfully.";
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
