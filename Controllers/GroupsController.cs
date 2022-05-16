using ClosedXML.Excel;
using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class GroupsController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public GroupsController(AppCtx context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.Groups
                .Include(s => s.Specialty.FormOfStudy)
                .Where(w => w.Specialty.FormOfStudy.IdUser == user.Id)
                .OrderByDescending(f => f.YearOfAdmission)
                .ThenBy(f => f.YearOfIssue)
                .ThenByDescending(f => f.Name);

            return View(await appCtx.ToListAsync());
        }

        // GET: Groups/Create
        public async Task<IActionResult> CreateAsync()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // переменная для хранения выбранного номера по списку по умолчанию
            short selectedIndex = 1;

            // создание выпадающего списка Форм обучения, в качестве аргументов:
            // элементы списка, номер элемента списка, его название и номер выбранного элемента
            SelectList formsOfStudy = new(await _context.FormsOfStudy
               .Where(w => w.IdUser == user.Id)
               .OrderBy(o => o.FormOfEdu).ToListAsync(),
               "Id", "FormOfEdu", selectedIndex);

            // передаем настроенный список с помощью ViewBag
            ViewBag.FormsOfStudy = formsOfStudy;

            // создание выпадающего списка Специальностей, который зависим от списка с Формами обучения
            // в качестве аргументов:
            // элементы списка (для текущей формы обучения), номер элемента списка, его название
            SelectList specialties = new(await _context.Specialties
                .Where(w => w.FormOfStudy.IdUser == user.Id && w.IdFormOfStudy == selectedIndex)
                .OrderBy(o => o.Code).ToListAsync(),
                "Id", "Code");

            // передаем настроенный список с помощью ViewBag
            ViewBag.Specialties = specialties;
            return View();
        }

        public async Task<IActionResult> GetItemsAsync(short id)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // с помощью частичного представления снова возвращаем список с элементами
            // (специальности для выбранного пользователем элемента списка формы обчения)
            return PartialView(
                _context.Specialties
                .Where(w => w.FormOfStudy.IdUser == user.Id && w.IdFormOfStudy == id)
                .OrderBy(o => o.Code).ToList());
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGroupViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Groups
                .Where(f => f.Specialty.FormOfStudy.IdUser == user.Id &&
                    f.Name == model.Name &&
                    f.ClassTeacher == model.ClassTeacher &&
                    f.YearOfAdmission == model.YearOfAdmission &&
                    f.YearOfIssue == model.YearOfIssue &&
                    f.IdSpecialty == model.IdSpecialty).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная группа уже существует");
            }

            if (model.YearOfIssue < model.YearOfAdmission)
            {
                ModelState.AddModelError("", "Год выпуска должен быть больше, чем год поступления");
            }

            if (ModelState.IsValid)
            {
                Group group = new()
                {
                    IdSpecialty = model.IdSpecialty,
                    Name = model.Name,
                    CountOfStudents = model.CountOfStudents,
                    YearOfAdmission = model.YearOfAdmission,
                    YearOfIssue = model.YearOfIssue,
                    ClassTeacher = model.ClassTeacher,
                    ContactsTeacher = model.ContactsTeacher
                };

                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            short selectedIndex = 1;

            SelectList formsOfStudy = new(await _context.FormsOfStudy
               .Where(w => w.IdUser == user.Id)
               .OrderBy(o => o.FormOfEdu).ToListAsync(),
               "Id", "FormOfEdu", selectedIndex);

            ViewBag.FormsOfStudy = formsOfStudy;

            SelectList specialties = new(await _context.Specialties
                .Where(w => w.FormOfStudy.IdUser == user.Id && w.IdFormOfStudy == selectedIndex)
                .OrderBy(o => o.Code).ToListAsync(),
                "Id", "Code");

            ViewBag.Specialties = specialties;

            return View(model);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            short idFormOfStudy = await _context.Specialties
                .Where(w => w.Id == group.IdSpecialty)
                .Select(s => s.IdFormOfStudy)
                .FirstOrDefaultAsync();

            EditGroupViewModel model = new()
            {
                Id = group.Id,
                IdFormOfStudy = idFormOfStudy,
                IdSpecialty = group.IdSpecialty,
                Name = group.Name,
                CountOfStudents = group.CountOfStudents,
                YearOfAdmission = group.YearOfAdmission,
                YearOfIssue = group.YearOfIssue,
                ClassTeacher = group.ClassTeacher,
                ContactsTeacher = group.ContactsTeacher
            };

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            SelectList formsOfStudy = new(await _context.FormsOfStudy
               .Where(w => w.IdUser == user.Id)
               .OrderBy(o => o.FormOfEdu).ToListAsync(),
               "Id", "FormOfEdu", idFormOfStudy);

            ViewBag.FormsOfStudy = formsOfStudy;

            SelectList specialties = new(await _context.Specialties
                .Where(w => w.FormOfStudy.IdUser == user.Id && w.IdFormOfStudy == idFormOfStudy)
                .OrderBy(o => o.Code).ToListAsync(),
                "Id", "Code");

            ViewBag.Specialties = specialties;

            return View(model);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditGroupViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            Group group = await _context.Groups.FindAsync(id);

            if (id != group.Id)
            {
                return NotFound();
            }

            if (_context.Groups
                .Where(f => f.Specialty.FormOfStudy.IdUser == user.Id &&
                    f.Name == model.Name &&
                    f.CountOfStudents == model.CountOfStudents &&
                    f.ClassTeacher == model.ClassTeacher &&
                    f.YearOfAdmission == model.YearOfAdmission &&
                    f.YearOfIssue == model.YearOfIssue &&
                    f.IdSpecialty == model.IdSpecialty).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная группа уже существует");
            }

            if (model.YearOfIssue < model.YearOfAdmission)
            {
                ModelState.AddModelError("", "Год выпуска должен быть больше, чем год поступления");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    group.IdSpecialty = model.IdSpecialty;
                    group.Name = model.Name;
                    group.CountOfStudents = model.CountOfStudents;
                    group.YearOfAdmission = model.YearOfAdmission;
                    group.YearOfIssue = model.YearOfIssue;
                    group.ClassTeacher = model.ClassTeacher;
                    group.ContactsTeacher = model.ContactsTeacher;
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Id))
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
            SelectList formsOfStudy = new(await _context.FormsOfStudy
               .Where(w => w.IdUser == user.Id)
               .OrderBy(o => o.FormOfEdu).ToListAsync(),
               "Id", "FormOfEdu", model.IdFormOfStudy);

            ViewBag.FormsOfStudy = formsOfStudy;

            SelectList specialties = new(await _context.Specialties
                .Where(w => w.FormOfStudy.IdUser == user.Id && w.IdFormOfStudy == model.IdFormOfStudy)
                .OrderBy(o => o.Code).ToListAsync(),
                "Id", "Code");

            ViewBag.Specialties = specialties;

            return View(model);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .Include(s => s.Specialty).ThenInclude(t => t.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Groups/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .Include(s => s.Specialty).ThenInclude(t => t.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return PartialView(group);
        }

        public async Task<FileResult> DownloadPattern()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // выбираем из базы данных все специальности текущего пользователя
            var appCtx = _context.Specialties
                .Include(s => s.FormOfStudy)    // устанавливая связь с формами обучения
                .Include(f => f.Groups)         // и с группами
                .Where(w => w.FormOfStudy.IdUser == user.Id)
                .OrderBy(f => f.FormOfStudy.FormOfEdu)    // сортируем записи по названию формы обучения
                .ThenBy(f => f.Code);                     // а затем по коду обучения

            int i = 1;      // счетчик

            IXLRange rngBorder;     // объект для работы с диапазонами в Excel (выделение групп ячеек)

            // создание книги Excel
            using (XLWorkbook workbook = new(XLEventTracking.Disabled))
            {
                // для каждой специальности 
                foreach (Specialty specialty in appCtx)
                {
                    // добавить лист в книгу Excel
                    // с названием 3 символа формы обучения и кода специальности
                    IXLWorksheet worksheet = workbook.Worksheets
                        .Add($"{specialty.FormOfStudy.FormOfEdu.Substring(0, 3)} {specialty.Code}");

                    // в первой строке текущего листа указываем: 
                    // в ячейку A1 значение "Форма обучения"
                    worksheet.Cell("A" + i).Value = "Форма обучения";
                    // в ячейку B1 значение - название формы обучения текущей специальности
                    worksheet.Cell("B" + i).Value = specialty.FormOfStudy.FormOfEdu;
                    // увеличение счетчика на единицу
                    i++;

                    // во второй строке
                    worksheet.Cell("A" + i).Value = "Код специальности";
                    worksheet.Cell("B" + i).Value = $"'{specialty.Code}";

                    worksheet.Cell("C" + i).Value = "Название";
                    worksheet.Cell("D" + i).Value = specialty.Name;

                    // делаем отступ на одну строку и пишем в четвертой строке
                    i += 2;
                    // заголовки у столбцов
                    worksheet.Cell("A" + i).Value = "Название группы";
                    worksheet.Cell("B" + i).Value = "Кол. студентов";
                    worksheet.Cell("C" + i).Value = "Год поступления";
                    worksheet.Cell("D" + i).Value = "Год выпуска";
                    worksheet.Cell("E" + i).Value = "Имя классного руководителя";
                    worksheet.Cell("F" + i).Value = "Контакты классного руководителя";

                    // устанавливаем внешние границы для диапазона A4:F4
                    rngBorder = worksheet.Range("A4:F4");       // создание диапазона (выделения ячеек)
                    rngBorder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;       // для диапазона задаем внешнюю границу

                    // на листе для столбцов задаем значение ширины по содержимому
                    worksheet.Columns().AdjustToContents();

                    // счетчик "обнуляем"
                    i = 1;
                }

                // создаем стрим
                using (MemoryStream stream = new())
                {
                    // помещаем в стрим созданную книгу
                    workbook.SaveAs(stream);
                    stream.Flush();

                    // возвращаем файл определенного типа
                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"groups_{DateTime.UtcNow.ToShortDateString()}.xlsx"     //в названии файла указываем таблицу и текущую дату
                    };
                }
            }
        }

        private bool GroupExists(short id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}