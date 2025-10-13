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

        // ✅ READ — List all users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // ✅ CREATE — Display form
        public IActionResult Create()
        {
            return View();
        }

        // ✅ CREATE — Save new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                // 1️⃣ Save user first (to get UserId)
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 2️⃣ If role = Staff, create related Staff record
                if (user.Role == "Staff")
                {
                    var staff = new Staff
                    {
                        UserId = user.UserId,
                        Name = user.FullName ?? user.Username,
                        Phone = "N/A",
                        Position = "Not Assigned"
                    };

                    _context.Staff.Add(staff);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // ✅ EDIT — Display form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ✅ EDIT — Save changes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Update related Staff info if needed
                    if (user.Role == "Staff")
                    {
                        var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                        if (staff == null)
                        {
                            _context.Staff.Add(new Staff
                            {
                                UserId = user.UserId,
                                Name = user.FullName ?? user.Username,
                                Phone = "N/A",
                                Position = "Not Assigned"
                            });
                        }
                        else
                        {
                            staff.Name = user.FullName ?? user.Username;
                            _context.Staff.Update(staff);
                        }

                        await _context.SaveChangesAsync();
                    }

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

        // ✅ DELETE — Confirm page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null) return NotFound();

            return View(user);
        }

        // ✅ DELETE — Execute deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Remove related staff if role = staff
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.UserId == id);
                if (staff != null)
                    _context.Staff.Remove(staff);

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
