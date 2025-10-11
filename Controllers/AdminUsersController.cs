using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Helper: Restrict access to Admins only
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // ---------- READ (List all users) ----------
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // ---------- CREATE (Add user) ----------
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ If role is "Staff", also add to Staff table
                if (user.Role == "Staff")
                {
                    var staff = new Staff
                    {
                        Name = user.FullName ?? user.Username,
                        Email = user.Email,
                        // You can add more default values as needed
                    };

                    _context.Staff.Add(staff);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction(nameof(Index));

            }

            return View(user);
        }

        // ---------- EDIT (Update user info) ----------
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserId == user.UserId))
                        return NotFound();
                    else
                        throw;
                }
            }

            return View(user);
        }

        // ---------- DELETE (Confirm and delete user) ----------
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "User");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
