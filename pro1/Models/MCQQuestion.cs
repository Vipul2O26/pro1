using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro1.Models
{
    public class MCQQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionID { get; set; }

        // Foreign key to SubjectUnit
        [Required]
        public int SubjectUnitID { get; set; }

        [ForeignKey("SubjectUnitID")]
        public virtual SubjectUnit SubjectUnit { get; set; }

        [Required, StringLength(500)]
        public string QuestionText { get; set; }

        [Required, StringLength(200)]
        public string OptionA { get; set; }

        [Required, StringLength(200)]
        public string OptionB { get; set; }

        [Required, StringLength(200)]
        public string OptionC { get; set; }

        [Required, StringLength(200)]
        public string OptionD { get; set; }

        [Required]
        [RegularExpression("^[ABCD]$", ErrorMessage = "CorrectAnswer must be one of A, B, C, or D")]
        public string CorrectAnswer { get; set; }

        public virtual ICollection<ExamQuestion> ExamQuestions { get; set; }
    }
}