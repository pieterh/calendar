namespace Calendar.Model;

/// <summary>A named single-day event shown in the calendar grid.</summary>
public sealed record CalendarEvent(DateOnly Date, string Name);
