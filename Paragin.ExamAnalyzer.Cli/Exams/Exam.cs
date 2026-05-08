namespace Paragin.ExamAnalyzer.Cli.Exam;

internal sealed class Exam(IReadOnlyList<Question> questions)
{
    private const decimal MinGrade = 1.0m;
    private const decimal PassGrade = 5.5m;
    private const decimal MaxGrade = 10.0m;

    private const decimal MinGradeBoundary = 0.20m;
    private const decimal PassBoundary = 0.70m;
    private const decimal MaxGradeBoundary = 1.00m;

    public IReadOnlyList<Question> Questions { get; } = questions;

    public int MaxScore { get; } = questions.Sum(q => q.MaxScore);

    public decimal CalculateGrade(decimal score)
    {
        if (MaxScore == 0)
        {
            return MinGrade;
        }

        var fraction = score / MaxScore;
        decimal grade;

        if (fraction <= MinGradeBoundary)
        {
            grade = MinGrade;
        }
        else if (fraction <= PassBoundary)
        {
            grade = MinGrade + (fraction - MinGradeBoundary) / (PassBoundary - MinGradeBoundary) * (PassGrade - MinGrade);
        }
        else if (fraction <= MaxGradeBoundary)
        {
            grade = PassGrade + (fraction - PassBoundary) / (MaxGradeBoundary - PassBoundary) * (MaxGrade - PassGrade);
        }
        else
        {
            grade = MaxGrade;
        }

        return Math.Round(grade, 1, MidpointRounding.AwayFromZero);
    }

    public bool HasPassed(decimal grade) => grade >= PassGrade;
}
