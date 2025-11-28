using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class TrainerApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TrainerApplicationController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Apply() => View();

        [HttpPost]
        public async Task<IActionResult> Apply(TrainerApplication model, IFormFile? CertificateFile)
        {
            if (ModelState.IsValid)
            {
                if (CertificateFile != null)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + CertificateFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CertificateFile.CopyToAsync(stream);
                    }

                    model.CertificatePath = "/uploads/" + uniqueFileName;
                }

                model.Status = "Pending";
                _context.TrainerApplications.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Your trainer application was submitted successfully!";
                return RedirectToAction("Apply");
            }

            return View(model);
        }

        // ADMIN VIEW
        [HttpGet]
        public IActionResult AdminView()
        {
            var apps = _context.TrainerApplications.OrderByDescending(a => a.SubmittedAt).ToList();
            return View(apps);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            // 1) Load application
            var application = await _context.TrainerApplications
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            // 2) Mark application as approved
            application.Status = "Approved";

            // 3) Find user using info from the application
            //    👉 If your application stores Username instead of Email,
            //    change Email to Username below.
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == application.Email);

            if (user != null)
            {
                // Promote to trainer if you want
                user.Role = "Trainer";

                // Set one-time notification message
                user.TrainerNotification =
                    "Congratulations! Your application to become a trainer has been approved. " +
                    "You are now a trainer in the gym.";
            }

            await _context.SaveChangesAsync();

            TempData["AdminMessage"] = "Application approved. The user will see a confirmation message on next login.";
            return RedirectToAction("TrainerApplications");
        }




        [HttpPost]
        public IActionResult Reject(int id)
        {
            var app = _context.TrainerApplications.Find(id);
            if (app != null)
            {
                app.Status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("AdminView");
        }
    }
}
