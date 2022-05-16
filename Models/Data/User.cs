using EducateApp.Models.Data;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EducateApp.Models
{
    public class User : IdentityUser
    {
        //дополнительные поля для каждого пользователя
        //для преподавателя могут понадобиться данные о ФИО

        [Required(ErrorMessage = "Введите фамилию")]   // сообщение об ошибке при валидации на стороне клиента
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }   //отображение Фамилия вместо LastName

        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }


        //навигационные свойства
        [Required]
        public ICollection<FormOfStudy> FormsOfStudy { get; set; }
        public ICollection<Discipline> Disciplines { get; set; }
        public ICollection<TypeOfTotal> TypesOfTotals { get; set; }
    }
}
