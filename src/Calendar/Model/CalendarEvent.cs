namespace Calendar.Model;

/// <summary>What an event is. Only public holidays get the coloured cell fill.</summary>
public enum EventKind
{
    PublicHoliday,
    FlagDay,
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

/// <summary>A named single-day event shown in the calendar grid.</summary>
public sealed record CalendarEvent(
    DateOnly Date,
    string Name,
    EventKind Kind = EventKind.PublicHoliday,
    FlagInstruction Flag = FlagInstruction.None);
