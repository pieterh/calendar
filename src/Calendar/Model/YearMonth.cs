namespace Calendar.Model;

/// <summary>A calendar month in a specific year, e.g. 2026-09.</summary>
public readonly record struct YearMonth(int Year, int Month) : IComparable<YearMonth>
{
    public static YearMonth From(DateOnly date) => new(date.Year, date.Month);

    public DateOnly FirstDay => new(Year, Month, 1);

    public int DayCount => DateTime.DaysInMonth(Year, Month);

    public YearMonth Next() => Month == 12 ? new(Year + 1, 1) : new(Year, Month + 1);

    public YearMonth AddYears(int years) => new(Year + years, Month);

    public int CompareTo(YearMonth other) =>
        Year != other.Year ? Year.CompareTo(other.Year) : Month.CompareTo(other.Month);

    public static bool operator <(YearMonth a, YearMonth b) => a.CompareTo(b) < 0;
    public static bool operator >(YearMonth a, YearMonth b) => a.CompareTo(b) > 0;
    public static bool operator <=(YearMonth a, YearMonth b) => a.CompareTo(b) <= 0;
    public static bool operator >=(YearMonth a, YearMonth b) => a.CompareTo(b) >= 0;

    /// <summary>Formats as <c>yyyyMM</c>, the form used in default file names.</summary>
    public override string ToString() => $"{Year:D4}{Month:D2}";
}
