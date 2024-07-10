using System.ComponentModel.DataAnnotations;

namespace test_indentity.ViewModels
{
    public class CreateUserVM
    {
        [Required]
        [Display(Name = "Логин пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "ФИО пользователя")]
        public string Name { get; set; } // ФИО пользователя

        [Required]
        [Display(Name = "Роль")]
        public string SelectedRole { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Почта пользователя")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string UserPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Подтвердите пароль")]
        public string ConfirmPassword { get; set; }
    }
}
