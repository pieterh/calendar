using Calendar.Model;

namespace Calendar.Events;

/// <summary>
/// Dutch public events. Currently a fixed list; a follow-up will compute
/// recurring holidays (Koningsdag, Bevrijdingsdag, Easter-based days, Prinsjesdag = third Tuesday of September, …).
/// </summary>
public sealed class DutchPublicEvents : IPublicEventProvider
{
    private static readonly CalendarEvent[] KnownEvents =
    [
        new(new DateOnly(2026, 9, 15), "Prinsjesdag"),
    ];

    public IEnumerable<CalendarEvent> GetEvents(YearMonth month) =>
        KnownEvents.Where(e => YearMonth.From(e.Date) == month);
}
