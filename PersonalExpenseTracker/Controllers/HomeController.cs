using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PersonalExpenseTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Features()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string message)
        {
            // In a real application this would send an email or save an enquiry.
            TempData["SuccessMessage"] = "Thank you for reaching out! We will get back to you soon.";
            return RedirectToAction("Contact");
        }

        public IActionResult StatusCode(int id)
        {
            ViewData["StatusCodeValue"] = id;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
