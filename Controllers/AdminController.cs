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

        // ✅ Dashboard (stats)
        public IActionResult Dashboard()
        {
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalMembers = _context.Customers.Count();
            ViewBag.TotalStaff = _context.Staff.Count();
            ViewBag.TotalClasses = _context.Classes.Count();

            return View();
        }

        // ✅ List all customers (users)
        public async Task<IActionResult> ManageCustomers()
        {
            var customers = await _context.Customers
                .Include(c => c.Membership)
                .ToListAsync();

            return View(customers);
        }

        // ✅ Create (Add new customer)
        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Customer added successfully!";
                return RedirectToAction(nameof(ManageCustomers));
            }

            return View(customer);
        }

        // ✅ Edit (Update customer info)
        [HttpGet]
        public async Task<IActionResult> EditCustomer(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, Customer updatedCustomer)
        {
            if (id != updatedCustomer.CustomerID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedCustomer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Customer updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(c => c.CustomerID == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(ManageCustomers));
            }

            return View(updatedCustomer);
        }

        // ✅ Delete
        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerID == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("DeleteCustomer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Customer deleted successfully!";
            }

            return RedirectToAction(nameof(ManageCustomers));
        }

        // ✅ Expired Members Report
        // ✅ Expired Members Report (With Reminder System)
        public async Task<IActionResult> ExpiredMembers()
        {
            var today = DateTime.UtcNow;

            // Get all expired memberships
            var expiredMembers = await _context.Subscriptions
                .Include(s => s.Customer)
                .Include(s => s.Membership)
                .Where(s => s.EndDate < today)
                .Select(s => new
                {
                    s.SubscriptionID,
                    s.Customer.CustomerID,
                    s.Customer.UserId,
                    s.Customer.Name,
                    s.Customer.Email,
                    Plan = s.Membership.Type,
                    ExpiryDate = s.EndDate,
                    DaysExpired = EF.Functions.DateDiffDay(s.EndDate, today)
                })
                .ToListAsync();

            // No expired members
            if (!expiredMembers.Any())
            {
                ViewBag.Message = "🎉 Great news! No expired memberships found!";
                return View(new List<object>());
            }

            return View(expiredMembers);
        }

        [HttpPost]
        public async Task<IActionResult> SendReminder(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Create message for user's profile
            var message = $"Hi {user.Username}, your gym membership has expired. Please renew it to continue accessing your classes.";

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                DateSent = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Reminder sent to {user.Username}'s profile successfully!";
            return RedirectToAction(nameof(ExpiredMembers));
        }


        // ✅ Active / Inactive Members Page
        public async Task<IActionResult> ActiveInactiveMembers()
        {
            var now = DateTime.UtcNow;

            var users = await _context.Users
                .Include(u => u.Customer)
                    .ThenInclude(c => c.Subscriptions)
                        .ThenInclude(s => s.Membership)
                .Include(u => u.Customer)
                    .ThenInclude(c => c.Classes)
                .ToListAsync();

            var activeMembers = users
                .Where(u =>
                    (u.Customer != null &&
                     (
                         (u.Customer.Subscriptions.Any(s => s.IsActive && s.EndDate > now)) ||
                         (u.Customer.Classes.Any())
                     ))
                )
                .Select(u => new
                {
                    u.UserId,
                    Name = u.Username,
                    Email = u.Email,
                    Plan = u.Customer?.Subscriptions
                                .OrderByDescending(s => s.StartDate)
                                .FirstOrDefault(s => s.IsActive && s.EndDate > now)?.Membership?.Type ?? "—",
                    Expiry = u.Customer?.Subscriptions
                                .OrderByDescending(s => s.StartDate)
                                .FirstOrDefault(s => s.IsActive && s.EndDate > now)?.EndDate,
                    Classes = u.Customer?.Classes != null && u.Customer.Classes.Any()
                                ? string.Join(", ", u.Customer.Classes.Select(c => c.ClassName))
                                : "No classes"
                })
                .ToList();

            var inactiveMembers = users
                .Where(u =>
                    u.Customer == null ||
                    (
                        (!u.Customer.Subscriptions.Any(s => s.IsActive && s.EndDate > now)) &&
                        (!u.Customer.Classes.Any())
                    )
                )
                .Select(u => new
                {
                    u.UserId,
                    Name = u.Username,
                    Email = u.Email,
                    Plan = "—",
                    Expiry = (DateTime?)null,
                    Classes = "—"
                })
                .ToList();

            ViewBag.ActiveMembers = activeMembers;
            ViewBag.InactiveMembers = inactiveMembers;

            return View();
        }


        // ✅ Activate a member
        [HttpPost]
        public async Task<IActionResult> ActivateMember(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.IsActive = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{customer.Name} has been activated.";
            return RedirectToAction(nameof(ActiveInactiveMembers));
        }

        // ✅ Deactivate a member
        [HttpPost]
        public async Task<IActionResult> DeactivateMember(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{customer.Name} has been deactivated.";
            return RedirectToAction(nameof(ActiveInactiveMembers));
        }

      

    }
}
