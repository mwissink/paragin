using Paragin.ExamAnalyzer.Cli.Student;
using Paragin.ExamAnalyzer.Cli.Students;

namespace Paragin.ExamAnalyzer.Cli;

using GroupType = Group;

internal static class CommandLine
{
    private const int ExitSuccess = 0;
    private const int ExitUsage = 1;
    private const int ExitInputNotFound = 2;
    private const int ExitIoError = 3;

    public static int Run(string[] args, TextWriter stdout, TextWriter stderr)
    {
        if (!TryParse(args, out var input, out var output, out var analyticsOutput, out var error))
        {
            stderr.WriteLine(error);
            WriteUsage(stderr);
            return ExitUsage;
        }

        if (!File.Exists(input))
        {
            stderr.WriteLine($"Input file not found: {input}");
            return ExitInputNotFound;
        }

        var resolvedAnalyticsOutput = string.IsNullOrWhiteSpace(analyticsOutput)
            ? DefaultAnalyticsOutput(output)
            : analyticsOutput;

        try
        {
            var students = GroupType.LoadFromXlsx(input);
            var count = students.WriteResultsCsv(output);
            var analyticsCount = students.WriteQuestionAnalyticsCsv(resolvedAnalyticsOutput);

            stdout.WriteLine($"Students: {count}");
            stdout.WriteLine($"Questions: {analyticsCount}");
        }
        catch (IOException ex)
        {
            stderr.WriteLine($"I/O error: {ex.Message}");
            return ExitIoError;
        }
        catch (UnauthorizedAccessException ex)
        {
            stderr.WriteLine($"Access denied: {ex.Message}");
            return ExitIoError;
        }

        stdout.WriteLine($"Read:      {input}");
        stdout.WriteLine($"Wrote:     {output}");
        stdout.WriteLine($"Analytics: {resolvedAnalyticsOutput}");
        return ExitSuccess;
    }

    private static string DefaultAnalyticsOutput(string output)
    {
        var dir = Path.GetDirectoryName(output);
        var name = Path.GetFileNameWithoutExtension(output);
        var ext = Path.GetExtension(output);
        var analyticsName = $"{name}.analytics{ext}";
        return string.IsNullOrEmpty(dir) ? analyticsName : Path.Combine(dir, analyticsName);
    }

    private static bool TryParse(string[] args, out string input, out string output, out string analyticsOutput, out string error)
    {
        input = string.Empty;
        output = string.Empty;
        analyticsOutput = string.Empty;
        error = string.Empty;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-i":
                case "--input":
                    if (i + 1 >= args.Length)
                    {
                        error = $"Missing value for {arg}.";
                        return false;
                    }
                    input = args[++i];
                    break;
                case "-o":
                case "--output":
                    if (i + 1 >= args.Length)
                    {
                        error = $"Missing value for {arg}.";
                        return false;
                    }
                    output = args[++i];
                    break;
                case "-a":
                case "--analytics-output":
                    if (i + 1 >= args.Length)
                    {
                        error = $"Missing value for {arg}.";
                        return false;
                    }
                    analyticsOutput = args[++i];
                    break;
                case "-h":
                case "--help":
                    error = "Help requested.";
                    return false;
                default:
                    error = $"Unknown argument: {arg}";
                    return false;
            }
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Required argument --input is missing.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(output))
        {
            error = "Required argument --output is missing.";
            return false;
        }

        return true;
    }

    private static void WriteUsage(TextWriter writer)
    {
        writer.WriteLine("Usage: paragin-exam --input <path> --output <path> [--analytics-output <path>]");
        writer.WriteLine("  -i, --input              Path to the input file.");
        writer.WriteLine("  -o, --output             Path where the results CSV will be written.");
        writer.WriteLine("  -a, --analytics-output   Path where the question analytics CSV will be written.");
        writer.WriteLine("                           Defaults to <output>.analytics.<ext> next to --output.");
        writer.WriteLine("  -h, --help               Show this help.");
    }
}
