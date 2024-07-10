using Microsoft.AspNetCore.Mvc;
using test_indentity.Models;
using test_indentity.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace test_indentity.Controllers
{
    public class CabinetController : Controller
    {
        private readonly AppDbContext _context;

        public CabinetController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cabinets = await _context.CabinetLists.ToListAsync();
            return View(cabinets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,CabinetNumber,CabinetName")] CabinetList cabinet)
        {
            if (ModelState.IsValid)
            {
    
                cabinet.FullCabinetName = $"Кабинет - {cabinet.CabinetNumber} - {cabinet.CabinetName}";

                _context.Add(cabinet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cabinet);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cabinet = await _context.CabinetLists.FindAsync(id);
            if (cabinet == null)
            {
                return NotFound();
            }
            return View(cabinet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CabinetId,CabinetName,CabinetNumber")] CabinetList cabinet)
        {
            if (id != cabinet.CabinetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Заполнение FullCabinetName перед сохранением
                    cabinet.FullCabinetName = $"Кабинет - {cabinet.CabinetNumber} - {cabinet.CabinetName}";

                    _context.Update(cabinet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CabinetExists(cabinet.CabinetId))
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
            return View(cabinet);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cabinet = await _context.CabinetLists.FindAsync(id);
            if (cabinet == null)
            {
                return NotFound();
            }
            _context.CabinetLists.Remove(cabinet);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        private bool CabinetExists(int id)
        {
            return _context.CabinetLists.Any(e => e.CabinetId == id);
        }
    }
}
