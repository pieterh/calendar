using Calendar.Model;

namespace Calendar.Events;

/// <summary>An <see cref="IPublicEventProvider"/> over a list of events fetched up front.</summary>
public sealed class PublicEventList(IReadOnlyList<CalendarEvent> events) : IPublicEventProvider
{
    public IEnumerable<CalendarEvent> GetEvents(YearMonth month) =>
        events.Where(e => YearMonth.From(e.Date) == month);
}
