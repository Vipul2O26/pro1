using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro1.Models
{
    public class ExamQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamQuestionID { get; set; }

        [Required]
        public int ExamID { get; set; }

        [ForeignKey("ExamID")]
        public virtual Exams Exam { get; set; }

        [Required]
        public int QuestionID { get; set; }

        [ForeignKey("QuestionID")]
        public virtual MCQQuestion McqQuestion { get; set; }
    }
}
