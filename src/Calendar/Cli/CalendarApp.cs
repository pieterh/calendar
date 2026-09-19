using System.Globalization;
using Calendar.Events;
using Calendar.Model;
using Calendar.Output;
using Calendar.Rendering;
using QuestPDF.Fluent;

namespace Calendar.Cli;

public sealed record CalendarOptions(
    IReadOnlyList<string> MonthArguments,
    string? OutputPath,
    bool Open,
    bool UseTemp,
    bool Force);

/// <summary>Runs one invocation: parse months → resolve path → overwrite check → render → open.</summary>
public sealed class CalendarApp(TextWriter stdout, TextWriter stderr, TextReader stdin)
{
    public const int ExitOk = 0;
    public const int ExitFailure = 1;
    public const int ExitUsage = 2;

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("nl-NL");

    public int Run(CalendarOptions options)
    {
        IReadOnlyList<YearMonth> months;
        string outputPath;
        try
        {
            months = new MonthArgumentParser(DateOnly.FromDateTime(DateTime.Today)).Parse(options.MonthArguments);
            outputPath = OutputPathResolver.Resolve(
                months, options.OutputPath, options.UseTemp, Environment.CurrentDirectory, Path.GetTempPath());
        }
        catch (Exception e) when (e is FormatException or ArgumentException)
        {
            stderr.WriteLine($"error: {e.Message}");
            return ExitUsage;
        }

        if (File.Exists(outputPath) && !options.Force && !OverwritePrompt.Confirm(outputPath, stdout, stdin))
        {
            stderr.WriteLine("Cancelled.");
            return ExitFailure;
        }

        try
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            new CalendarDocument(months, new DutchPublicEvents(), Culture).GeneratePdf(outputPath);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            stderr.WriteLine($"error: could not write '{outputPath}': {e.Message}");
            return ExitFailure;
        }

        stdout.WriteLine($"Written {outputPath} ({months.Count} {(months.Count == 1 ? "page" : "pages")}).");

        if (options.Open || options.UseTemp)
        {
            try
            {
                PdfOpener.Open(outputPath);
            }
            catch (Exception e) when (e is InvalidOperationException or System.ComponentModel.Win32Exception)
            {
                stderr.WriteLine($"error: could not open '{outputPath}': {e.Message}");
                return ExitFailure;
            }
        }

        return ExitOk;
    }
}
