using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Coffee_Store.Data;
using Coffee_Store.Models;
using Microsoft.AspNetCore.Authorization;

namespace Coffee_Store.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Verify if current user is yungjoky
        private bool IsAuthorized()
        {
            return User.Identity.IsAuthenticated && User.Identity.Name == "yungjoky";
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            // Change from User to Users
            var users = await _context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Password")] User user)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.UtcNow;
                _context.Users.Add(user); // Change from User to Users
                await _context.SaveChangesAsync();
                TempData["Success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var User = new User
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return View(User);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,Email")] User userViewModel)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != userViewModel.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.Name = userViewModel.Name;
                    user.Email = userViewModel.Email;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "User updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userViewModel.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAuthorized())
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}