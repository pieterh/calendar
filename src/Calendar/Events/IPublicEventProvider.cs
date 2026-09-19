using Calendar.Model;

namespace Calendar.Events;

/// <summary>Supplies public events (holidays, national days) for a month.</summary>
public interface IPublicEventProvider
{
    IEnumerable<CalendarEvent> GetEvents(YearMonth month);
}
