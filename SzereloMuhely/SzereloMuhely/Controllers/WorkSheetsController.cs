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
        public async Task<IActionResult> Index(string? searchString, bool showAll = false)
        {
            var query = _context.WorkSheets
                .Include(w => w.Vehicle)
                .Include(w => w.WorkProcesses)
                .ThenInclude(wp => wp.Materials)
                .Include(w => w.WorkProcesses)
                .ThenInclude(wp => wp.Parts)
                .AsQueryable();

            if (!showAll)
            {
                query = query.Where(w => w.Status == true);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(w => w.Title.Contains(searchString) ||
                                         w.Vehicle.LicensePlate.Contains(searchString) ||
                                         w.Vehicle.OwnerName.Contains(searchString));
            }

            return View(await query.OrderByDescending(w => w.CreatedAt).ToListAsync());
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
            ViewData["MechanicID"] = new SelectList(_context.Users, "ID", "Username");
            return View();
        }

        // POST: WorkSheets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,MechanicID,RecruiterName")] WorkSheet workSheet)
        {
            workSheet.RecruiterName = "Szabó Mari";
            workSheet.CreatedAt = DateTime.Now;
            workSheet.Status = true;

            ModelState.Remove("RecruiterName");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("Vehicle");
            ModelState.Remove("WorkProcesses");

            if (ModelState.IsValid)
            {
                _context.Add(workSheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MechanicID"] = new SelectList(_context.Users, "ID", "Username", workSheet.MechanicID);
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

            if (workSheet.IsClosed)
            {
                return BadRequest("Lezárt munkalap nem módosítható.");
            }
            ViewData["MechanicID"] = new SelectList(_context.Users, "ID", "Username", workSheet.MechanicID);
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

            var originalWorkSheet = await _context.WorkSheets.AsNoTracking().FirstOrDefaultAsync(w => w.ID == id);
            if (originalWorkSheet == null) return NotFound();
            if (!originalWorkSheet.Status) return BadRequest("Lezárt munkalap nem módosítható.");

            if (ModelState.IsValid)
            {
                try
                {
                    workSheet.Status = originalWorkSheet.Status;
                    workSheet.CreatedAt = originalWorkSheet.CreatedAt;
                    workSheet.PaymentMethod = originalWorkSheet.PaymentMethod;

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
            if (workSheet.IsClosed) return BadRequest("A munkalap már le van zárva.");

            return View(workSheet);
        }

        // POST: WorkSheets/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id, string paymentMethod)
        {
            var workSheet = await _context.WorkSheets.FindAsync(id);
            if (workSheet == null) return NotFound();
            if (workSheet.IsClosed) return BadRequest("A munkalap már le van zárva.");

            if (string.IsNullOrEmpty(paymentMethod))
            {
                ModelState.AddModelError("PaymentMethod", "A fizetési mód megadása kötelező.");
                return View(workSheet);
            }

            workSheet.Status = false; // Closed
            workSheet.PaymentMethod = paymentMethod;
            _context.Update(workSheet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
