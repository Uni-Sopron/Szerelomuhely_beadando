using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SzereloMuhely.Data;
using SzereloMuhely.Models;

namespace SzereloMuhely.Controllers
{
    [Authorize(Roles = "Admin,Recruiter, Mechanic")]
    public class WorkSheetsController : Controller
    {
        private readonly ServiceContext _context;
        private readonly ApplicationDbContext _identityContext;

        public WorkSheetsController(ServiceContext context, ApplicationDbContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        // GET: WorkSheets
        public async Task<IActionResult> Index(string? searchString, bool showAll = false)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.WorkSheets
                .Include(w => w.Vehicle)
                .Include(w => w.WorkProcesses).ThenInclude(wp => wp.Materials)
                .Include(w => w.WorkProcesses).ThenInclude(wp => wp.Parts)
                .AsQueryable();

            if (User.IsInRole("Mechanic"))
            {
                query = query.Where(w => w.MechanicID == currentUserId);
            }
            else if (User.IsInRole("Recuiter"))
            {
                query = query.Where(w => w.RecruiterId == currentUserId);
            }

            if (!showAll && !User.IsInRole("Admin"))
            {
                query = query.Where(w => w.IsOpen == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(w => w.Title.Contains(searchString) ||
                                         (w.Vehicle != null && w.Vehicle.LicensePlate.Contains(searchString)) ||
                                         (w.Vehicle != null && w.Vehicle.OwnerName.Contains(searchString)));
            }

            var workSheets = await query.OrderByDescending(w => w.CreatedAt).ToListAsync();

            var users = await _identityContext.Users.ToListAsync();
            foreach (var ws in workSheets)
            {
                ws.Mechanic = users.FirstOrDefault(u => u.Id == ws.MechanicID);
            }
            return View(workSheets);
        }

        // GET: WorkSheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var workSheet = await _context.WorkSheets
                .Include(w => w.Vehicle)
                .Include(w => w.WorkProcesses).ThenInclude(wp => wp.Materials)
                .Include(w => w.WorkProcesses).ThenInclude(wp => wp.Parts)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (workSheet == null) return NotFound();

            workSheet.Mechanic = await _identityContext.Users
                .FirstOrDefaultAsync(u => u.Id == workSheet.MechanicID);

            return View(workSheet);
        }

        // GET: WorkSheets/Create
        public IActionResult Create()
        {
            ViewData["MechanicID"] = new SelectList(_identityContext.Users, "Id", "UserName");
            return View();
        }

        // POST: WorkSheets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,MechanicID,RecruiterName")] WorkSheet workSheet)
        {
            workSheet.RecruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            workSheet.CreatedAt = DateTime.Now;
            workSheet.IsOpen = true;

            ModelState.Remove("Vehicle");
            ModelState.Remove("WorkProcesses");
            ModelState.Remove("Mechanic");
            ModelState.Remove("RecruiterId");

            if (ModelState.IsValid)
            {
                _context.Add(workSheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MechanicID"] = new SelectList(_identityContext.Users, "Id", "UserName");
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

            if (!workSheet.IsOpen)
            {
                return BadRequest("Lezárt munkalap nem módosítható.");
            }
            ViewData["MechanicID"] = new SelectList(_identityContext.Users, "Id", "UserName");
            return View(workSheet);
        }

        // POST: WorkSheets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,MechanicID,RecruiterName")] WorkSheet workSheet)
        {
            if (id != workSheet.ID)
            {
                return NotFound();
            }

            ModelState.Remove("Mehanic");
            ModelState.Remove("Vehicle");
            ModelState.Remove("WorkProcesses");
            ModelState.Remove("RecruiterName");

            if (ModelState.IsValid)
            {
                try
                {
                    var originalWorkSheet = await _context.WorkSheets.FindAsync(id);
                    if (originalWorkSheet == null) return NotFound();

                    originalWorkSheet.Title = workSheet.Title;
                    originalWorkSheet.MechanicID = workSheet.MechanicID;

                    _context.Update(originalWorkSheet);
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
            ViewData["MechanicID"] = new SelectList(_identityContext.Users, "Id", "UserName");
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

        // GET: WorkSheets/Close/5
        public async Task<IActionResult> Close(int? id)
        {
            if (id == null) return NotFound();

            var workSheet = await _context.WorkSheets
                .Include(w => w.Vehicle)
                .Include(w => w.WorkProcesses)
                .ThenInclude(wp => wp.Materials)
                .Include(w => w.WorkProcesses)
                .ThenInclude(wp => wp.Parts)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (workSheet == null) return NotFound();
            if (!workSheet.IsOpen) return BadRequest("A munkalap már le van zárva.");

            return View(workSheet);
        }

        // POST: WorkSheets/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id, string paymentMethod)
        {
            var workSheet = await _context.WorkSheets.FindAsync(id);
            if (workSheet == null) return NotFound();
            if (!workSheet.IsOpen) return BadRequest("A munkalap már le van zárva.");

            if (string.IsNullOrEmpty(paymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "A fizetési mód megadása kötelező.");
                return View(workSheet);
            }

            workSheet.IsOpen = false; // Closed
            workSheet.PaymentMethod = paymentMethod;
            _context.Update(workSheet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
