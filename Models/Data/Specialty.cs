using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EducateApp.Models.Data
{
    public class Specialty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "ИД")]
        public short Id { get; set; }

        [Required(ErrorMessage = "Введите индекс специальности")]
        [Display(Name = "Индекс")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Введите название специальности")]
        [Display(Name = "Специальность")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Форма обучения")]
        public short IdFormOfStudy { get; set; }


        // Навигационные свойства
        [Display(Name = "Форма обучения")]
        [ForeignKey("IdFormOfStudy")]
        public FormOfStudy FormOfStudy { get; set; }

        public ICollection<Group> Groups { get; set; }
    }
}