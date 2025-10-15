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
            var classes = await _context.Classes
                .Include(c => c.Customers)
                .ToListAsync();

            return View(classes);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Classes newClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(newClass);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Class added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If it reaches here, model validation failed
            return View(newClass);
        }


        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var classItem = await _context.Classes.FindAsync(id);
            if (classItem == null) return NotFound();

            return View(classItem);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Classes updatedClass)
        {
            if (id != updatedClass.ClassID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedClass);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Class updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Classes.Any(c => c.ClassID == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(updatedClass);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var classItem = await _context.Classes.FirstOrDefaultAsync(c => c.ClassID == id);
            if (classItem == null) return NotFound();

            return View(classItem);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classItem = await _context.Classes.FindAsync(id);
            if (classItem != null)
            {
                _context.Classes.Remove(classItem);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Class deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
