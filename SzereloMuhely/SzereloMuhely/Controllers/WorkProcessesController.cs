using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SzereloMuhely.Data;
using SzereloMuhely.Models;

namespace SzereloMuhely.Controllers
{
    [Authorize(Roles = "Admin,Mechanic")]
    public class WorkProcessesController : Controller
    {
        private readonly ServiceContext _context;

        public WorkProcessesController(ServiceContext context)
        {
            _context = context;
        }

        // GET: WorkProcesses
        public async Task<IActionResult> Index(string? searchString, bool showAll = false, int page = 1)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.WorkProcesses
                .Include(wp => wp.WorkSheet)
                .ThenInclude(ws => ws.Vehicle)
                .AsQueryable();

            if (User.IsInRole("Mechanic"))
            {
                query = query.Where(wp => wp.WorkSheet.MechanicID == currentUserId);
            }

            if (!showAll)
            {
                query = query.Where(wp => wp.WorkSheet.IsOpen == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(wp => wp.Name.Contains(searchString) ||
                                         (wp.WorkSheet != null && (
                                            wp.WorkSheet.Title.Contains(searchString) ||
                                            (wp.WorkSheet.Vehicle != null && wp.WorkSheet.Vehicle.LicensePlate.Contains(searchString))
                                         )));
            }

            int pageSize = 10;
            int totalCount = await query.CountAsync();

            var workProcesses = await query
                .OrderByDescending(wp => wp.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["AktualisKereses"] = searchString;
            ViewData["ShowAll"] = showAll;

            return View(workProcesses);
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
            var query = _context.WorkSheets.Where(ws => ws.IsOpen).AsQueryable();
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = query.Where(ws => ws.MechanicID == currentUserId);

            }
            var assignedWorkSheets = query.ToList();

            ViewData["WorkSheetID"] = new SelectList(assignedWorkSheets, "ID", "Title");
            return View();
        }

        // POST: WorkProcesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Duration,Price,WorkSheetID")] WorkProcess workProcess)
        {
            ModelState.Remove("WorkSheet");
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Duration,Price,WorkSheetID")] WorkProcess workProcess)
        {
            if (id != workProcess.ID)
            {
                return NotFound();
            }

            ModelState.Remove("WorkSheet");

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
            var workProcess = await _context.WorkProcesses
                .Include(wp => wp.Materials)
                .Include(wp => wp.Parts)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (workProcess != null)
            {
                _context.WorkProcesses.Remove(workProcess);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WorkProcessExists(int id)
        {
            return _context.WorkProcesses.Any(e => e.ID == id);
        }
    }
}
