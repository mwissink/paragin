namespace Paragin.ExamAnalyzer.Cli.Student;

internal sealed class Student(int number, string id, IReadOnlyList<decimal> questionScores)
{
    public int Number { get; } = number;
    public string Id { get; } = id;
    public IReadOnlyList<decimal> QuestionScores { get; } = questionScores;
    public decimal TotalScore { get; } = questionScores.Sum();
}
