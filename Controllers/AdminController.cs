using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Gym_Membership.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalMembers = _context.Customers.Count();
            ViewBag.TotalStaff = _context.Staff.Count();
            ViewBag.TotalClasses = _context.Classes.Count();

            return View();
        }
        public async Task<IActionResult> ExpiredMembers()
        {
            var today = DateTime.Now;

            var expiredMembers = _context.Customers
                .Include(c => c.Membership)
                .AsEnumerable() 
                .Where(c => c.Membership.ExpiryDate < today)
                .Select(c => new ExpiredMemberViewModel
                {
                    MemberID = c.CustomerID,
                    Name = c.Name,
                    PlanName = c.Membership.Type,
                    ExpiryDate = c.Membership.ExpiryDate
                })
                .ToList();

            return View();
        }


    }
}
