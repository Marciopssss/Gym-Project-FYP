using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Gym_Membership.Services;


namespace Gym_Membership.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context , EmailService emailService)
        {
            _logger = logger;
            _context = context;
           _emailService = emailService;

        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    var activeSub = _context.Subscriptions
                        .Include(s => s.Membership)
                        .Where(s => s.UserEmail == user.Email && s.IsActive && s.EndDate > DateTime.UtcNow)
                        .OrderByDescending(s => s.StartDate)
                        .FirstOrDefault();

                    ViewBag.ActiveSubscription = activeSub;
                }
            }
            // Auto-expire old subscriptions
            var expired = _context.Subscriptions
                .Where(s => s.IsActive && s.EndDate <= DateTime.UtcNow)
                .ToList();

            if (expired.Any())
            {
                foreach (var s in expired) s.IsActive = false;
                _context.SaveChanges();
            }


            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string Name, string Email, string Message)
        {
            string subject = $"📩 New Message from {Name}";
            string body = $@"
                <h3>New Contact Message</h3>
                <p><b>Name:</b> {Name}</p>
                <p><b>Email:</b> {Email}</p>
                <p><b>Message:</b><br>{Message}</p>";

            await _emailService.SendEmailAsync("FitnessGymfyp@gmail.com", subject, body);
            TempData["MessageSuccess"] = "✅ Message sent successfully!";
            return RedirectToAction("Index");
        }

    }
}
