using Microsoft.AspNetCore.Mvc;
using MyFirstMVCApp.Models;

namespace MyFirstMVCApp.Controllers
{
    public class WindowsController : Controller
    {
        // Show the form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission
        [HttpPost]
        public IActionResult Create(Window window)
        {
            if (ModelState.IsValid)
            {
                // Here you would typically save the data to the database
                // For now, we can just return a success view or message
                return RedirectToAction("Success");
            }

            return View(window);
        }

        // Show a success page
        public IActionResult Success()
        {
            return View();
        }
    }
}