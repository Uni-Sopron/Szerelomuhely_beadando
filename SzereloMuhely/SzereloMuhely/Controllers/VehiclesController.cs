using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SzereloMuhely.Data;
using SzereloMuhely.Models;
using System.Security.Claims;

namespace SzereloMuhely.Controllers
{
    [Authorize(Roles = "Admin,Recruiter")]
    public class VehiclesController : Controller
    {
        private readonly ServiceContext _context;

        public VehiclesController(ServiceContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index(string? searchString, bool showAll = false, int page = 1)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Vehicles.Include(v => v.WorkSheet).AsQueryable();

            if (!showAll)
            {
                query = query.Where(v => v.WorkSheet.IsOpen == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(v => v.LicensePlate.Contains(searchString) ||
                                         v.OwnerName.Contains(searchString) ||
                                         v.Make.Contains(searchString) ||
                                         v.Model.Contains(searchString) ||
                                         (v.WorkSheet != null && v.WorkSheet.Title.Contains(searchString)));
            }

            int pageSize = 10;
            int totalCount = await query.CountAsync();

            var vehicles = await query
                .OrderByDescending(v => v.ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewData["AktualisKereses"] = searchString;
            ViewData["ShowAll"] = showAll;

            return View(vehicles);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.WorkSheet)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            var assignedWorkSheetIds = _context.Vehicles
                .Select(v => v.WorkSheetID)
                .ToList();

            var query = _context.WorkSheets
                .Where(ws => !assignedWorkSheetIds.Contains(ws.ID));

            if (!User.IsInRole("Admin"))
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                query = query.Where(ws => ws.RecruiterId == currentUserId);
            }

            var freeWorkSheets = query.ToList();

            ViewData["WorkSheetID"] = new SelectList(freeWorkSheets, "ID", "Title");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LicensePlate,Make,Model,OwnerName,OwnerAddress,WorkSheetID")] Vehicle vehicle)
        {
            ModelState.Remove("WorkSheet");
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", vehicle.WorkSheetID);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", vehicle.WorkSheetID);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LicensePlate,Make,Model,OwnerName,OwnerAddress,WorkSheetID")] Vehicle vehicle)
        {
            if (id != vehicle.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.ID))
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
            ViewData["WorkSheetID"] = new SelectList(_context.WorkSheets, "ID", "Title", vehicle.WorkSheetID);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.WorkSheet)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.ID == id);
        }
    }
}
