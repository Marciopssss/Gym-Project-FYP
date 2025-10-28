using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Payments/Checkout/5
        public async Task<IActionResult> Checkout(int id)
        {
            var membership = await _context.Memberships.FindAsync(id);
            if (membership == null) return NotFound();

            var vm = new CheckoutViewModel
            {
                MembershipID = membership.MembershipID,
                MembershipType = membership.Type,
                Price = membership.Price,
                Duration = membership != null ? membership.Duration.ToString() : "N/A"

            };

            return View(vm);
        }

        // POST: /Payments/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutInputModel input)
        {
            // Basic server-side validation
            if (!ModelState.IsValid)
            {
                // Re-populate membership info for the view
                var membership = await _context.Memberships.FindAsync(input.MembershipID);
                var vm = new CheckoutViewModel
                {
                    MembershipID = membership?.MembershipID ?? input.MembershipID,
                    MembershipType = membership?.Type ?? "Unknown",
                    Price = membership?.Price ?? input.Amount,
                    Duration = membership != null ? membership.Duration.ToString() : "N/A"

                };
                return View(vm);
            }

            // Simulate processing delay
            await Task.Delay(800);

            // Fake card checks (just for demo)
            if (!IsCardValid(input.CardNumber, input.ExpiryMonth, input.ExpiryYear, input.CVV))
            {
                ModelState.AddModelError(string.Empty, "Card validation failed. Use a 16-digit card number and valid expiry.");
                var membership = await _context.Memberships.FindAsync(input.MembershipID);
                var vm = new CheckoutViewModel
                {
                    MembershipID = membership?.MembershipID ?? input.MembershipID,
                    MembershipType = membership?.Type ?? "Unknown",
                    Price = membership?.Price ?? input.Amount,
                    Duration = membership != null ? membership.Duration.ToString() : "N/A"

                };
                return View(vm);
            }

            // At this point payment is "successful" in simulation
            // OPTIONAL: record payment in DB (uncomment and adjust the Payment model if you have one)
            /*
            var payment = new Payment
            {
                MembershipID = input.MembershipID,
                Amount = input.Amount,
                Currency = "USD",
                PaidAt = DateTime.UtcNow,
                CardLast4 = input.CardNumber?.Replace(" ", "").Trim().Substring(input.CardNumber.Length - 4)
                // add other fields as needed (UserId, TransactionId, etc.)
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            */

            TempData["PaymentSuccessMessage"] = $"Payment of ${input.Amount:F2} was successful. Thank you!";
            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            ViewBag.Message = TempData["PaymentSuccessMessage"];
            return View();
        }

        // ----- helper / validation -----
        private bool IsCardValid(string cardNumber, int expiryMonth, int expiryYear, string cvv)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Replace(" ", "").Length < 12)
                return false;

            var normalized = cardNumber.Replace(" ", "").Trim();
            if (normalized.Length < 12 || normalized.Length > 19) // allow 12-19 digits
                return false;

            if (expiryMonth < 1 || expiryMonth > 12) return false;

            // expiryYear maybe 2-digit or 4-digit; accept both
            if (expiryYear < 100) expiryYear += 2000;
            try
            {
                var lastDayOfMonth = new DateTime(expiryYear, expiryMonth, DateTime.DaysInMonth(expiryYear, expiryMonth));
                if (lastDayOfMonth < DateTime.UtcNow.Date) return false;
            }
            catch
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(cvv) || (cvv.Length != 3 && cvv.Length != 4)) return false;

            return true;
        }
    }

    // ViewModel passed to the GET view
    public class CheckoutViewModel
    {
        public int MembershipID { get; set; }
        public string MembershipType { get; set; } = "";
        public decimal Price { get; set; }
        public string Duration { get; set; } = "";

    }

    // Input model bound from the form
    public class CheckoutInputModel
    {
        [Required]
        public int MembershipID { get; set; }

        [Required]
        public decimal Amount { get; set; }

        // Card details
        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; } = "";

        [Required]
        [Range(1, 12)]
        [Display(Name = "Expiry Month")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Display(Name = "Expiry Year")]
        public int ExpiryYear { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; } = "";

        // Optional customer info
        [EmailAddress]
        [Display(Name = "Email (optional)")]
        public string? Email { get; set; }
    }
}
