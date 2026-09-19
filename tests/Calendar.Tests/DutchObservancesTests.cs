using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class DutchObservancesTests
{
    private static DateOnly DateOf(int year, string name) =>
        DutchObservances.ForYear(year).Single(e => e.Name == name).Date;

    [Fact]
    public void Year2026_FullTable()
    {
        var expected = new[]
        {
            new CalendarEvent(new(2026, 1, 6), "Driekoningen", EventKind.Observance),
            new CalendarEvent(new(2026, 3, 29), "Zomertijd", EventKind.Observance, Icon: EventIcon.ClockForward),
            new CalendarEvent(new(2026, 10, 4), "Dierendag", EventKind.Observance),
            new CalendarEvent(new(2026, 10, 25), "Wintertijd", EventKind.Observance, Icon: EventIcon.ClockBack),
            new CalendarEvent(new(2026, 11, 11), "Sint-Maarten", EventKind.Observance),
            new CalendarEvent(new(2026, 12, 5), "Sinterklaas", EventKind.Observance),
            new CalendarEvent(new(2026, 12, 31), "Oudejaarsdag", EventKind.Observance),
        };

        Assert.Equal(expected, DutchObservances.ForYear(2026));
    }

    [Theory]
    [InlineData(2025, 3, 30)]
    [InlineData(2027, 3, 28)]
    [InlineData(2028, 3, 26)]
    public void Zomertijd_IsTheLastSundayOfMarch(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, "Zomertijd"));
    }

    [Theory]
    [InlineData(2025, 10, 26)]
    [InlineData(2027, 10, 31)]
    [InlineData(2028, 10, 29)]
    public void Wintertijd_IsTheLastSundayOfOctober(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, "Wintertijd"));
    }

    [Fact]
    public void FixedDates_NeverMove_EvenOnASunday()
    {
        Assert.Equal(new DateOnly(2026, 10, 4), DateOf(2026, "Dierendag")); // Sunday
        Assert.Equal(new DateOnly(2027, 12, 5), DateOf(2027, "Sinterklaas")); // Sunday
    }

    [Fact]
    public void Attributes()
    {
        var days = DutchObservances.ForYear(2026);

        Assert.Equal(7, days.Count);
        Assert.All(days, d => Assert.Equal(EventKind.Observance, d.Kind));
        Assert.All(days, d => Assert.Equal(FlagInstruction.None, d.Flag));
        Assert.Equal(["Zomertijd"], days.Where(d => d.Icon == EventIcon.ClockForward).Select(d => d.Name));
        Assert.Equal(["Wintertijd"], days.Where(d => d.Icon == EventIcon.ClockBack).Select(d => d.Name));
    }
}
