using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Gym_Membership.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Checkout(decimal amount, string membershipType)
        {
            var domain = "https://localhost:5200"; // your base URL

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmountDecimal = amount * 100, // Stripe uses cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{membershipType} Plan Subscription"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Payment/Success",
                CancelUrl = domain + "/Payment/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        
        public IActionResult Success()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Gym Membership", "yourEmail@gmail.com"));
            message.To.Add(new MailboxAddress("Customer", "customer@gmail.com"));
            message.Subject = "Payment Confirmation";
            message.Body = new TextPart("plain")
            {
                Text = "Your subscription has been activated. Thank you!"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("yourEmail@gmail.com", "yourAppPassword"); // Use App Password
                client.Send(message);
                client.Disconnect(true);
            }

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

    }
}
