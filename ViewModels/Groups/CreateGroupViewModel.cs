using System;
using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.Groups
{
    public class CreateGroupViewModel
    {
        [Required]
        [Display(Name = "Выберите форму обучения")]
        public short IdFormOfStudy { get; set; }

        [Required]
        [Display(Name = "Выберите специальность")]
        public short IdSpecialty { get; set; }

        [Required(ErrorMessage = "Введите название группы")]
        [Display(Name = "Группа")]
        public string Name { get; set; }

        [Display(Name = "Количество студентов")]
        [Range(1, 255, ErrorMessage = "Введите число от 1 до 255")]
        public byte? CountOfStudents { get; set; }

        [Required(ErrorMessage = "Введите год поступления")]
        [Display(Name = "Год поступления")]
        [Range(1950, 9999, ErrorMessage = "Введите корректное число")]
        public short YearOfAdmission { get; set; }

        [Required(ErrorMessage = "Введите год выпуска")]
        [Display(Name = "Год выпуска")]
        [Range(1950, 9999, ErrorMessage = "Введите корректное число")]
        public short YearOfIssue { get; set; }

        [Required(ErrorMessage = "Введите имя классного руководителя")]
        [Display(Name = "Имя классного руководителя")]
        [DataType(DataType.Text)]
        public string ClassTeacher { get; set; }

        [Display(Name = "Контакты классного руководителя")]
        public string ContactsTeacher { get; set; }
    }
}