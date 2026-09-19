namespace Calendar.Model;

/// <summary>Date arithmetic for rule-based events ("third Tuesday of September", Easter-relative days).</summary>
public static class DateRules
{
    /// <summary>The <paramref name="n"/>th (1-based) <paramref name="dayOfWeek"/> of the month.</summary>
    public static DateOnly NthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int n)
    {
        var first = new DateOnly(year, month, 1);
        var offset = ((int)dayOfWeek - (int)first.DayOfWeek + MonthGrid.DaysPerWeek) % MonthGrid.DaysPerWeek;
        return first.AddDays(offset + MonthGrid.DaysPerWeek * (n - 1));
    }

    /// <summary>The last <paramref name="dayOfWeek"/> of the month.</summary>
    public static DateOnly LastWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek)
    {
        var last = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        var offset = ((int)last.DayOfWeek - (int)dayOfWeek + MonthGrid.DaysPerWeek) % MonthGrid.DaysPerWeek;
        return last.AddDays(-offset);
    }

    /// <summary>Easter Sunday in the Gregorian calendar (Meeus/Jones/Butcher algorithm).</summary>
    public static DateOnly EasterSunday(int year)
    {
        var a = year % 19;
        var b = year / 100;
        var c = year % 100;
        var d = b / 4;
        var e = b % 4;
        var f = (b + 8) / 25;
        var g = (b - f + 1) / 3;
        var h = (19 * a + b - d - g + 15) % 30;
        var i = c / 4;
        var k = c % 4;
        var l = (32 + 2 * e + 2 * i - h - k) % 7;
        var m = (a + 11 * h + 22 * l) / 451;
        var month = (h + l - 7 * m + 114) / 31;
        var day = (h + l - 7 * m + 114) % 31 + 1;
        return new DateOnly(year, month, day);
    }
}
