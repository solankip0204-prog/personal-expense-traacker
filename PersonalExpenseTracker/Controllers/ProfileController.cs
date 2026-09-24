using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    [Authorize(Roles = AuthConstants.RoleUser)]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int userId = CurrentUser.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate
            };

            return View(model);
        }

        public async Task<IActionResult> Edit()
        {
            int userId = CurrentUser.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            int userId = CurrentUser.GetUserId(User);
            if (userId != model.UserId) return Forbid();

            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            bool emailTaken = await _context.Users.AnyAsync(u => u.UserId != userId && u.Email.ToLower() == model.Email.ToLower());
            if (emailTaken)
            {
                ModelState.AddModelError("Email", "This email is already in use by another account.");
                return View(model);
            }

            user.FullName = model.FullName.Trim();
            user.Email = model.Email.Trim().ToLower();
            user.PhoneNumber = model.PhoneNumber.Trim();

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            int userId = CurrentUser.GetUserId(User);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!PasswordHasher.VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            user.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
