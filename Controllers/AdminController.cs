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

            if (!expiredMembers.Any())
            {
                ViewBag.Message = "No expired members found.";
            }

            return View(expiredMembers);
        }

        // ✅ Active / Inactive Members Page
        public async Task<IActionResult> ActiveInactiveMembers()
        {
            var members = await _context.Customers
                .Include(c => c.Membership)
                .ToListAsync();

            ViewBag.ActiveMembers = members.Where(m => m.IsActive).ToList();
            ViewBag.InactiveMembers = members.Where(m => !m.IsActive).ToList();

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
