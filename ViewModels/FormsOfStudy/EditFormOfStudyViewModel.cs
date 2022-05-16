using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.FormsOfStudy
{
    public class EditFormOfStudyViewModel
    {
        public short Id { get; set; }

        [Required(ErrorMessage = "Введите название формы обучения")]
        [Display(Name = "Форма обучения")]
        public string FormOfEdu { get; set; }

        public string IdUser { get; set; }
    }
}