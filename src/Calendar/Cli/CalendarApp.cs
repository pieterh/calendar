using System.Globalization;
using System.Text.Json;
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

/// <summary>Runs one invocation: parse months → resolve path → overwrite check → fetch holidays → render → open.</summary>
public sealed class CalendarApp(TextWriter stdout, TextWriter stderr, TextReader stdin)
{
    public const int ExitOk = 0;
    public const int ExitFailure = 1;
    public const int ExitUsage = 2;

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("nl-NL");
    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(10);

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

        var events = LoadPublicEvents(months);

        try
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            new CalendarDocument(months, events, Culture).GeneratePdf(outputPath);
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

    /// <summary>
    /// Collects the events for every year that is on the calendar: the static flag days and observances plus
    /// the public holidays fetched from the API. A year whose fetch fails (offline, API down, unexpected response) is
    /// reported on stderr and simply has no holidays.
    /// </summary>
    private PublicEventList LoadPublicEvents(IReadOnlyList<YearMonth> months)
    {
        using var http = new HttpClient { Timeout = HttpTimeout };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("calendar-cli");
        var client = new NagerDateClient(http);

        var events = new List<CalendarEvent>();
        foreach (var year in months.Select(m => m.Year).Distinct())
        {
            events.AddRange(DutchFlagDays.ForYear(year));
            events.AddRange(DutchObservances.ForYear(year));
            try
            {
                events.AddRange(client.GetHolidays(year));
            }
            catch (Exception e) when (e is HttpRequestException or TaskCanceledException or JsonException)
            {
                stderr.WriteLine($"warning: could not fetch public holidays for {year}: {e.Message}");
            }
        }

        return new PublicEventList(events);
    }
}
