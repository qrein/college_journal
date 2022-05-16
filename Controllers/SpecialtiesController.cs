using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.Specialties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class SpecialtiesController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public SpecialtiesController(AppCtx context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Specialties
        public async Task<IActionResult> Index(string code, string name, short? formOfEdu,
            int page = 1,
            SpecialtySortState sortOrder = SpecialtySortState.CodeAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            int pageSize = 15;

            //фильтрация
            IQueryable<Specialty> specialties = _context.Specialties
                .Include(s => s.FormOfStudy)                    // связываем специальности с формами обучения
                .Where(w => w.FormOfStudy.IdUser == user.Id);    // в формах обучения есть поле с внешним ключом пользователя


            if (!String.IsNullOrEmpty(code))
            {
                specialties = specialties.Where(p => p.Code.Contains(code));
            }
            if (!String.IsNullOrEmpty(name))
            {
                specialties = specialties.Where(p => p.Name.Contains(name));
            }
            if (formOfEdu != null && formOfEdu != 0)
            {
                specialties = specialties.Where(p => p.IdFormOfStudy == formOfEdu);
            }


            // сортировка
            switch (sortOrder)
            {
                case SpecialtySortState.CodeDesc:
                    specialties = specialties.OrderByDescending(s => s.Code);
                    break;
                case SpecialtySortState.NameAsc:
                    specialties = specialties.OrderBy(s => s.Name);
                    break;
                case SpecialtySortState.NameDesc:
                    specialties = specialties.OrderByDescending(s => s.Name);
                    break;
                case SpecialtySortState.FormOfStudyAsc:
                    specialties = specialties.OrderBy(s => s.FormOfStudy.FormOfEdu);
                    break;
                case SpecialtySortState.FormOfStudyDesc:
                    specialties = specialties.OrderByDescending(s => s.FormOfStudy.FormOfEdu);
                    break;
                default:
                    specialties = specialties.OrderBy(s => s.Code);
                    break;
            }

            // пагинация
            var count = await specialties.CountAsync();
            var items = await specialties.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // формируем модель представления
            IndexSpecialtyViewModel viewModel = new()
            {
                PageViewModel = new(count, page, pageSize),
                SortSpecialtyViewModel = new(sortOrder),
                FilterSpecialtyViewModel = new(code, name, _context.FormsOfStudy.ToList(), formOfEdu),
                Specialties = items
            };
            return View(viewModel);
        }

        // GET: Specialties/Create
        public async Task<IActionResult> CreateAsync()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // при отображении страницы заполняем элемент "выпадающий список" формами обучения
            // при этом указываем, что в качестве идентификатора используется поле "Id"
            // а отображать пользователю нужно поле "FormOfEdu" - название формы обучения
            ViewData["IdFormOfStudy"] = new SelectList(_context.FormsOfStudy
                .Where(w => w.IdUser == user.Id), "Id", "FormOfEdu");
            return View();
        }

        // POST: Specialties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSpecialtyViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                // если введены корректные данные,
                // то создается экземпляр класса модели Specialty, т.е. формируется запись в таблицу Specialties
                Specialty specialty = new()
                {
                    Code = model.Code,
                    Name = model.Name,

                    // с помощью свойства модели получим идентификатор выбранной формы обучения пользователем
                    IdFormOfStudy = model.IdFormOfStudy
                };

                _context.Add(specialty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", model.IdFormOfStudy);
            return View(model);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            EditSpecialtyViewModel model = new()
            {
                Id = specialty.Id,
                Code = specialty.Code,
                Name = specialty.Name,
                IdFormOfStudy = specialty.IdFormOfStudy
            };

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // в списке в качестве текущего элемента устанавливаем значение из базы данных,
            // указываем параметр specialty.IdFormOfStudy
            ViewData["IdFormOfStudy"] = new SelectList(
                _context.FormsOfStudy.Where(w => w.IdUser == user.Id),
                "Id", "FormOfEdu", specialty.IdFormOfStudy);
            return View(model);
        }

        // POST: Specialties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditSpecialtyViewModel model)
        {
            Specialty specialty = await _context.Specialties.FindAsync(id);

            if (id != specialty.Id)
            {
                return NotFound();
            }
            
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            if (_context.Specialties
                .Where(f => f.FormOfStudy.IdUser == user.Id &&
                    f.Code == model.Code &&
                    f.Name == model.Name &&
                    f.IdFormOfStudy == model.IdFormOfStudy)
                .FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная специальность уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    specialty.Code = model.Code;
                    specialty.Name = model.Name;
                    specialty.IdFormOfStudy = model.IdFormOfStudy;
                    _context.Update(specialty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
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
            return View(model);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialties
                .Include(s => s.FormOfStudy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        private bool SpecialtyExists(short id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}