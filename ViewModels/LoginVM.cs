using System.ComponentModel.DataAnnotations;

namespace test_indentity.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Необходимо ввести логин пользователя!")]
        public string? UserName { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Необходимо ввести пароль пользователя!")]
        [DataType(DataType.Password)]
        public string? UserPassword { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
