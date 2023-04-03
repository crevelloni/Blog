using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "E-mail is required ")]
        [EmailAddress(ErrorMessage = "E-mail format is invalid")]
        public string Email { get; set; }
    }
}
