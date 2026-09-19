using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class MonthGridTests
{
    [Fact]
    public void September2026_HasFiveRows_Weeks36To40()
    {
        var grid = new MonthGrid(new YearMonth(2026, 9), []);

        Assert.Equal(5, grid.Rows.Count);
        Assert.Equal([36, 37, 38, 39, 40], grid.Rows.Select(r => r.IsoWeek));
    }

    [Fact]
    public void September2026_StartsOnTuesday()
    {
        var grid = new MonthGrid(new YearMonth(2026, 9), []);
        var firstRow = grid.Rows[0].Days;

        Assert.False(firstRow[0].IsInMonth);          // Monday 31 Aug → blank
        Assert.Equal(1, firstRow[1].DayNumber);        // Tuesday 1 Sep
        Assert.Equal(6, firstRow[6].DayNumber);        // Sunday 6 Sep
        Assert.Equal(30, grid.Rows[4].Days[2].DayNumber); // Wednesday 30 Sep
        Assert.False(grid.Rows[4].Days[3].IsInMonth);
    }

    [Fact]
    public void EveryRowHasSevenCells()
    {
        var grid = new MonthGrid(new YearMonth(2026, 9), []);
        Assert.All(grid.Rows, r => Assert.Equal(MonthGrid.DaysPerWeek, r.Days.Count));
    }

    [Fact]
    public void February2027_StartsOnMonday_HasFourRows()
    {
        var grid = new MonthGrid(new YearMonth(2027, 2), []);

        Assert.Equal(4, grid.Rows.Count);
        Assert.Equal(1, grid.Rows[0].Days[0].DayNumber);
        Assert.Equal(28, grid.Rows[3].Days[6].DayNumber);
    }

    [Fact]
    public void May2027_NeedsSixRows()
    {
        // 1 May 2027 is a Saturday and the month has 31 days → 6 rows.
        var grid = new MonthGrid(new YearMonth(2027, 5), []);
        Assert.Equal(6, grid.Rows.Count);
    }

    [Fact]
    public void January2027_FirstRowIsIsoWeek53Of2026()
    {
        var grid = new MonthGrid(new YearMonth(2027, 1), []);
        Assert.Equal(53, grid.Rows[0].IsoWeek);
        Assert.Equal(1, grid.Rows[1].IsoWeek);
    }

    [Fact]
    public void Events_AreAttachedToTheirDay()
    {
        var prinsjesdag = new CalendarEvent(new DateOnly(2026, 9, 15), "Prinsjesdag");
        var grid = new MonthGrid(new YearMonth(2026, 9), [prinsjesdag]);

        var cell = grid.Rows[2].Days[1]; // week 38, Tuesday
        Assert.Equal(15, cell.DayNumber);
        Assert.Equal([prinsjesdag], cell.Events);
        Assert.Empty(grid.Rows[2].Days[0].Events);
    }

    [Fact]
    public void SameDayFromTwoSources_NamedOnce_FilledAndFlagged()
    {
        var fromApi = new CalendarEvent(new DateOnly(2026, 4, 27), "Koningsdag");
        var grid = new MonthGrid(new YearMonth(2026, 4), [fromApi, .. DutchFlagDays.ForYear(2026)]);

        var cell = grid.Rows[4].Days[0]; // Monday 27 April
        Assert.Equal(27, cell.DayNumber);
        Assert.Equal(2, cell.Events.Count);
        Assert.Equal(["Koningsdag"], cell.Names);
        Assert.True(cell.IsPublicHoliday);
        Assert.Equal(FlagInstruction.WithPennant, cell.Flag);
    }

    [Fact]
    public void FlagDayOnly_IsNotAPublicHoliday()
    {
        var grid = new MonthGrid(new YearMonth(2026, 5), DutchFlagDays.ForYear(2026));

        var cell = grid.Rows[1].Days[0]; // Monday 4 May
        Assert.Equal(4, cell.DayNumber);
        Assert.False(cell.IsPublicHoliday);
        Assert.Equal(FlagInstruction.HalfMast, cell.Flag);
        Assert.Equal(["Dodenherdenking"], cell.Names);
    }

    [Fact]
    public void PublicHolidayOnly_HasNoFlag()
    {
        var grid = new MonthGrid(new YearMonth(2026, 1), [new CalendarEvent(new DateOnly(2026, 1, 1), "Nieuwjaarsdag")]);

        var cell = grid.Rows[0].Days[3]; // Thursday 1 January
        Assert.True(cell.IsPublicHoliday);
        Assert.Equal(FlagInstruction.None, cell.Flag);
    }

    [Fact]
    public void ObservanceOnly_IsNotAPublicHoliday_HasNoFlagOrIcon()
    {
        var grid = new MonthGrid(new YearMonth(2026, 12), DutchObservances.ForYear(2026));

        var cell = grid.Rows[0].Days[5]; // Saturday 5 December
        Assert.Equal(5, cell.DayNumber);
        Assert.False(cell.IsPublicHoliday);
        Assert.Equal(FlagInstruction.None, cell.Flag);
        Assert.Equal(EventIcon.None, cell.Icon);
        Assert.Equal(["Sinterklaas"], cell.Names);
    }

    [Fact]
    public void ClockChangeOnAHoliday_FilledAndIconed_BothNamed()
    {
        // 28 March 2027 is Eerste Paasdag and the switch to summer time.
        var easter = new CalendarEvent(new DateOnly(2027, 3, 28), "Eerste Paasdag");
        var grid = new MonthGrid(new YearMonth(2027, 3), [easter, .. DutchObservances.ForYear(2027)]);

        var cell = grid.Rows[3].Days[6]; // Sunday 28 March
        Assert.Equal(28, cell.DayNumber);
        Assert.True(cell.IsPublicHoliday);
        Assert.Equal(EventIcon.ClockForward, cell.Icon);
        Assert.Equal(["Eerste Paasdag", "Zomertijd"], cell.Names);
    }
}
