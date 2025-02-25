using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstMVCApp.Data;
using MyFirstMVCApp.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyFirstMVCApp.Controllers
{
    public class WindowsController : Controller
    {
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select an Excel file.";
                return RedirectToAction("Index");
            }

            List<Window> windowsList = new List<Window>();

            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);

                // Fix: Set License Context for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet? worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet == null)
                    {
                        TempData["Error"] = "Invalid Excel file or empty worksheet.";
                        return RedirectToAction("Index");
                    }

                    int rowCount = worksheet.Dimension?.Rows ?? 0; // Handle possible null

                    if (rowCount < 2) // Ensure there is at least one data row
                    {
                        TempData["Error"] = "Excel file contains no data.";
                        return RedirectToAction("Index");
                    }

                    for (int row = 2; row <= rowCount; row++) // Assuming first row is header
                    {
                        windowsList.Add(new Window
                        {
                            Name = worksheet.Cells[row, 1].Value?.ToString() ?? "Unnamed",
                            Width = double.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out double width) ? width : 0,
                            Height = double.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out double height) ? height : 0
                        });
                    }
                }
            }

            // Save to database
            _context.Windows.AddRange(windowsList);
            _context.SaveChanges();

            TempData["Success"] = "Windows data uploaded successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var window = _context.Windows.Find(id);
            if (window == null)
            {
                return NotFound();
            }
            return View(window);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var window = _context.Windows.Find(id);
            if (window != null)
            {
                _context.Windows.Remove(window);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var window = _context.Windows.Find(id);
            if (window == null)
            {
                return NotFound();
            }
            return View(window);
        }

        [HttpPost]
        public IActionResult Edit(int id, Window updatedWindow)
        {
            if (id != updatedWindow.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Windows.Update(updatedWindow);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(updatedWindow);
        }

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