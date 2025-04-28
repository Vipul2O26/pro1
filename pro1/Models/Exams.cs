using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro1.Models
{
    public class Exams
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamID { get; set; }

        [Required]
        public int FacultyID { get; set; }

        [ForeignKey("FacultyID")]
        public virtual User Faculty { get; set; }

        [Required]
        public int SubjectUnitID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? SubjectCode { get; set; }


        [Required]
        public int TotalQuestions { get; set; }

        [Required]
        public int DurationTime { get; set; } // in minutes

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("SubjectUnitID")]
        public virtual SubjectUnit SubjectUnit { get; set; }

        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
