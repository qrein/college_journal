using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EducateApp.Models.Validators
{
    public class CustomUserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();

            string patternEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string pattern = @"^[А-ЯA-Z]{1}[а-яa-z]+[\s]{0,1}[а-яa-z]{0,}$";

            if (!Regex.IsMatch(user.Email, patternEmail))
            {
                errors.Add(new IdentityError
                {
                    Description = "Неверный формат электронной почты"
                });
            }
            if (!Regex.IsMatch(user.FirstName, pattern)
                || !Regex.IsMatch(user.LastName, pattern)
                || !Regex.IsMatch(user.Patronymic, pattern))
            {
                errors.Add(new IdentityError
                {
                    Description = "ФИО должны начнаться с прописной буквы"
                });
            }
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}