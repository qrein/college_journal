using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels.Disciplines
{
    public class EditDisciplinesViewModel
    {
        public short Id { get; set; }

        [Display(Name = "Индекс профессионального модуля")]
        public string IndexProfModule { get; set; }

        [Display(Name = "Название профессионального модуль")]
        public string ProfModule { get; set; }

        [Required(ErrorMessage = "Введите индекс")]
        [Display(Name = "Индекс")]
        public string Index { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите краткое название")]
        [Display(Name = "Краткое название")]
        public string ShortName { get; set; }

        public string IdUser { get; set; }
    }
}
