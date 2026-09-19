using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class PublicEventListTests
{
    private static readonly CalendarEvent Koningsdag = new(new DateOnly(2026, 4, 27), "Koningsdag");
    private static readonly CalendarEvent Kerst = new(new DateOnly(2026, 12, 25), "Eerste Kerstdag");

    [Fact]
    public void ReturnsOnlyEventsOfTheMonth() =>
        Assert.Equal([Koningsdag], new PublicEventList([Koningsdag, Kerst]).GetEvents(new YearMonth(2026, 4)));

    [Fact]
    public void OtherMonth_HasNoEvents() =>
        Assert.Empty(new PublicEventList([Koningsdag, Kerst]).GetEvents(new YearMonth(2026, 10)));
}
