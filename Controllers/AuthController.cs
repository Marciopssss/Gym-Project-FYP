using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;


namespace Gym_Membership.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role ?? "User");

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    ViewBag.Error = "Username already exists!";
                    return View(user);
                }

                // Default new users as normal Users
                user.Role = "User";

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // ---------- LOGOUT ----------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        

public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;

            if (claims == null)
            {
                TempData["Error"] = "Google login failed — no user data received.";
                return RedirectToAction("Login", "Auth");
            }

            // Extract user info from Google claims
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // ✅ New safety check
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Your Google account did not return an email address. Please enable email access or use another account.";
                return RedirectToAction("Login", "Auth");
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            {
                // Create new user if not exists
                var newUser = new User
                {
                    Username = name ?? email.Split('@')[0],
                    FullName = name,
                    Email = email,
                    Role = "User",
                    Password = null // ✅ Google users don't need a password
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                existingUser = newUser;
            }

            // ✅ Log the user in
            HttpContext.Session.SetString("Username", existingUser.Username);
            HttpContext.Session.SetString("Role", existingUser.Role);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var props = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(props, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync();
            // or use HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme)
            // … map Google claims (email, name) to your User model, auto-create if new, sign in, etc.
            return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
        }
    }
}
