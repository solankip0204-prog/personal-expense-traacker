using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Helpers;
using PersonalExpenseTracker.Models;
using PersonalExpenseTracker.ViewModels;

namespace PersonalExpenseTracker.Controllers
{
    // Handles registration, login (for both users and admin) and logout.
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- REGISTER ----------------

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered. Please login instead.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PhoneNumber = model.PhoneNumber.Trim(),
                PasswordHash = PasswordHasher.HashPassword(model.Password),
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login with your new account.";
            return RedirectToAction("Login");
        }

        // ---------------- LOGIN (User) ----------------

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            await SignInUserAsync(user, model.RememberMe);

            return RedirectToAction("Index", "Dashboard");
        }

        private async Task SignInUserAsync(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, AuthConstants.RoleUser)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(4)
                });
        }

        // ---------------- LOGIN (Admin) ----------------

        [HttpGet]
        public IActionResult AdminLogin()
        {
            if (User.Identity?.IsAuthenticated == true && User.IsInRole(AuthConstants.RoleAdmin))
                return RedirectToAction("Index", "Admin");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email.ToLower() == model.Email.ToLower());

            if (admin == null || !PasswordHasher.VerifyPassword(model.Password, admin.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid admin email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new(ClaimTypes.Name, admin.FullName),
                new(ClaimTypes.Email, admin.Email),
                new(ClaimTypes.Role, AuthConstants.RoleAdmin)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4) });

            return RedirectToAction("Index", "Admin");
        }

        // ---------------- LOGOUT ----------------

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            bool wasAdmin = User.IsInRole(AuthConstants.RoleAdmin);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return wasAdmin ? RedirectToAction("AdminLogin") : RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
