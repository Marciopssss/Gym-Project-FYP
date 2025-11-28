using Gym_Membership.Data;
using Gym_Membership.Models;
using Gym_Membership.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Trainer/Profile
        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != "Trainer")
                return RedirectToAction("Login", "Auth");

            // 🔹 Current trainer user
            var trainerUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Role == "Trainer");

            if (trainerUser == null)
                return Unauthorized();

            // 🔹 All trainers in the gym
            var allTrainers = await _context.Users
                .Where(u => u.Role == "Trainer")
                .ToListAsync();

            // 🔹 All clients where THIS trainer is the personal trainer
            var myClients = await _context.Customers
                .Include(c => c.PersonalTrainer)
                .Where(c => c.PersonalTrainerId == trainerUser.UserId)
                .ToListAsync();

            var vm = new TrainerProfileViewModel
            {
                CurrentTrainer = trainerUser,
                AllTrainers = allTrainers,
                MyClients = myClients
            };

            return View(vm);
        }

        public async Task<IActionResult> Browse()
        {
            // Only users that are trainers
            var trainers = await _context.Users
                .Where(u => u.Role == "Trainer" || u.IsTrainer)
                .Select(u => new TrainerCardViewModel
                {
                    TrainerUserId = u.UserId,
                    Name = u.FullName ?? u.Username,
                    Price = u.TrainerPrice,
                    Schedule = u.TrainerSchedule
                })
                .ToListAsync();

            return View(trainers);
        }

        // ✅ When a user chooses a trainer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choose(int trainerUserId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Please log in first.";
                return RedirectToAction("Login", "Auth");
            }

            // Find the logged-in user
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Login", "Auth");
            }

            // Find the Customer row linked to this User
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.UserId);

            if (customer == null)
            {
                TempData["Error"] = "Customer profile not found.";
                return RedirectToAction("Profile", "User");
            }

            // Assign the personal trainer
            customer.PersonalTrainerId = trainerUserId;

            // (optional) you can set some default time – later you can make a proper form
            // Example: store "now + 1 day" as first session time
            customer.PersonalTrainingTime = DateTime.Now.AddDays(1);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Personal trainer selected successfully!";
            return RedirectToAction("Browse");
        }
    }
}
