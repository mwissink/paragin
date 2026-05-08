namespace Paragin.ExamAnalyzer.Cli.Exams;

internal sealed record QuestionAnalytic(int Number, int MaxScore, decimal AverageScore, decimal PValue)
{
    public static QuestionAnalytic Build(Question question, IReadOnlyList<decimal> studentScores)
    {
        var average = studentScores.Count == 0
            ? 0m
            : studentScores.Average();
        var pValue = question.MaxScore == 0
            ? 0m
            : average / question.MaxScore;
        return new QuestionAnalytic(question.Number, question.MaxScore, average, pValue);
    }
}
