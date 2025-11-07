using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace Gym_Membership.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public SubscriptionController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: /Subscription/Subscribe
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Subscribe(int membershipId)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login", "User");

            // ✅ Check if user already has an active subscription
            var activeSub = _context.Subscriptions
                .Include(s => s.Membership)
                .Where(s => s.UserEmail == user.Email && s.IsActive && s.EndDate > DateTime.UtcNow)
                .FirstOrDefault();

            if (activeSub != null)
            {
                TempData["Error"] = $"You already have an active plan ({activeSub.Membership.Type}) that expires on {activeSub.EndDate:MMMM dd, yyyy}. Please wait until it expires before subscribing again.";
                return RedirectToAction("Plans", "Membership");
            }

            // ✅ Continue normally (Stripe checkout)
            var membership = await _context.Memberships.FindAsync(membershipId);
            if (membership == null)
                return NotFound();

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

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
                    UnitAmountDecimal = membership.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = membership.Type,
                        Description = membership.description
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = $"{domain}/Subscription/Success?membershipId={membershipId}",
                CancelUrl = $"{domain}/Subscription/Cancel"
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }


        public IActionResult Success(int membershipId)
        {
            var membership = _context.Memberships.FirstOrDefault(m => m.MembershipID == membershipId);
            if (membership == null)
                return NotFound();

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "User");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login", "User");

            // ✅ Find customer linked to this user
            var customer = _context.Customers.FirstOrDefault(c => c.Email == user.Email);
            if (customer == null)
            {
                // If no customer exists for this user, create one
                customer = new Gym_Membership.Models.Customer
                {
                    Name = user.FullName ?? user.Username,
                    Email = user.Email,
                    Phone = user.Phone ?? "N/A",
                    UserId = user.UserId // ✅ Link this customer to the logged-in user
                };


                _context.Customers.Add(customer);
                _context.SaveChanges();
            }

            // ✅ Create new subscription linked to customer + membership
            var sub = new Gym_Membership.Models.Subscription
            {
                MembershipID = membership.MembershipID,
                CustomerID = customer.CustomerID, // <— FK is now valid
                UserEmail = user.Email,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(membership.Duration),
                IsActive = true
            };

            _context.Subscriptions.Add(sub);
            _context.SaveChanges();

            ViewBag.Message = $"✅ You are now subscribed to the {membership.Type} plan until {sub.EndDate:MMMM dd, yyyy}.";
            return View();
        }



        public IActionResult Cancel()
        {
            return View();
        }
    }
}
