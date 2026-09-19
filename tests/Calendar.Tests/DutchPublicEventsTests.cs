using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class DutchPublicEventsTests
{
    [Fact]
    public void September2026_HasPrinsjesdag()
    {
        var events = new DutchPublicEvents().GetEvents(new YearMonth(2026, 9)).ToList();
        Assert.Equal([new CalendarEvent(new DateOnly(2026, 9, 15), "Prinsjesdag")], events);
    }

    [Fact]
    public void OtherMonth_HasNoEvents() =>
        Assert.Empty(new DutchPublicEvents().GetEvents(new YearMonth(2026, 10)));
}
