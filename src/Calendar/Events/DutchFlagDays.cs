using Calendar.Model;

namespace Calendar.Events;

/// <summary>
/// The official Dutch flag days (vlagdagen) per the Rijksoverheid "vlaginstructie". A static table; nothing is
/// fetched. See docs/public-events.md for the table and its sources.
/// </summary>
public static class DutchFlagDays
{
    /// <summary>
    /// <paramref name="Alternative"/> gives the replacement date the instruction names for when the primary date
    /// is a Sunday or a generally recognised Christian holiday; null when the day is never moved.
    /// </summary>
    private sealed record Rule(
        string Name,
        FlagInstruction Flag,
        Func<int, DateOnly> Date,
        Func<DateOnly, DateOnly>? Alternative);

    private static readonly Func<DateOnly, DateOnly> NextDay = d => d.AddDays(1);
    private static readonly Func<DateOnly, DateOnly> PreviousDay = d => d.AddDays(-1);

    private static readonly Rule[] Rules =
    [
        new("Verjaardag prinses Beatrix", FlagInstruction.WithPennant, y => new(y, 1, 31), NextDay),
        new("Koningsdag", FlagInstruction.WithPennant, y => new(y, 4, 27), PreviousDay),
        new("Dodenherdenking", FlagInstruction.HalfMast, y => new(y, 5, 4), null),
        new("Bevrijdingsdag", FlagInstruction.Full, y => new(y, 5, 5), null),
        new("Verjaardag koningin Máxima", FlagInstruction.WithPennant, y => new(y, 5, 17), NextDay),
        new("Veteranendag", FlagInstruction.Full, y => DateRules.LastWeekdayOfMonth(y, 6, DayOfWeek.Saturday), null),
        new("Formeel einde Tweede Wereldoorlog", FlagInstruction.Full, y => new(y, 8, 15), NextDay),
        new("Prinsjesdag (vlag alleen in Den Haag)", FlagInstruction.Full, y => DateRules.NthWeekdayOfMonth(y, 9, DayOfWeek.Tuesday, 3), null),
        new("Verjaardag prinses Amalia (prinses van Oranje)", FlagInstruction.WithPennant, y => new(y, 12, 7), NextDay),
        new("Koninkrijksdag", FlagInstruction.Full, y => new(y, 12, 15), NextDay),
    ];

    public static IReadOnlyList<CalendarEvent> ForYear(int year)
    {
        var christianHolidays = ChristianHolidays(year);
        return Rules
            .Select(rule =>
            {
                var date = rule.Date(year);
                if (rule.Alternative is not null
                    && (date.DayOfWeek == DayOfWeek.Sunday || christianHolidays.Contains(date)))
                {
                    date = rule.Alternative(date);
                }

                return new CalendarEvent(date, rule.Name, EventKind.FlagDay, rule.Flag);
            })
            .ToList();
    }

    /// <summary>The generally recognised Christian holidays that can displace a flag day.</summary>
    private static HashSet<DateOnly> ChristianHolidays(int year)
    {
        var easter = DateRules.EasterSunday(year);
        return
        [
            easter.AddDays(-2), // Goede Vrijdag
            easter,             // Eerste Paasdag
            easter.AddDays(1),  // Tweede Paasdag
            easter.AddDays(39), // Hemelvaartsdag
            easter.AddDays(49), // Eerste Pinksterdag
            easter.AddDays(50), // Tweede Pinksterdag
            new DateOnly(year, 12, 25),
            new DateOnly(year, 12, 26),
        ];
    }
}
