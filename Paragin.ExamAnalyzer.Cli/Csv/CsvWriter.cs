using System.Text;

namespace Paragin.ExamAnalyzer.Cli.Csv;

internal sealed class CsvWriter
{
    private const char Separator = ';';

    public void Write(MemoryTable table, string outputPath)
    {
        var outputDir = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        using var writer = new StreamWriter(outputPath);
        var line = new StringBuilder();

        WriteRow(writer, line, table.Header);
        foreach (var row in table.Rows)
        {
            WriteRow(writer, line, row);
        }
    }

    private static void WriteRow(TextWriter writer, StringBuilder line, IReadOnlyList<string> fields)
    {
        line.Clear();
        for (var i = 0; i < fields.Count; i++)
        {
            if (i > 0)
            {
                line.Append(Separator);
            }
            line.Append(fields[i]);
        }
        writer.WriteLine(line);
    }
}
