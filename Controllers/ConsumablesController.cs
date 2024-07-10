using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using test_indentity.Data;
using test_indentity.Models;
using System.Globalization;
namespace test_indentity.Controllers
{
    [Authorize]
    public class ConsumablesController : Controller
    {
        private readonly AppDbContext _context;

        public ConsumablesController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin, Engineer, Director")]
        public async Task<IActionResult> Index()
        {
            var consumables = _context.Consumables.ToList();
            return View(consumables);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Engineer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Consumable consumable)
        {
            if (ModelState.IsValid)
            {
                _context.Consumables.Add(consumable);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(consumable);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Engineer")]
        public async Task<IActionResult> Edit(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null)
            {
                return NotFound();
            }
            return View(consumable);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Consumable consumable)
        {
            if (ModelState.IsValid)
            {
                _context.Consumables.Update(consumable);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(consumable);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Engineer")]
        public async Task<IActionResult> Delete(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null)
            {
                return NotFound();
            }
            _context.Consumables.Remove(consumable);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null)
            {
                return NotFound();
            }

            consumable.Quantity += 1;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var consumable = await _context.Consumables.FindAsync(id);
            if (consumable == null)
            {
                return NotFound();
            }

            if (consumable.Quantity > 0)
            {
                consumable.Quantity -= 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var consumables = await _context.Consumables.ToListAsync();
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Consumables");
                worksheet.Cells["A1"].Value = "Наименование";
                worksheet.Cells["B1"].Value = "Количество";
                worksheet.Cells["C1"].Value = "Описание";

                worksheet.Column(1).Width = 30; 
                worksheet.Column(2).Width = 15;
                worksheet.Column(3).Width = 50;

                var row = 2;
                foreach (var consumable in consumables)
                {
                    worksheet.Cells["A1:C1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1].Value = consumable.Name;
                    worksheet.Cells[row, 2].Value = consumable.Quantity;
                    worksheet.Cells[row, 3].Value = consumable.Description;
                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                var content = stream.ToArray();
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var monthName = startDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU"));
                var fileName = $"Отчет по расходным материалам за {monthName}.xlsx";

                return File(content, contentType, fileName);
            }
        }
    }
}
