namespace Paragin.ExamAnalyzer.Cli.Exams;

internal sealed record QuestionAnalytic(int Number, int MaxScore, double AverageScore, double PValue, double RitValue)
{
    public static QuestionAnalytic Build(
        Question question,
        IReadOnlyList<double> studentScores,
        IReadOnlyList<double> studentTotalScores)
    {
        var average = studentScores.Count == 0
            ? 0d
            : studentScores.Average();
        var pValue = question.MaxScore == 0
            ? 0d
            : average / question.MaxScore;
        var ritValue = PearsonCorrelation(studentScores, studentTotalScores);
        return new QuestionAnalytic(question.Number, question.MaxScore, average, pValue, ritValue);
    }

    private static double PearsonCorrelation(IReadOnlyList<double> xs, IReadOnlyList<double> ys)
    {
        if (xs.Count != ys.Count || xs.Count < 2)
        {
            return 0d;
        }

        var meanX = xs.Average();
        var meanY = ys.Average();

        var sumXy = 0d;
        var sumX2 = 0d;
        var sumY2 = 0d;
        for (var i = 0; i < xs.Count; i++)
        {
            var dx = xs[i] - meanX;
            var dy = ys[i] - meanY;
            sumXy += dx * dy;
            sumX2 += dx * dx;
            sumY2 += dy * dy;
        }

        var denominator = Math.Sqrt(sumX2 * sumY2);
        return denominator == 0d
            ? 0d
            : sumXy / denominator;
    }
}
