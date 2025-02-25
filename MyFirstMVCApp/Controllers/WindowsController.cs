using Microsoft.AspNetCore.Mvc;
using MyFirstMVCApp.Data;
using MyFirstMVCApp.Models;

namespace MyFirstMVCApp.Controllers
{
    public class WindowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WindowsController(ApplicationDbContext context)
        {
            _context = context;
        }

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
                // Add the Window object to the database
                _context.Windows.Add(window);
                _context.SaveChanges(); // Save the changes to the database

                return RedirectToAction("Success");
            }

            return View(window);
        }

        // Show a success page
        public IActionResult Success()
        {
            return View();
        }

        // New action to display the list of windows
        public IActionResult Index()
        {
            var windowsList = _context.Windows.ToList();
            return View(windowsList);
        }
    }
}