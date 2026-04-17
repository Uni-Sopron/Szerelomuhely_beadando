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
    public class WorkProcessesController : Controller
    {
        private readonly ServiceContext _context;

        public WorkProcessesController(ServiceContext context)
        {
            _context = context;
        }

        // GET: WorkProcesses
        public async Task<IActionResult> Index()
        {
            var serviceContext = _context.WorkProcesses.Include(w => w.WorkSheet);
            return View(await serviceContext.ToListAsync());
        }

        // GET: WorkProcesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workProcess = await _context.WorkProcesses
                .Include(w => w.WorkSheet)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (workProcess == null)
            {
                return NotFound();
            }

            return View(workProcess);
        }

        // GET: WorkProcesses/Create
        public IActionResult Create()
        {
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title");
            return View();
        }

        // POST: WorkProcesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Duration,WorkSheetID")] WorkProcess workProcess)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workProcess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", workProcess.WorkSheetID);
            return View(workProcess);
        }

        // GET: WorkProcesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workProcess = await _context.WorkProcesses.FindAsync(id);
            if (workProcess == null)
            {
                return NotFound();
            }
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", workProcess.WorkSheetID);
            return View(workProcess);
        }

        // POST: WorkProcesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Duration,WorkSheetID")] WorkProcess workProcess)
        {
            if (id != workProcess.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workProcess);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkProcessExists(workProcess.ID))
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
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", workProcess.WorkSheetID);
            return View(workProcess);
        }

        // GET: WorkProcesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workProcess = await _context.WorkProcesses
                .Include(w => w.WorkSheet)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (workProcess == null)
            {
                return NotFound();
            }

            return View(workProcess);
        }

        // POST: WorkProcesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workProcess = await _context.WorkProcesses.FindAsync(id);
            if (workProcess != null)
            {
                _context.WorkProcesses.Remove(workProcess);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkProcessExists(int id)
        {
            return _context.WorkProcesses.Any(e => e.ID == id);
        }
    }
}
