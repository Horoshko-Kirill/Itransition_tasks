
using System.ComponentModel.DataAnnotations;

namespace MyApp.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email {  get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password is different")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
