namespace Paragin.ExamAnalyzer.Cli.Student;

internal sealed class Student(int number, string id, IReadOnlyList<double> questionScores)
{
    public int Number { get; } = number;
    public string Id { get; } = id;
    public IReadOnlyList<double> QuestionScores { get; } = questionScores;
    public double TotalScore { get; } = questionScores.Sum();
}
