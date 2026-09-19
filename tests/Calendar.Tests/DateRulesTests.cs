using Calendar.Model;

namespace Calendar.Tests;

public class DateRulesTests
{
    [Theory]
    [InlineData(2012, 4, 8)]
    [InlineData(2024, 3, 31)]
    [InlineData(2025, 4, 20)]
    [InlineData(2026, 4, 5)]
    [InlineData(2027, 3, 28)]
    [InlineData(2038, 4, 25)] // latest possible Easter
    public void EasterSunday(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateRules.EasterSunday(year));
    }

    [Theory]
    [InlineData(2026, 9, 15)]
    [InlineData(2027, 9, 21)]
    public void ThirdTuesdayOfSeptember(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateRules.NthWeekdayOfMonth(year, 9, DayOfWeek.Tuesday, 3));
    }

    [Fact]
    public void NthWeekday_WhenTheFirstIsThatWeekday()
    {
        // 1 June 2026 is a Monday.
        Assert.Equal(new DateOnly(2026, 6, 1), DateRules.NthWeekdayOfMonth(2026, 6, DayOfWeek.Monday, 1));
    }

    [Theory]
    [InlineData(2026, 6, 27)]
    [InlineData(2029, 6, 30)] // the last day itself is a Saturday
    public void LastSaturdayOfJune(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateRules.LastWeekdayOfMonth(year, 6, DayOfWeek.Saturday));
    }
}
