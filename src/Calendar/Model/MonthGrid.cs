using System.Globalization;

namespace Calendar.Model;

/// <summary>One cell in the month grid. <see cref="Date"/> is null for padding cells outside the month.</summary>
public sealed record DayCell(DateOnly? Date, IReadOnlyList<CalendarEvent> Events)
{
    public bool IsInMonth => Date is not null;
    public int? DayNumber => Date?.Day;

    public bool IsPublicHoliday => Events.Any(e => e.Kind == EventKind.PublicHoliday);

    /// <summary>The flag instruction for the day, or <see cref="FlagInstruction.None"/>.</summary>
    public FlagInstruction Flag => Events.Select(e => e.Flag).FirstOrDefault(f => f != FlagInstruction.None);

    /// <summary>The icon for the day, or <see cref="EventIcon.None"/>.</summary>
    public EventIcon Icon => Events.Select(e => e.Icon).FirstOrDefault(i => i != EventIcon.None);

    /// <summary>Event names to print; a day known from two sources (e.g. Koningsdag) is named once.</summary>
    public IReadOnlyList<string> Names => Events.Select(e => e.Name).Distinct(StringComparer.Ordinal).ToList();
}

/// <summary>A week row: seven cells, Monday first, plus the ISO-8601 week number.</summary>
public sealed record WeekRow(int IsoWeek, IReadOnlyList<DayCell> Days);

/// <summary>Lays a month out as Monday-first week rows, only as many rows as the month needs (4–6).</summary>
public sealed class MonthGrid
{
    public const int DaysPerWeek = 7;

    public YearMonth Month { get; }
    public IReadOnlyList<WeekRow> Rows { get; }

    public MonthGrid(YearMonth month, IEnumerable<CalendarEvent> events)
    {
        Month = month;
        var eventsByDate = events
            .GroupBy(e => e.Date)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<CalendarEvent>)g.ToList());

        var first = month.FirstDay;
        // DayOfWeek has Sunday = 0; shift so Monday = 0 … Sunday = 6.
        var leadingBlanks = ((int)first.DayOfWeek + 6) % DaysPerWeek;
        var rowStart = first.AddDays(-leadingBlanks);

        var rows = new List<WeekRow>();
        while (YearMonth.From(rowStart) <= month)
        {
            var cells = new DayCell[DaysPerWeek];
            for (var i = 0; i < DaysPerWeek; i++)
            {
                var date = rowStart.AddDays(i);
                cells[i] = YearMonth.From(date) == month
                    ? new DayCell(date, eventsByDate.GetValueOrDefault(date, []))
                    : new DayCell(null, []);
            }

            rows.Add(new WeekRow(ISOWeek.GetWeekOfYear(rowStart.ToDateTime(TimeOnly.MinValue)), cells));
            rowStart = rowStart.AddDays(DaysPerWeek);
        }

        Rows = rows;
    }
}
