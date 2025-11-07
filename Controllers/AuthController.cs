using Gym_Membership.Data;
using Gym_Membership.Models;
using Gym_Membership.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gym_Membership.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
        [HttpPost]
        public async Task<IActionResult> Register(User user, string verificationCode)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == user.Username))
                {
                    ViewBag.Error = "Username already exists!";
                    return View(user);
                }

                // ✅ Verify the code
                var sessionCode = HttpContext.Session.GetString("VerificationCode");
                var sessionEmail = HttpContext.Session.GetString("VerificationEmail");

                if (sessionCode == null || sessionEmail == null)
                {
                    ViewBag.Error = "Please verify your email before signing up.";
                    return View(user);
                }

                if (user.Email != sessionEmail || verificationCode != sessionCode)
                {
                    ViewBag.Error = "Invalid verification code.";
                    return View(user);
                }

                // ✅ Save user if code is valid
                user.Role = "User";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Clear session
                HttpContext.Session.Remove("VerificationCode");
                HttpContext.Session.Remove("VerificationEmail");

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

        // ---------- GOOGLE LOGIN ----------
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

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Your Google account did not return an email address. Please enable email access or use another account.";
                return RedirectToAction("Login", "Auth");
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser == null)
            {
                var newUser = new User
                {
                    Username = name ?? email.Split('@')[0],
                    FullName = name,
                    Email = email,
                    Role = "User",
                    Password = null
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                existingUser = newUser;
            }

            HttpContext.Session.SetString("Username", existingUser.Username);
            HttpContext.Session.SetString("Role", existingUser.Role);

            return RedirectToAction("Index", "Home");
        }

        // ---------- EXTERNAL LOGIN ----------
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
            return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
        }

        [HttpGet]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email is required");

            // Generate random 6-digit code
            string code = new Random().Next(100000, 999999).ToString();

            // Store it temporarily (in session)
            HttpContext.Session.SetString("VerificationCode", code);
            HttpContext.Session.SetString("VerificationEmail", email);

            // Send the code to the user's email
            await _emailService.SendEmailAsync(
                email,
                "Fitness Gym – Email Verification Code",
                $"<h2>Your verification code is:</h2><h1>{code}</h1><p>Enter this code on the registration page to complete signup.</p>"
            );

            return Ok();
        }

    }
}
