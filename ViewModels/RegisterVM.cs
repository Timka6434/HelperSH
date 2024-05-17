using System.ComponentModel.DataAnnotations;

namespace test_indentity.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType (DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        [Display(Name = "Подтвердите пароль!")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword {  get; set; }
    }
}
