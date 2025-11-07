using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Gym_Membership.Controllers
{
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;  // ✅ Add this line


        public ClassesController(ApplicationDbContext context , IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes.AsNoTracking().ToListAsync();
            return View(classes);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            return View(new Classes { ScheduleTime = DateTime.Now });
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Classes model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the validation errors.";
                return View(model);
            }

            _context.Classes.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Class added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.Classes.FindAsync(id.Value);
            if (entity == null) return NotFound();
            return View(entity);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Classes model)
        {
            if (id != model.ClassID) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix the validation errors.";
                return View(model);
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Classes.AnyAsync(c => c.ClassID == id))
                    return NotFound();
                throw;
            }
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var entity = await _context.Classes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClassID == id.Value);
            if (entity == null) return NotFound();
            return View(entity);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Classes.FindAsync(id);
            if (entity != null)
            {
                _context.Classes.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ✅ USER SIDE - Browse Classes
        public IActionResult Browse()
        {
            // Simple fetch (TrainerName is just a string, no navigation)
            var classes = _context.Classes.ToList();
            return View(classes);
        }

        // ✅ Subscribe to a class
        [HttpPost]
        public IActionResult Subscribe(int classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "You must log in to subscribe to a class.";
                return RedirectToAction("Browse");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Browse");
            }

            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassID == classId);
            if (classEntity == null)
            {
                TempData["Error"] = "Class not found.";
                return RedirectToAction("Browse");
            }

            // ✅ Prevent duplicate subscription
            var alreadyEnrolled = _context.CustomerClasses
                .Any(cc => cc.ClassID == classId && cc.UserID == user.UserId);

            if (alreadyEnrolled)
            {
                TempData["Error"] = $"You are already subscribed to {classEntity.ClassName}.";
                return RedirectToAction("Browse");
            }

            // ✅ Add new record in join table
            var customerClass = new CustomerClass
            {
                ClassID = classId,
                UserID = user.UserId,
                SubscriptionDate = DateTime.Now
            };

            _context.CustomerClasses.Add(customerClass);
            _context.SaveChanges();

            TempData["Success"] = $"You have successfully subscribed to {classEntity.ClassName}!";
            return RedirectToAction("Browse");
        }
        [HttpPost]
        public async Task<IActionResult> SubscribeWithStripe(int classId)
        {
            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassID == classId);
            if (classEntity == null)
                return NotFound();

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login", "User");

            // ✅ Use Stripe API key from appsettings.json
            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = classEntity.PricePerSession * 100, // Stripe uses cents
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = classEntity.ClassName,
                        Description = classEntity.Description
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/Classes/Success?classId={classId}",
                CancelUrl = $"{domain}/Classes/Cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }
        public IActionResult Success(int classId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login", "User");

            var classEntity = _context.Classes.FirstOrDefault(c => c.ClassID == classId);
            if (classEntity == null)
                return NotFound();

            // ✅ Prevent duplicate enrollment
            var alreadyEnrolled = _context.CustomerClasses
                .Any(cc => cc.ClassID == classId && cc.UserID == user.UserId);

            if (!alreadyEnrolled)
            {
                var customerClass = new CustomerClass
                {
                    ClassID = classId,
                    UserID = user.UserId,
                    SubscriptionDate = DateTime.Now
                };
                _context.CustomerClasses.Add(customerClass);
                _context.SaveChanges();
            }

            ViewBag.Message = $"✅ Payment successful! You are now enrolled in {classEntity.ClassName}.";
            return View();
        }

        public IActionResult Cancel()
        {
            ViewBag.Message = "❌ Payment was canceled.";
            return View();
        }



    }
}
