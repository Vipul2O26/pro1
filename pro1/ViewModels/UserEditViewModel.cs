using System.ComponentModel.DataAnnotations;

namespace pro1.ViewModels
{
    public class UserEditViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

      
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
