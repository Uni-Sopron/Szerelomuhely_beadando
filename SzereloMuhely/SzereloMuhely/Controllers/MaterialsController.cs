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
    public class MaterialsController : Controller
    {
        private readonly ServiceContext _context;

        public MaterialsController(ServiceContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index(string? searchString, bool showAll = false, int page = 1)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Materials
                .Include(p => p.WorkProcess)
                    .ThenInclude(wp => wp.WorkSheet)
                        .ThenInclude(ws => ws.Vehicle)
                .AsQueryable();

            if (User.IsInRole("Mechanic"))
            {
                query = query.Where(m => m.WorkProcess.WorkSheet.MechanicID == currentUserId);
            }

            if (!showAll)
            {
                query = query.Where(m => m.WorkProcess.WorkSheet.IsOpen == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => m.Name.Contains(searchString) ||
                                         (m.WorkProcess != null && (
                                            m.WorkProcess.Name.Contains(searchString) ||
                                            (m.WorkProcess.WorkSheet != null && (
                                                m.WorkProcess.WorkSheet.Title.Contains(searchString) ||
                                                (m.WorkProcess.WorkSheet.Vehicle != null && m.WorkProcess.WorkSheet.Vehicle.LicensePlate.Contains(searchString))
                                            ))
                                         )));
            }

            int pageSize = 10;
            int totalCount = await query.CountAsync();

            var materials = await query
                .OrderByDescending(m => m.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["AktualisKereses"] = searchString;
            ViewData["ShowAll"] = showAll;

            return View(materials);
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.WorkProcess)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Materials/Create
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

        // POST: Materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Quantity,Price,WorkProcessID")] Material material)
        {
            ModelState.Remove("WorkProcess");

            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", material.WorkProcessID);
            return View(material);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", material.WorkProcessID);
            return View(material);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Quantity,Price,WorkProcessID")] Material material)
        {
            if (id != material.ID)
            {
                return NotFound();
            }

            ModelState.Remove("WorkProcess");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.ID))
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
            ViewData["WorkProcessID"] = new SelectList(_context.WorkProcesses, "ID", "Name", material.WorkProcessID);
            return View(material);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.WorkProcess)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.ID == id);
        }
    }
}
