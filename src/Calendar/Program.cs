using System.CommandLine;
using Calendar.Cli;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.UseSystemFonts = true;
QuestPDF.Settings.ThrowOnMissingFontFamilies = false;

var months = new Argument<string[]>("months")
{
    Description = "Months to render, one page each. Forms: 2026-09; 9 (next occurrence of that month); " +
                  "ranges 2026-09..2026-12 or 9..2. Default: the current month.",
    Arity = ArgumentArity.ZeroOrMore,
};
var output = new Option<string?>("--output", "-o")
{
    Description = "Output file. Default: calendar-yyyyMM.pdf, or calendar-yyyyMM-yyyyMM.pdf for several months.",
};
var open = new Option<bool>("--open")
{
    Description = "Open the generated PDF with the default application.",
};
var temp = new Option<bool>("--temp")
{
    Description = "Write to the system temp folder and open it (implies --open).",
};
var force = new Option<bool>("--force", "-f")
{
    Description = "Overwrite an existing file without asking.",
};

var root = new RootCommand("Generates a printable PDF calendar with one month per page.")
{
    months, output, open, temp, force,
};

root.SetAction(parseResult =>
{
    var options = new CalendarOptions(
        parseResult.GetValue(months) ?? [],
        parseResult.GetValue(output),
        parseResult.GetValue(open),
        parseResult.GetValue(temp),
        parseResult.GetValue(force));
    return new CalendarApp(Console.Out, Console.Error, Console.In).Run(options);
});

var result = root.Parse(args);
if (result.Errors.Count > 0)
{
    foreach (var error in result.Errors)
    {
        Console.Error.WriteLine($"error: {error.Message}");
    }

    Console.Error.WriteLine("Use --help for usage.");
    return CalendarApp.ExitUsage;
}

return result.Invoke();
