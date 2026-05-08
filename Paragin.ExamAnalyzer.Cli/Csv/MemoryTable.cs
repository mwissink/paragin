namespace Paragin.ExamAnalyzer.Cli.Csv;

internal sealed class MemoryTable(IReadOnlyList<string> header, IReadOnlyList<IReadOnlyList<string>> rows)
{
    public IReadOnlyList<string> Header { get; } = header;
    public IReadOnlyList<IReadOnlyList<string>> Rows { get; } = rows;
}
