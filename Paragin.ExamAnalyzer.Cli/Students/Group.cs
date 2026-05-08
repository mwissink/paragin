using ClosedXML.Excel;
using System.Globalization;
using Paragin.ExamAnalyzer.Cli.Csv;
using Paragin.ExamAnalyzer.Cli.Exams;

namespace Paragin.ExamAnalyzer.Cli.Students;

internal sealed record StudentResult(int Number, string Id, double TotalScore, double Grade, bool Passed);

internal sealed class Group(Exam exam, IReadOnlyList<Student.Student> students)
{
    private const int HeaderRowNumber = 1;
    private const int MaxScoreRowNumber = 2;
    private const int FirstStudentRowNumber = 3;
    private const int IdColumnNumber = 1;
    private const int FirstQuestionColumnNumber = 2;

    public Exam Exam => exam;
    public IReadOnlyList<Student.Student> Students => students;

    public IReadOnlyList<QuestionAnalytic> QuestionAnalytics()
    {
        var totalScores = Students.Select(student => student.TotalScore).ToList();
        return Exam.Questions
            .Select((question, index) => QuestionAnalytic.Build(
                question,
                Students.Select(student => student.QuestionScores[index]).ToList(),
                totalScores))
            .ToList();
    }

    public int WriteResultsCsv(string outputPath)
    {
        var results = Students.Select(ToResult).ToList();

        var table = BuildResultsTable(results);
        new CsvWriter().Write(table, outputPath);

        return results.Count;
    }

    public int WriteQuestionAnalyticsCsv(string outputPath)
    {
        var analytics = QuestionAnalytics();

        var table = BuildQuestionAnalyticsTable(analytics);
        new CsvWriter().Write(table, outputPath);

        return analytics.Count;
    }

    public static Group LoadFromXlsx(string path)
    {
        using var workbook = new XLWorkbook(path);
        
        var sheet = workbook.Worksheet(1);
        
        var questionCount = QuestionCount(sheet);
        var exam = new Exam(ReadQuestions(sheet, questionCount));
        var students = ReadStudents(sheet, questionCount);
        
        return new Group(exam, students);
    }

    private MemoryTable BuildResultsTable(IReadOnlyList<StudentResult> results)
    {
        var header = new[] { "Number", "Id", "TotalScore", "MaxScore", "Grade", "Passed" };
        var rows = results
            .Select(r => (IReadOnlyList<string>)new[]
            {
                r.Number.ToString(CultureInfo.InvariantCulture),
                r.Id,
                r.TotalScore.ToString(CultureInfo.InvariantCulture),
                Exam.MaxScore.ToString(CultureInfo.InvariantCulture),
                r.Grade.ToString("0.0", CultureInfo.InvariantCulture),
                r.Passed ? "passed" : "failed",
            })
            .ToList();
        return new MemoryTable(header, rows);
    }

    private static MemoryTable BuildQuestionAnalyticsTable(IReadOnlyList<QuestionAnalytic> analytics)
    {
        var header = new[] { "Number", "PValue", "RitValue" };
        var rows = analytics
            .Select(a => (IReadOnlyList<string>)new[]
            {
                a.Number.ToString(CultureInfo.InvariantCulture),
                a.PValue.ToString("0.###", CultureInfo.InvariantCulture),
                a.RitValue.ToString("0.###", CultureInfo.InvariantCulture),
            })
            .ToList();
        return new MemoryTable(header, rows);
    }

    private StudentResult ToResult(Student.Student student)
    {
        var grade = Exam.CalculateGrade(student.TotalScore);
        return new StudentResult(student.Number, student.Id, student.TotalScore, grade, Exam.HasPassed(grade));
    }

    private static int QuestionCount(IXLWorksheet sheet)
    {
        var lastColumn = sheet.Row(HeaderRowNumber).LastCellUsed()?.Address.ColumnNumber ?? IdColumnNumber;
        return lastColumn - IdColumnNumber;
    }

    private static IReadOnlyList<Question> ReadQuestions(IXLWorksheet sheet, int questionCount) =>
        Enumerable.Range(0, questionCount)
            .Select(i => new Question(
                number: i + 1,
                maxScore: sheet.Row(MaxScoreRowNumber).Cell(FirstQuestionColumnNumber + i).GetValue<int>()))
            .ToList();

    private static IReadOnlyList<Student.Student> ReadStudents(IXLWorksheet sheet, int questionCount) =>
        StudentRows(sheet)
            .Select((row, index) => new Student.Student(
                number: index + 1,
                id: row.Cell(IdColumnNumber).GetString(),
                questionScores: ReadQuestionScores(row, questionCount)))
            .ToList();

    private static IEnumerable<IXLRow> StudentRows(IXLWorksheet sheet)
    {
        var rowNumber = FirstStudentRowNumber;
        while (!sheet.Row(rowNumber).Cell(IdColumnNumber).IsEmpty())
        {
            yield return sheet.Row(rowNumber);
            rowNumber++;
        }
    }

    private static IReadOnlyList<double> ReadQuestionScores(IXLRow row, int questionCount) =>
        Enumerable.Range(0, questionCount)
            .Select(i => row.Cell(FirstQuestionColumnNumber + i).GetValue<double>())
            .ToList();
}
