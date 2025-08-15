using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Input username")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Input first name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Input last name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Input email")]
        [EmailAddress(ErrorMessage = "Incorrect email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Input password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
