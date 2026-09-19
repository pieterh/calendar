using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Calendar.Rendering;

/// <summary>Colours, fonts and measurements (in points) for the calendar page. See docs/layout.md.</summary>
public static class CalendarTheme
{
    public static readonly PageSize PageSize = PageSizes.A4.Landscape();
    public const float PageMargin = 20;

    public const string FontFamily = "Helvetica";
    public const float TitleFontSize = 22;
    public const float DayNameFontSize = 9;
    public const float DayNumberFontSize = 8;
    public const float WeekNumberFontSize = 7;
    public const float EventFontSize = 8;

    public const float CardCornerRadius = 12;
    public const float CardBorderWidth = 0.75f;
    public const float HeaderHeight = 48;
    public const float GridPaddingHorizontal = 24;
    public const float GridPaddingTop = 12;
    public const float GridPaddingBottom = 18;
    public const float WeekColumnWidth = 24;
    public const float DayNameRowHeight = 20;
    public const float CellPadding = 4;
    public const float CellBorderWidth = 0.5f;

    // Flag-day marker: a 3:2 tricolour top-right in the cell, with an orange strip above it on wimpel days.
    public const float FlagWidth = 12;
    public const float FlagHeight = 8;
    public const float PennantHeight = 2;
    public const float FlagBorderWidth = 0.3f; // outlines the white band on a white cell

    public static readonly Color HeaderTop = Color.FromHex("#2a8fdb");
    public static readonly Color HeaderBottom = Color.FromHex("#0c5ea9");
    public static readonly Color Title = Colors.White;
    public static readonly Color CardBorder = Color.FromHex("#d5d5d5");
    public static readonly Color CardBackground = Colors.White;
    public static readonly Color CellBorder = Color.FromHex("#dcdcdc");
    public static readonly Color WeekdayBackground = Colors.White;
    public static readonly Color WeekendBackground = Color.FromHex("#efefef");
    public static readonly Color EventBackground = Color.FromHex("#cce4f7");
    public static readonly Color Text = Color.FromHex("#222222");
    public static readonly Color MutedText = Color.FromHex("#666666");
    public static readonly Color FlagRed = Color.FromHex("#AE1C28");
    public static readonly Color FlagWhite = Colors.White;
    public static readonly Color FlagBlue = Color.FromHex("#21468B");
    public static readonly Color FlagOrange = Color.FromHex("#FF7F00");
}
