using System.ComponentModel.DataAnnotations;

namespace pro1.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public int UserID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^(Admin|Faculty)$", ErrorMessage = "Role must be Admin or Faculty")]
        public string Role { get; set; }
    }
}
