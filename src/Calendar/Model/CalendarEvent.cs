namespace Calendar.Model;

/// <summary>What an event is. Only public holidays get the coloured cell fill.</summary>
public enum EventKind
{
    PublicHoliday,
    FlagDay,

    /// <summary>A well-known day that is neither a holiday nor a flag day (Sinterklaas, Dierendag, …): named only.</summary>
    Observance,
}

/// <summary>
/// How the flag is flown on a flag day. <see cref="WithPennant"/> is "met oranje wimpel". The states are
/// mutually exclusive: a half-mast flag never carries a pennant.
/// </summary>
public enum FlagInstruction
{
    None,
    Full,
    WithPennant,
    HalfMast,
}

/// <summary>A small marker drawn in the cell besides the flag; currently only the clock change.</summary>
public enum EventIcon
{
    None,

    /// <summary>Clocks go forward one hour: start of summer time.</summary>
    ClockForward,

    /// <summary>Clocks go back one hour: start of winter time.</summary>
    ClockBack,
}

/// <summary>A named single-day event shown in the calendar grid.</summary>
public sealed record CalendarEvent(
    DateOnly Date,
    string Name,
    EventKind Kind = EventKind.PublicHoliday,
    FlagInstruction Flag = FlagInstruction.None,
    EventIcon Icon = EventIcon.None);
