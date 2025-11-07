using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Controllers
{
    public class ClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassesController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
