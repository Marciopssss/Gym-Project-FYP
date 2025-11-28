using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class MembershipController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Membership
        public async Task<IActionResult> Index()
        {
            var memberships = await _context.Memberships.AsNoTracking().ToListAsync();
            return View(memberships);
        }
        // GET: Membership/Plans
        public async Task<IActionResult> Plans()
        {
            var plans = await _context.Memberships.AsNoTracking().ToListAsync();
            return View(plans);
        }


        // GET: Membership/Create
        public IActionResult Create()
        {
            return View(new Membership());
        }

        // POST: Membership/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Membership model)
        {
            // Default StartDate if left empty
            if (model.StartDate == DateTime.MinValue)
                model.StartDate = DateTime.Now;

            // Debug log (you can remove later)
            Console.WriteLine($"DEBUG: Type={model.Type}, Duration={model.Duration}, Price={model.Price}, StartDate={model.StartDate}");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields correctly.";
                return View(model);
            }

            try
            {
                _context.Memberships.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membership created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
                return View(model);
            }
        }

        // GET: Membership/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var membership = await _context.Memberships.FindAsync(id.Value);
            if (membership == null)
                return NotFound();

            return View(membership);
        }

        // POST: Membership/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Membership model)
        {
            if (id != model.MembershipID)
                return NotFound();

            if (model.StartDate == DateTime.MinValue)
                model.StartDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields correctly.";
                return View(model);
            }

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Membership updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Memberships.AnyAsync(m => m.MembershipID == id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Database error: " + ex.Message;
                return View(model);
            }
        }

        // GET: Membership/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var membership = await _context.Memberships
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MembershipID == id.Value);

            if (membership == null)
                return NotFound();

            return View(membership);
        }

        // POST: Membership/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subs = await _context.Subscriptions
                .Where(s => s.MembershipID == id)
                .ToListAsync();

            if (subs.Any())
                _context.Subscriptions.RemoveRange(subs);

            var membership = await _context.Memberships.FindAsync(id);
            if (membership != null)
                _context.Memberships.Remove(membership);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Membership deleted.";
            return RedirectToAction(nameof(Index));
        }


    }
}
