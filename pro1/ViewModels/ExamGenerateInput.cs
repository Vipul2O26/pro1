public class ExamGenerateInput
{
    public string SubjectName { get; set; }
    public Dictionary<string, int> UnitQuestions { get; set; } // Key = UnitName, Value = QuestionCount
}
