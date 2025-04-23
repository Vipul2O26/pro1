using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pro1.Models
{
    [Table("SubjectUnits")]
    public class SubjectUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required]
        public int Semester { get; set; }

        [StringLength(20)]
        public string SubjectCode { get; set; }

        [Required]
        [StringLength(100)]
        public string UnitName { get; set; }

        // Correct navigation property
        public virtual ICollection<MCQQuestion> MCQQuestions { get; set; }
    }
}
