using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducateApp.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // так как в приложении не требуется роли кроме Администратор, Зарегистрированный пользователь
            // роли задаются при помощи инициализации
            // в будущем вы можете добавить список ролей и работу с ними, так же как и с пользователями

            // обычно такие данные об администраторе хранятся с помощью разработки MS Azure,
            // но пользование данной системой бесплатно только год

            string adminEmail = "katedesu@mail.ru";
            string password = "_Aa123456";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("registeredUser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("registeredUser"));
            }

            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    LastName = "Петрова",
                    FirstName = "Екатерина",
                    Patronymic = "Сергеевна",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}