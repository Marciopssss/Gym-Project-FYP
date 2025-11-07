using Gym_Membership.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gym_Membership.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;

        public ContactController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name, string email, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                ViewBag.Error = "Please fill out all fields.";
                return View();
            }

            string subject = $"📩 New Message from {name}";
            string body = $@"
                <h3>New Contact Form Message</h3>
                <p><strong>Name:</strong> {name}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Message:</strong><br>{message}</p>
            ";

            await _emailService.SendEmailAsync("FitnessGymfyp@gmail.com", subject, body);
            ViewBag.Success = "✅ Your message has been sent successfully!";

            return View();
        }
    }
}
