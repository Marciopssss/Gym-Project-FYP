using Microsoft.AspNetCore.Mvc;
using Gym_Membership.Data;
using Gym_Membership.Models;
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
        public async Task<IActionResult> Subscribe(int membershipId)
        {
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
                            UnitAmountDecimal = membership.Price * 100, // Stripe works in cents
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
            ViewBag.MembershipId = membershipId;
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
