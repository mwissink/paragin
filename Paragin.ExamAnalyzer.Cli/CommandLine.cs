namespace Paragin.ExamAnalyzer.Cli;

internal static class CommandLine
{
    private const int ExitSuccess = 0;
    private const int ExitUsage = 1;
    private const int ExitInputNotFound = 2;
    private const int ExitIoError = 3;

    public static int Run(string[] args, TextWriter stdout, TextWriter stderr)
    {
        if (!TryParse(args, out var input, out var output, out var error))
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

        try
        {
            var outputDir = Path.GetDirectoryName(Path.GetFullPath(output));
            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Placeholder: copy input to output so the round-trip works end-to-end.
            File.Copy(input, output, overwrite: true);
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

        stdout.WriteLine($"Read:  {input}");
        stdout.WriteLine($"Wrote: {output}");
        return ExitSuccess;
    }

    private static bool TryParse(string[] args, out string input, out string output, out string error)
    {
        input = string.Empty;
        output = string.Empty;
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
        writer.WriteLine("Usage: paragin-exam --input <path> --output <path>");
        writer.WriteLine("  -i, --input   Path to the input file.");
        writer.WriteLine("  -o, --output  Path where the output file will be written.");
        writer.WriteLine("  -h, --help    Show this help.");
    }
}
