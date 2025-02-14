using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Coffee_Store.Data;
using Coffee_Store.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Coffee_Store.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuItemsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: MenuItems
        public async Task<IActionResult> Index()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return View(menuItems);
        }
        // GET: MenuItems/DetailsList
        public async Task<IActionResult> DetailsList()
        {
            ViewBag.CurrentUser = User.Identity?.Name ?? "yungjoky";
            var items = await _context.MenuItems.ToListAsync();
            return View(items);
        }
        // GET: MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Item ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (menuItem == null)
            {
                TempData["ErrorMessage"] = "Item not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CurrentUser = User.Identity?.Name;
            ViewBag.CurrentTime = DateTime.UtcNow;
            return View(menuItem);
        }

        // GET: MenuItems/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new List<string>
            {
                "Hot Coffee",
                "Cold Coffee",
                "Tea",
                "Pastries",
                "Snacks"
            };
            return View();
        }

        // POST: MenuItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuItemId,Name,Description,Price,Category,ImageFile")] MenuItem menuItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    menuItem.CreatedAt = DateTime.UtcNow;
                    if (menuItem.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "menu");
                        string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}_{menuItem.ImageFile.FileName}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        Directory.CreateDirectory(uploadsFolder);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await menuItem.ImageFile.CopyToAsync(fileStream);
                        }

                        menuItem.Image = "/images/menu/" + uniqueFileName;
                    }

                    menuItem.CreatedAt = DateTime.UtcNow;
                    _context.Add(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error creating item: " + ex.Message);
            }

            ViewBag.Categories = new List<string> { "Hot Coffee", "Cold Coffee", "Tea", "Pastries", "Snacks" };
            return View(menuItem);
        }

        // GET: MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Item ID not provided.";
                return RedirectToAction(nameof(Index));
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                TempData["ErrorMessage"] = "Item not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new List<string> { "Hot Coffee", "Cold Coffee", "Tea", "Pastries", "Snacks" };
            ViewBag.CurrentUser = User.Identity?.Name;
            ViewBag.CurrentTime = DateTime.UtcNow;
            return View(menuItem);
        }

        // POST: MenuItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuItemId,Name,Description,Price,Category,Image,ImageFile,CreatedAt")] MenuItem menuItem)
        {
            if (id != menuItem.MenuItemId)
            {
                TempData["ErrorMessage"] = "Invalid item ID.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (menuItem.ImageFile != null)
                    {
                        // Delete old image
                        if (!string.IsNullOrEmpty(menuItem.Image))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, menuItem.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "menu");
                        string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}_{menuItem.ImageFile.FileName}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await menuItem.ImageFile.CopyToAsync(fileStream);
                        }

                        menuItem.Image = "/images/menu/" + uniqueFileName;
                    }

                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.MenuItemId))
                    {
                        TempData["ErrorMessage"] = "Item not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Categories = new List<string> { "Hot Coffee", "Cold Coffee", "Tea", "Pastries", "Snacks" };
            return View(menuItem);
        }

        // GET: MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // If no ID is provided, show the selection page
            if (id == null)
            {
                ViewBag.CurrentUser = "yungjoky"; // Or User.Identity?.Name
                var menuItems = await _context.MenuItems.ToListAsync();
                return View("DeleteSelection", menuItems);
            }

            // If ID is provided, show the confirmation page
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemId == id);

            if (menuItem == null)
            {
                TempData["ErrorMessage"] = "Item not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CurrentUser = "yungjoky"; // Or User.Identity?.Name
            ViewBag.CurrentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.WarningMessage = $"You are about to delete {menuItem.Name}. This action cannot be undone.";

            return View(menuItem);
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(menuItem.Image))
                    {
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, menuItem.Image.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    _context.MenuItems.Remove(menuItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Item deleted successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error deleting item: " + ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}