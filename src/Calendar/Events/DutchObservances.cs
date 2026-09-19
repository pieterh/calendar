using Calendar.Model;

namespace Calendar.Events;

/// <summary>
/// Well-known Dutch days that are neither public holidays nor flag days: Sinterklaas, Dierendag, the clock
/// changes, and so on. A static table; nothing is fetched. See docs/public-events.md.
/// </summary>
public static class DutchObservances
{
    private sealed record Rule(string Name, Func<int, DateOnly> Date, EventIcon Icon = EventIcon.None);

    // In date order. Unlike flag days these never move, whatever weekday they fall on.
    private static readonly Rule[] Rules =
    [
        new("Driekoningen", y => new(y, 1, 6)),
        new("Zomertijd", y => DateRules.LastWeekdayOfMonth(y, 3, DayOfWeek.Sunday), EventIcon.ClockForward),
        new("Dierendag", y => new(y, 10, 4)),
        new("Wintertijd", y => DateRules.LastWeekdayOfMonth(y, 10, DayOfWeek.Sunday), EventIcon.ClockBack),
        new("Sint-Maarten", y => new(y, 11, 11)),
        new("Sinterklaas", y => new(y, 12, 5)),
        new("Oudejaarsdag", y => new(y, 12, 31)),
    ];

    public static IReadOnlyList<CalendarEvent> ForYear(int year) =>
        Rules
            .Select(rule => new CalendarEvent(rule.Date(year), rule.Name, EventKind.Observance, Icon: rule.Icon))
            .ToList();
}
