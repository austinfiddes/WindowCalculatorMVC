using System.ComponentModel.DataAnnotations;

namespace MyFirstMVCApp.Models
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Prevent null values

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Prevent null values

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}