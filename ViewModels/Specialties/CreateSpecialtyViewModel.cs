using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.Specialties
{
    public class CreateSpecialtyViewModel
    {
        [Required(ErrorMessage = "Введите индекс специальности")]
        [Display(Name = "Индекс")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Введите название специальности")]
        [Display(Name = "Специальность")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Форма обучения")]
        public short IdFormOfStudy { get; set; }    //будем передавать ИД формы обучения
    }
}