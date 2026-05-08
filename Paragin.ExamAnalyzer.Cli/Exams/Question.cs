namespace Paragin.ExamAnalyzer.Cli.Exam;

internal sealed class Question(int number, int maxScore)
{
    public int Number { get; } = number;
    public int MaxScore { get; } = maxScore;
}
