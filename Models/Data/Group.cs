using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducateApp.Models.Data
{
    public class Group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ИД")]
        public short Id { get; set; }

        [Required]
        [Display(Name = "Специальность")]
        public short IdSpecialty { get; set; }

        [Required(ErrorMessage = "Введите название группы")]
        [Display(Name = "Группа")]
        public string Name { get; set; }

        [Display(Name = "Кол. студентов")]
        [Range(1, 255, ErrorMessage = "Введите число от 1 до 255")]
        public byte? CountOfStudents { get; set; }

        [Required(ErrorMessage = "Введите год поступления")]
        [Display(Name = "Год пост.")]
        [Range(1950, 9999, ErrorMessage = "Введите корректное число")]
        public short YearOfAdmission { get; set; }

        [Required(ErrorMessage = "Введите год выпуска")]
        [Display(Name = "Год выпуска")]
        [Range(1950, 9999, ErrorMessage = "Введите корректное число")]
        public short YearOfIssue { get; set; }

        [Required(ErrorMessage = "Введите имя классного руководителя")]
        [Display(Name = "Имя кл. рук.")]
        [DataType(DataType.Text)]
        public string ClassTeacher { get; set; }

        [Display(Name = "Контакты кл. рук.")]
        public string ContactsTeacher { get; set; }


        // Навигационные свойства
        [Display(Name = "Специальность")]
        [ForeignKey("IdSpecialty")]
        public Specialty Specialty { get; set; }
    }
}