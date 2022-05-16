using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.FormsOfStudy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class FormsOfStudyController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public FormsOfStudyController(
            AppCtx context,
            UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: FormsOfStudy
        public async Task<IActionResult> Index(FormOfStudySortState sortOrder = FormOfStudySortState.FormOfEduAsc)
        {
            // находим информацию о пользователе, который вошел в систему по его имени
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // через контекст данных получаем доступ к таблице базы данных FormsOfStudy
            var formsOfStudy = _context.FormsOfStudy
               .Include(f => f.User)                // и связываем с таблицей пользователи через класс User
                .Where(f => f.IdUser == user.Id);     // устанавливается условие с выбором записей форм обучения текущего пользователя по его Id
            
            ViewData["FormOfEduSort"] = sortOrder == FormOfStudySortState.FormOfEduAsc ? FormOfStudySortState.FormOfEduDesc : FormOfStudySortState.FormOfEduAsc;

            formsOfStudy = sortOrder switch
            {
                FormOfStudySortState.FormOfEduDesc => formsOfStudy.OrderByDescending(s => s.FormOfEdu),
                _ => formsOfStudy.OrderBy(s => s.FormOfEdu),
            };

            // возвращаем в представление полученный список записей
            return View(await formsOfStudy.AsNoTracking().ToListAsync());
        }

        // GET: FormsOfStudy/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFormOfStudyViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.FormsOfStudy
                .Where(f => f.IdUser == user.Id &&
                    f.FormOfEdu == model.FormOfEdu).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная форма обучения уже существует");
            }

            if (ModelState.IsValid)
            {
                FormOfStudy formOfStudy = new()
                {
                    FormOfEdu = model.FormOfEdu,
                    IdUser = user.Id
                };

                _context.Add(formOfStudy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: FormsOfStudy/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy.FindAsync(id);
            if (formOfStudy == null)
            {
                return NotFound();
            }

            EditFormOfStudyViewModel model = new()
            {
                Id = formOfStudy.Id,
                FormOfEdu = formOfStudy.FormOfEdu,
                IdUser = formOfStudy.IdUser
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditFormOfStudyViewModel model)
        {
            FormOfStudy formOfStudy = await _context.FormsOfStudy.FindAsync(id);

            if (id != formOfStudy.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.FormsOfStudy
                .Where(f => f.IdUser == user.Id &&
                    f.FormOfEdu == model.FormOfEdu).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введеная форма обучения уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    formOfStudy.FormOfEdu = model.FormOfEdu;
                    _context.Update(formOfStudy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormOfStudyExists(formOfStudy.Id))
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

        // GET: FormsOfStudy/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (formOfStudy == null)
            {
                return NotFound();
            }

            return View(formOfStudy);
        }

        // POST: FormsOfStudy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var formOfStudy = await _context.FormsOfStudy.FindAsync(id);
            _context.FormsOfStudy.Remove(formOfStudy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FormsOfStudy/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formOfStudy = await _context.FormsOfStudy
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (formOfStudy == null)
            {
                return NotFound();
            }

            return View(formOfStudy);
        }

        private bool FormOfStudyExists(short id)
        {
            return _context.FormsOfStudy.Any(e => e.Id == id);
        }
    }
}
