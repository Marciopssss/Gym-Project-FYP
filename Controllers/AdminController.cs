using Gym_Membership.Data;
using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Dashboard Page
        public IActionResult Dashboard()
        {
            // Simple statistics (optional)
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalMembers = _context.Customers.Count();
            ViewBag.TotalStaff = _context.Staff.Count();
            ViewBag.TotalClasses = _context.Classes.Count();

            return View();
        }
    }
}
