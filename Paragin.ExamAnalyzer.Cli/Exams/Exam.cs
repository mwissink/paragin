namespace Paragin.ExamAnalyzer.Cli.Exams;

internal sealed class Exam(IReadOnlyList<Question> questions)
{
    private const double MinGrade = 1.0;
    private const double PassGrade = 5.5;
    private const double MaxGrade = 10.0;

    private const double MinGradeBoundary = 0.20;
    private const double PassBoundary = 0.70;
    private const double MaxGradeBoundary = 1.00;

    public IReadOnlyList<Question> Questions { get; } = questions;

    public int MaxScore { get; } = questions.Sum(q => q.MaxScore);

    public double CalculateGrade(double score)
    {
        if (MaxScore == 0)
        {
            return MinGrade;
        }

        var fraction = score / MaxScore;
        double grade;

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

    public bool HasPassed(double grade) => grade >= PassGrade;
}
