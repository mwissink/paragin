namespace Paragin.ExamAnalyzer.Cli.Exams;

internal sealed class Question(int number, int maxScore)
{
    public int Number { get; } = number;
    public int MaxScore { get; } = maxScore;
}
