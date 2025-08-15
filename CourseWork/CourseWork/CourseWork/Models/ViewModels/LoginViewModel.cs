using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Input email")]
        [EmailAddress(ErrorMessage = "Incorrect email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Input password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

        public bool RememberMe { get; set; }

    }
}
