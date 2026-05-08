using ClosedXML.Excel;
using Paragin.ExamAnalyzer.Cli.Exams;
using Paragin.ExamAnalyzer.Cli.Students;

namespace Paragin.ExamAnalyzer.Cli.Excel;

internal sealed class ExcelStudentsReader
{
    private const int HeaderRowNumber = 1;
    private const int MaxScoreRowNumber = 2;
    private const int FirstStudentRowNumber = 3;
    private const int IdColumnNumber = 1;
    private const int FirstQuestionColumnNumber = 2;

    public Group Read(string path)
    {
        using var workbook = new XLWorkbook(path);
        var sheet = workbook.Worksheet(1);
        var questionCount = QuestionCount(sheet);
        var exam = new Exam(ReadQuestions(sheet, questionCount));
        var students = ReadStudents(sheet, questionCount);
        return new Group(exam, students);
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
