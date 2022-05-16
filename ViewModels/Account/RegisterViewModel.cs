using System.ComponentModel.DataAnnotations;

namespace EducateApp.ViewModels
{
    // ViewModel - модель для представления,
    // т.е. какие свойства (поля) "модель" нужно заполнить на определенной странице Html "представление"
    // на странице с регистрацией помимо E-mail и пароля нужно внести информацию о преподавателе - его ФИО
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите E-mail")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]   // тип элемента управления на странице
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите отчество")]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]   // тип элемента управления на странице
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторите ввод пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]   // механизм, который сверяет текущее значение PasswordConfirm, с указанным Password
        [DataType(DataType.Password)]   // тип элемента управления на странице
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
