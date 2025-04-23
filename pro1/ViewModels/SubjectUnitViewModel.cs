using System.ComponentModel.DataAnnotations;

public class SubjectUnitViewModel
{
    [Required]
    public string SubjectName { get; set; }

    [Required]
    public int Semester { get; set; }

    public string SubjectCode { get; set; }

    [Required]
    public string UnitName { get; set; }
}
