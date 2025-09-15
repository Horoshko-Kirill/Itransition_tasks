using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class ExportToCrmViewModel
    {
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Company name is required")]
        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
