using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class DutchFlagDaysTests
{
    private static DateOnly DateOf(int year, string name) =>
        DutchFlagDays.ForYear(year).Single(e => e.Name == name).Date;

    [Fact]
    public void Year2028_NoSundaysOrCollisions_AllOnPrimaryDates()
    {
        var expected = new[]
        {
            new CalendarEvent(new(2028, 1, 31), "Verjaardag prinses Beatrix", EventKind.FlagDay, FlagInstruction.WithPennant),
            new CalendarEvent(new(2028, 4, 27), "Koningsdag", EventKind.FlagDay, FlagInstruction.WithPennant),
            new CalendarEvent(new(2028, 5, 4), "Dodenherdenking", EventKind.FlagDay, FlagInstruction.HalfMast),
            new CalendarEvent(new(2028, 5, 5), "Bevrijdingsdag", EventKind.FlagDay, FlagInstruction.Full),
            new CalendarEvent(new(2028, 5, 17), "Verjaardag koningin Máxima", EventKind.FlagDay, FlagInstruction.WithPennant),
            new CalendarEvent(new(2028, 6, 24), "Veteranendag", EventKind.FlagDay, FlagInstruction.Full),
            new CalendarEvent(new(2028, 8, 15), "Formeel einde Tweede Wereldoorlog", EventKind.FlagDay, FlagInstruction.Full),
            new CalendarEvent(new(2028, 9, 19), "Prinsjesdag (vlag alleen in Den Haag)", EventKind.FlagDay, FlagInstruction.Full),
            new CalendarEvent(new(2028, 12, 7), "Verjaardag prinses Amalia (prinses van Oranje)", EventKind.FlagDay, FlagInstruction.WithPennant),
            new CalendarEvent(new(2028, 12, 15), "Koninkrijksdag", EventKind.FlagDay, FlagInstruction.Full),
        };

        Assert.Equal(expected, DutchFlagDays.ForYear(2028));
    }

    [Theory]
    [InlineData(2026, "Verjaardag koningin Máxima", 5, 18)]
    [InlineData(2027, "Verjaardag prinses Beatrix", 2, 1)]
    [InlineData(2027, "Formeel einde Tweede Wereldoorlog", 8, 16)]
    [InlineData(2030, "Koninkrijksdag", 12, 16)]
    [InlineData(2031, "Verjaardag prinses Amalia (prinses van Oranje)", 12, 8)]
    public void OnASunday_MovesToTheNextDay(int year, string name, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, name));
    }

    [Fact]
    public void Koningsdag_OnASunday_MovesToSaturday()
    {
        Assert.Equal(new DateOnly(2031, 4, 26), DateOf(2031, "Koningsdag"));
    }

    [Theory]
    [InlineData(2027)] // 17 May 2027 is Tweede Pinksterdag
    [InlineData(2012)] // 17 May 2012 is Hemelvaartsdag
    public void MaximaBirthday_OnAChristianHoliday_MovesTo18May(int year)
    {
        Assert.Equal(new DateOnly(year, 5, 18), DateOf(year, "Verjaardag koningin Máxima"));
    }

    [Theory]
    [InlineData(2025, "Dodenherdenking", 5, 4)] // Sunday
    [InlineData(2024, "Bevrijdingsdag", 5, 5)]  // Sunday
    public void FourAndFiveMay_NeverMove(int year, string name, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, name));
    }

    [Theory]
    [InlineData(2026, 6, 27)]
    [InlineData(2027, 6, 26)]
    public void Veteranendag_IsTheLastSaturdayOfJune(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, "Veteranendag"));
    }

    [Theory]
    [InlineData(2026, 9, 15)]
    [InlineData(2027, 9, 21)]
    public void Prinsjesdag_IsTheThirdTuesdayOfSeptember(int year, int month, int day)
    {
        Assert.Equal(new DateOnly(year, month, day), DateOf(year, "Prinsjesdag (vlag alleen in Den Haag)"));
    }

    [Fact]
    public void FlagAttributes()
    {
        var days = DutchFlagDays.ForYear(2026);

        Assert.Equal(10, days.Count);
        Assert.All(days, d => Assert.Equal(EventKind.FlagDay, d.Kind));
        Assert.Equal(
            ["Verjaardag prinses Beatrix", "Koningsdag", "Verjaardag koningin Máxima", "Verjaardag prinses Amalia (prinses van Oranje)"],
            days.Where(d => d.Flag == FlagInstruction.WithPennant).Select(d => d.Name));
        Assert.Equal(["Dodenherdenking"], days.Where(d => d.Flag == FlagInstruction.HalfMast).Select(d => d.Name));
    }
}
