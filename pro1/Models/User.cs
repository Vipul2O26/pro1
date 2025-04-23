using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro1.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string Password { get; set; }

        [Required, StringLength(20)]
        [RegularExpression("^(Admin|Faculty)$", ErrorMessage = "Role must be Admin or Faculty")]
        public string Role { get; set; }

        public virtual ICollection<Exams> Exams { get; set; }
        public virtual ICollection<Audit> Audits { get; set; }
    }
}
