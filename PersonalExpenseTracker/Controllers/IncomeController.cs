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
    public class IncomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Income  (supports search by source/category)
        public async Task<IActionResult> Index(string search, int? categoryId, DateTime? startDate, DateTime? endDate, string sortBy)
        {
            int userId = CurrentUser.GetUserId(User);

            var query = _context.Incomes.Include(i => i.Category).Where(i => i.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i => i.Source.Contains(search) || i.Category.CategoryName.Contains(search));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(i => i.IncomeDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(i => i.IncomeDate <= endDate.Value);
            }

            query = sortBy switch
            {
                "amount_asc" => query.OrderBy(i => i.Amount),
                "amount_desc" => query.OrderByDescending(i => i.Amount),
                "date_asc" => query.OrderBy(i => i.IncomeDate),
                _ => query.OrderByDescending(i => i.IncomeDate)
            };

            ViewBag.Categories = await _context.Categories
                .Where(c => c.CategoryType == CategoryType.Income && (c.UserId == null || c.UserId == userId))
                .OrderBy(c => c.CategoryName).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;

            var incomes = await query.ToListAsync();
            return View(incomes);
        }

        public async Task<IActionResult> Details(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var income = await _context.Incomes.Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();
            return View(income);
        }

        public async Task<IActionResult> Create()
        {
            var model = new IncomeViewModel { Categories = await GetIncomeCategories() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetIncomeCategories();
                return View(model);
            }

            int userId = CurrentUser.GetUserId(User);
            var income = new Income
            {
                UserId = userId,
                CategoryId = model.CategoryId,
                Amount = model.Amount,
                Source = model.Source,
                IncomeDate = model.IncomeDate,
                Description = model.Description,
                Notes = model.Notes,
                CreatedDate = DateTime.Now
            };

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Income added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var income = await _context.Incomes.FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();

            var model = new IncomeViewModel
            {
                IncomeId = income.IncomeId,
                Amount = income.Amount,
                CategoryId = income.CategoryId,
                Source = income.Source,
                IncomeDate = income.IncomeDate,
                Description = income.Description,
                Notes = income.Notes,
                Categories = await GetIncomeCategories()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IncomeViewModel model)
        {
            if (id != model.IncomeId) return NotFound();

            int userId = CurrentUser.GetUserId(User);
            var income = await _context.Incomes.FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Categories = await GetIncomeCategories();
                return View(model);
            }

            income.Amount = model.Amount;
            income.CategoryId = model.CategoryId;
            income.Source = model.Source;
            income.IncomeDate = model.IncomeDate;
            income.Description = model.Description;
            income.Notes = model.Notes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Income updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var income = await _context.Incomes.Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();

            return View(income);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var income = await _context.Incomes.FirstOrDefaultAsync(i => i.IncomeId == id && i.UserId == userId);
            if (income == null) return NotFound();

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Income deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Category>> GetIncomeCategories()
        {
            int userId = CurrentUser.GetUserId(User);
            return await _context.Categories
                .Where(c => c.CategoryType == CategoryType.Income && (c.UserId == null || c.UserId == userId))
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }
    }
}
