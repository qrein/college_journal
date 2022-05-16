using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.Specialties
{
    public class EditSpecialtyViewModel
    {
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
    }
}
