using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using test_indentity.Data;
using test_indentity.Models;
using test_indentity.ViewModels;

namespace test_indentity.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RequestController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var requests = _context.Requests.AsQueryable();

        if (!User.IsInRole("Engineer"))
        {
            requests = requests.Where(r => r.UserId == user.Id);
        }

        var model = await requests
            .Where(r => r.Status == "активна")
            .Select(r => new RequestVM
            {
                Id = r.Id,
                Description = r.Description,
                TypeTehnology = r.TypeTechnology,
                Urgency = r.Urgency,
                Room = r.Room,
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                UserName = r.User.Name
            })
            .ToListAsync();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new RequestCreateVM
        {
            Rooms = await _context.CabinetLists.ToListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RequestCreateVM model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var selectedRoom = await _context.CabinetLists.FindAsync(model.RoomId);
            var request = new Request
            {
                Description = model.Description,
                Urgency = model.Urgency,
                Room = selectedRoom.FullCabinetName, // Используем полное имя кабинета
                CreatedAt = DateTime.Now,
                UserId = user?.Id,
                User = user,
                Status = "активна",
                TypeTechnology = model.TypeTechnology
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        model.Rooms = await _context.CabinetLists.ToListAsync(); // Перезаполнение списка кабинетов, если модель недействительна
        return View(model);
    }

    [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.UserId != _userManager.GetUserId(User) && !User.IsInRole("Engineer"))
            {
                return Forbid();
            }

            request.Status = "отменена";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Engineer"))
            {
                return Forbid();
            }

            request.Status = "выполнена";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Archive()
        {
            var user = await _userManager.GetUserAsync(User);
            var requests = _context.Requests.IgnoreQueryFilters().AsQueryable(); // Отключаем фильтрацию

            if (!User.IsInRole("Engineer"))
            {
                requests = requests.Where(r => r.UserId == user.Id);
            }

            var model = await requests
                .Where(r => r.Status == "отменена" || r.Status == "выполнена")
                .Select(r => new RequestVM
                {
                    Id = r.Id,
                    Description = r.Description,
                    Urgency = r.Urgency,
                    Room = r.Room,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status,
                    UserName = r.User.Name
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GenerateMonthlyReport()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var monthlyRequests = await _context.Requests
                .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                .Select(r => new {
                    r.Description,
                    r.Urgency,
                    r.Room,
                    r.CreatedAt,
                    r.Status,
                    UserName = r.User.Name
                })
                .ToListAsync();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 25, 25);
                var writer = PdfWriter.GetInstance(document, stream);
                writer.CloseStream = false;
                document.Open();

                // Загрузка шрифта
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts", "Roboto-Regular.ttf");
                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                var font = new Font(baseFont, 12, Font.NORMAL);
                var boldFont = new Font(baseFont, 12, Font.BOLD);

                foreach (var request in monthlyRequests)
                {
                    // Таблица для каждой заявки
                    var table = new PdfPTable(2)
                    {
                        WidthPercentage = 100,
                        SpacingBefore = 10,
                        SpacingAfter = 10
                    };
                    table.SetWidths(new float[] { 1, 3 });

                    // Описание заявки
                    table.AddCell(new PdfPCell(new Phrase("Описание заявки:", boldFont)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });
                    table.AddCell(new PdfPCell(new Phrase(request.Description, font)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });

                    // Кабинет и статус
                    var subTable = new PdfPTable(2);
                    subTable.AddCell(new PdfPCell(new Phrase("Кабинет", boldFont)));
                    subTable.AddCell(new PdfPCell(new Phrase(request.Room, font)));
                    subTable.AddCell(new PdfPCell(new Phrase("Статус", boldFont)));
                    subTable.AddCell(new PdfPCell(new Phrase(request.Status, font)));

                    table.AddCell(new PdfPCell(subTable) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });
                    table.AddCell(new PdfPCell(new Phrase()) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });

                    // Дата создания
                    table.AddCell(new PdfPCell(new Phrase("Дата создания", boldFont)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });
                    table.AddCell(new PdfPCell(new Phrase(request.CreatedAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture), font)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER });

                    // ФИО заявителя
                    table.AddCell(new PdfPCell(new Phrase("ФИО заявителя", boldFont)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER });
                    table.AddCell(new PdfPCell(new Phrase(request.UserName, font)) { Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.BOTTOM_BORDER});

                    document.Add(table);
                }

                document.Close();

                var content = stream.ToArray();
                var monthName = startDate.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU"));
                return File(content, "application/pdf", $"Отчёт за {monthName}.pdf");
            }
        }
    }
}
