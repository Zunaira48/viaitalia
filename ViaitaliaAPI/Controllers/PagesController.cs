using Microsoft.AspNetCore.Mvc;

namespace ViaitaliaAPI.Controllers
{
    public class PagesController : Controller
    {
        // GET: /Pages/AboutUs
        public IActionResult AboutUs()
        {
            return View();
        }

        // GET: /Pages/ContactUs
        public IActionResult ContactUs()
        {
            return View();
        }

        // GET: /Pages/Blog
        public IActionResult Blog()
        {
            return View();
        }

        // GET: /Pages/PrivacyPolicy
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        // GET: /Pages/Terms
        public IActionResult Terms()
        {
            return View();
        }
    }
}
