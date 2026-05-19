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
    public class PartsController : Controller
    {
        private readonly ServiceContext _context;

        public PartsController(ServiceContext context)
        {
            _context = context;
        }

        // GET: Parts
        public async Task<IActionResult> Index(string? searchString, bool showAll = false, int page = 1)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Parts
                .Include(p => p.WorkProcess)
                    .ThenInclude(wp => wp.WorkSheet)
                        .ThenInclude(ws => ws.Vehicle)
                .AsQueryable();

            if (User.IsInRole("Mechanic"))
            {
                query = query.Where(p => p.WorkProcess.WorkSheet.MechanicID == currentUserId);
            }

            if (!showAll)
            {
                query = query.Where(p => p.WorkProcess.WorkSheet.IsOpen == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString) ||
                                         (p.WorkProcess != null && (
                                            p.WorkProcess.Name.Contains(searchString) ||
                                            (p.WorkProcess.WorkSheet != null && (
                                                p.WorkProcess.WorkSheet.Title.Contains(searchString) ||
                                                (p.WorkProcess.WorkSheet.Vehicle != null && p.WorkProcess.WorkSheet.Vehicle.LicensePlate.Contains(searchString))
                                            ))
                                         )));
            }

            int pageSize = 10;
            int totalCount = await query.CountAsync();

            var parts = await query
                .OrderByDescending(p => p.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["AktualisKereses"] = searchString;
            ViewData["ShowAll"] = showAll;

            return View(parts);
        }

        // GET: Parts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .Include(p => p.WorkProcess)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // GET: Parts/Create
        public IActionResult Create()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myWorkProcesses = _context.WorkProcesses
                .Include(wp => wp.WorkSheet)
                .Where(wp => wp.WorkSheet.IsOpen && (User.IsInRole("Admin") || wp.WorkSheet.MechanicID == currentUserId))
                .Select(wp => new {
                    ID = wp.ID,
                    DisplayName = $"{wp.WorkSheet.Title} - {wp.Name}"
                })
                .ToList();

            ViewData["WorkProcessID"] = new SelectList(myWorkProcesses, "ID", "DisplayName");
            return View();
        }

        // POST: Parts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Quantity,Price,WorkProcessID")] Part part)
        {
            ModelState.Remove("WorkProcess");
            if (ModelState.IsValid)
            {
                _context.Add(part);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", part.WorkProcessID);
            return View(part);
        }

        // GET: Parts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", part.WorkProcessID);
            return View(part);
        }

        // POST: Parts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Quantity,Price,WorkProcessID")] Part part)
        {
            if (id != part.ID)
            {
                return NotFound();
            }

            ModelState.Remove("WorkProcess");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(part);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartExists(part.ID))
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
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", part.WorkProcessID);
            return View(part);
        }

        // GET: Parts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .Include(p => p.WorkProcess)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartExists(int id)
        {
            return _context.Parts.Any(e => e.ID == id);
        }
    }
}
