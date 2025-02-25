using System.ComponentModel.DataAnnotations;

namespace MyFirstMVCApp.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // Ensure non-null default

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Ensure non-null default

        public bool RememberMe { get; set; }
    }
}