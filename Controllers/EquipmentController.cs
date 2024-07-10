using Microsoft.AspNetCore.Mvc;
using test_indentity.Models;
using test_indentity.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace test_indentity.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly AppDbContext _context;

        public EquipmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Engineer, Director")]
        public async Task<IActionResult> Index(string name, string inventoryNumber, string location)
        {
            ViewData["NameFilter"] = name;
            ViewData["InventoryNumberFilter"] = inventoryNumber;
            ViewData["LocationFilter"] = location;

            var equipments = from e in _context.Equipments
                             select e;

            if (!string.IsNullOrEmpty(name))
            {
                equipments = equipments.Where(s => s.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(inventoryNumber))
            {
                equipments = equipments.Where(s => s.InventoryNumber.Contains(inventoryNumber));
            }

            if (!string.IsNullOrEmpty(location))
            {
                equipments = equipments.Where(s => s.Location.Contains(location));
            }

            return View(await equipments.ToListAsync());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Engineer")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Cabinets = await _context.CabinetLists.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,InventoryNumber,Location,Description")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cabinets = await _context.CabinetLists.ToListAsync();
            return View(equipment);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Engineer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            ViewBag.Cabinets = await _context.CabinetLists.ToListAsync();
            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,InventoryNumber,Location,Description")] Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.Id))
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
            ViewBag.Cabinets = await _context.CabinetLists.ToListAsync();
            return View(equipment);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipments.Any(e => e.Id == id);
        }
    }
}