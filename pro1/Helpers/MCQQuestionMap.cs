using CsvHelper.Configuration;
using pro1.Models;

public class MCQQuestionMap : ClassMap<MCQQuestion>
{
    public MCQQuestionMap()
    {
        Map(m => m.QuestionText).Name("Question");
        Map(m => m.OptionA).Name("Option A");
        Map(m => m.OptionB).Name("Option B");
        Map(m => m.OptionC).Name("Option C");
        Map(m => m.OptionD).Name("Option D");
        Map(m => m.CorrectAnswer).Name("CorrectAnswer");
        // SubjectUnitID is not in CSV, you will set it manually later
    }
}
