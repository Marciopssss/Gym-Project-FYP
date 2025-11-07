using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- SIGN UP ----------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if username or email already exists
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);

                if (existingUser != null)
                {
                    ViewBag.Error = "Username or Email already exists!";
                    return View(user);
                }

                _context.Users.Add(user);
                _context.SaveChanges();

                ViewBag.Success = "Account created successfully!";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Save user info in session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role ?? "User");

                // Redirect based on role
                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password!";
            return View();
        }

        // ---------- LOGOUT ----------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        // ✅ View Profile
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return View(user);
        }

        // ✅ Edit Profile (GET)
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // ✅ Edit Profile (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User updatedUser)
        {
            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("FullName");
            if (!ModelState.IsValid)
            {
                // 👇 Log validation issues (for debugging)
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(updatedUser);
            }

            // ✅ Ensure UserId is fetched from session if it's missing
            if (updatedUser.UserId == 0)
            {
                var username = HttpContext.Session.GetString("Username");
                var userFromSession = _context.Users.FirstOrDefault(u => u.Username == username);
                if (userFromSession != null)
                    updatedUser.UserId = userFromSession.UserId;
            }

            var user = await _context.Users.FindAsync(updatedUser.UserId);
            if (user == null) return NotFound();

            // ✅ Update editable fields only
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Address = updatedUser.Address;
            user.Age = updatedUser.Age;
            user.Gender = updatedUser.Gender;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> ViewPlans()
        {
            var plans = await _context.Memberships.ToListAsync();
            return View(plans);
        }

        




    }
}
