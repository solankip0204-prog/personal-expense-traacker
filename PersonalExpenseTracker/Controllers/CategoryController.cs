using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleUser)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Shows system default categories plus the user's own custom categories.
        public async Task<IActionResult> Index()
        {
            int userId = CurrentUser.GetUserId(User);
            var categories = await _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .OrderBy(c => c.CategoryType).ThenBy(c => c.CategoryName)
                .ToListAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            ModelState.Remove(nameof(Category.User));
            if (!ModelState.IsValid) return View(model);

            int userId = CurrentUser.GetUserId(User);

            bool duplicate = await _context.Categories.AnyAsync(c =>
                c.CategoryName.ToLower() == model.CategoryName.ToLower() &&
                c.CategoryType == model.CategoryType &&
                (c.UserId == null || c.UserId == userId));

            if (duplicate)
            {
                ModelState.AddModelError("CategoryName", "A category with this name already exists.");
                return View(model);
            }

            var category = new Category
            {
                CategoryName = model.CategoryName.Trim(),
                CategoryType = model.CategoryType,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            // Users may only edit categories they personally created (not system defaults).
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.CategoryId) return NotFound();
            ModelState.Remove(nameof(Category.User));
            if (!ModelState.IsValid) return View(model);

            int userId = CurrentUser.GetUserId(User);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            category.CategoryName = model.CategoryName.Trim();
            category.CategoryType = model.CategoryType;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            bool inUse = await _context.Expenses.AnyAsync(e => e.CategoryId == id)
                         || await _context.Incomes.AnyAsync(i => i.CategoryId == id)
                         || await _context.Budgets.AnyAsync(b => b.CategoryId == id);

            ViewBag.InUse = inUse;
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = CurrentUser.GetUserId(User);
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            if (category == null) return NotFound();

            bool inUse = await _context.Expenses.AnyAsync(e => e.CategoryId == id)
                         || await _context.Incomes.AnyAsync(i => i.CategoryId == id)
                         || await _context.Budgets.AnyAsync(b => b.CategoryId == id);

            if (inUse)
            {
                TempData["ErrorMessage"] = "This category cannot be deleted because it is used by existing transactions.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
