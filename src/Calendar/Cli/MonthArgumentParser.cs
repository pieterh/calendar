using System.Globalization;
using Calendar.Model;

namespace Calendar.Cli;

/// <summary>
/// Turns month arguments into a sorted, de-duplicated list of <see cref="YearMonth"/>.
/// Accepted forms: <c>yyyy-MM</c>; a bare month number <c>M</c> / <c>MM</c> meaning the next
/// occurrence of that month (this year if not already past, otherwise next year);
/// and ranges <c>a..b</c> of either form.
/// </summary>
public sealed class MonthArgumentParser(DateOnly today)
{
    private const string RangeSeparator = "..";

    public YearMonth Current => YearMonth.From(today);

    /// <exception cref="FormatException">An argument is not a recognised month or range.</exception>
    public IReadOnlyList<YearMonth> Parse(IEnumerable<string> arguments)
    {
        var months = new SortedSet<YearMonth>();
        var any = false;
        foreach (var argument in arguments)
        {
            any = true;
            foreach (var month in ParseOne(argument))
            {
                months.Add(month);
            }
        }

        if (!any)
        {
            months.Add(Current);
        }

        return months.ToList();
    }

    private IEnumerable<YearMonth> ParseOne(string argument)
    {
        var separator = argument.IndexOf(RangeSeparator, StringComparison.Ordinal);
        if (separator < 0)
        {
            return [ParseSingle(argument)];
        }

        var startText = argument[..separator];
        var endText = argument[(separator + RangeSeparator.Length)..];
        var start = ParseSingle(startText);
        var end = ParseSingle(endText);

        // A bare-number end that lands before the start (e.g. "9..2") means the next year.
        if (end < start && IsBareMonth(endText))
        {
            end = end.AddYears(1);
        }

        if (end < start)
        {
            throw new FormatException($"Range '{argument}' ends before it starts.");
        }

        return Enumerate(start, end);
    }

    private YearMonth ParseSingle(string text)
    {
        text = text.Trim();
        if (text.StartsWith('-'))
        {
            throw new FormatException($"Unrecognized option '{text}'.");
        }

        if (IsBareMonth(text))
        {
            var month = int.Parse(text, CultureInfo.InvariantCulture);
            if (month is < 1 or > 12)
            {
                throw new FormatException($"'{text}' is not a valid month number (1-12).");
            }

            return month >= today.Month
                ? new YearMonth(today.Year, month)
                : new YearMonth(today.Year + 1, month);
        }

        if (DateOnly.TryParseExact(text, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return YearMonth.From(date);
        }

        throw new FormatException($"'{text}' is not a valid month. Use yyyy-MM (e.g. 2026-09) or a month number (1-12).");
    }

    private static bool IsBareMonth(string text) =>
        text.Trim() is { Length: 1 or 2 } t && t.All(char.IsAsciiDigit);

    private static IEnumerable<YearMonth> Enumerate(YearMonth start, YearMonth end)
    {
        for (var m = start; m <= end; m = m.Next())
        {
            yield return m;
        }
    }
}
