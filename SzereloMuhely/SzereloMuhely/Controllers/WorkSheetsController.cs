using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SzereloMuhely.Data;
using SzereloMuhely.Models;

namespace SzereloMuhely.Controllers
{
    public class WorkSheetsController : Controller
    {
        private readonly ServiceContext _context;

        public WorkSheetsController(ServiceContext context)
        {
            _context = context;
        }

        // GET: WorkSheets
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkSheets.ToListAsync());
        }

        // GET: WorkSheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSheet = await _context.WorkSheets
                .FirstOrDefaultAsync(m => m.ID == id);
            if (workSheet == null)
            {
                return NotFound();
            }

            return View(workSheet);
        }

        // GET: WorkSheets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkSheets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,MechanicID,Status,PaymentMethod")] WorkSheet workSheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workSheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workSheet);
        }

        // GET: WorkSheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSheet = await _context.WorkSheets.FindAsync(id);
            if (workSheet == null)
            {
                return NotFound();
            }
            return View(workSheet);
        }

        // POST: WorkSheets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,MechanicID,Status,PaymentMethod")] WorkSheet workSheet)
        {
            if (id != workSheet.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkSheetExists(workSheet.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workSheet);
        }

        // GET: WorkSheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSheet = await _context.WorkSheets
                .FirstOrDefaultAsync(m => m.ID == id);
            if (workSheet == null)
            {
                return NotFound();
            }

            return View(workSheet);
        }

        // POST: WorkSheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workSheet = await _context.WorkSheets.FindAsync(id);
            if (workSheet != null)
            {
                _context.WorkSheets.Remove(workSheet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkSheetExists(int id)
        {
            return _context.WorkSheets.Any(e => e.ID == id);
        }
    }
}
