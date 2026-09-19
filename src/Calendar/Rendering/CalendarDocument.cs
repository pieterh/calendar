using System.Globalization;
using Calendar.Events;
using Calendar.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Calendar.Rendering;

/// <summary>Renders one A4-landscape page per month.</summary>
public sealed class CalendarDocument(
    IReadOnlyList<YearMonth> months,
    IPublicEventProvider events,
    CultureInfo culture) : IDocument
{
    public DocumentMetadata GetMetadata() => new()
    {
        Title = months.Count == 1 ? $"Kalender {MonthTitle(months[0])}" : "Kalender",
        Creator = "calendar",
        Producer = "calendar (QuestPDF)",
    };

    public void Compose(IDocumentContainer container)
    {
        foreach (var month in months)
        {
            var grid = new MonthGrid(month, events.GetEvents(month));
            container.Page(page =>
            {
                page.Size(CalendarTheme.PageSize);
                page.Margin(CalendarTheme.PageMargin);
                page.DefaultTextStyle(style => style
                    .FontFamily(CalendarTheme.FontFamily)
                    .FontColor(CalendarTheme.Text));
                page.Content().Element(c => ComposeCard(c, grid));
            });
        }
    }

    private void ComposeCard(IContainer container, MonthGrid grid)
    {
        container
            .Background(CalendarTheme.CardBackground)
            .Border(CalendarTheme.CardBorderWidth)
            .BorderColor(CalendarTheme.CardBorder)
            .CornerRadius(CalendarTheme.CardCornerRadius)
            .Column(column =>
            {
                column.Item().Element(c => ComposeHeader(c, grid.Month));
                column.Item()
                    .PaddingHorizontal(CalendarTheme.GridPaddingHorizontal)
                    .PaddingTop(CalendarTheme.GridPaddingTop)
                    .PaddingBottom(CalendarTheme.GridPaddingBottom)
                    .Element(c => ComposeGrid(c, grid));
            });
    }

    private void ComposeHeader(IContainer container, YearMonth month)
    {
        container
            .Height(CalendarTheme.HeaderHeight)
            .BackgroundLinearGradient(90, [CalendarTheme.HeaderTop, CalendarTheme.HeaderBottom])
            .AlignCenter()
            .AlignMiddle()
            .Text(MonthTitle(month))
            .FontSize(CalendarTheme.TitleFontSize)
            .Bold()
            .FontColor(CalendarTheme.Title);
    }

    private void ComposeGrid(IContainer container, MonthGrid grid)
    {
        // The page has a fixed height, so the row height is computed rather than stretched.
        var available = CalendarTheme.PageSize.Height
                        - 2 * CalendarTheme.PageMargin
                        - 2 * CalendarTheme.CardBorderWidth
                        - CalendarTheme.HeaderHeight
                        - CalendarTheme.GridPaddingTop
                        - CalendarTheme.GridPaddingBottom
                        - CalendarTheme.DayNameRowHeight;
        var rowHeight = available / grid.Rows.Count;

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(CalendarTheme.WeekColumnWidth);
                for (var i = 0; i < MonthGrid.DaysPerWeek; i++)
                {
                    columns.RelativeColumn();
                }
            });

            // Day-name row.
            table.Cell().Height(CalendarTheme.DayNameRowHeight);
            for (var i = 0; i < MonthGrid.DaysPerWeek; i++)
            {
                var dayOfWeek = (DayOfWeek)((i + 1) % MonthGrid.DaysPerWeek); // Monday first
                table.Cell()
                    .Height(CalendarTheme.DayNameRowHeight)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(Capitalize(culture.DateTimeFormat.GetDayName(dayOfWeek)))
                    .FontSize(CalendarTheme.DayNameFontSize)
                    .Bold();
            }

            foreach (var row in grid.Rows)
            {
                table.Cell()
                    .Height(rowHeight)
                    .AlignMiddle()
                    .AlignCenter()
                    .Text(row.IsoWeek.ToString(CultureInfo.InvariantCulture))
                    .FontSize(CalendarTheme.WeekNumberFontSize)
                    .Italic()
                    .FontColor(CalendarTheme.MutedText);

                for (var i = 0; i < row.Days.Count; i++)
                {
                    var isWeekend = i >= 5;
                    table.Cell().Height(rowHeight).Element(c => ComposeDayCell(c, row.Days[i], isWeekend));
                }
            }
        });
    }

    private static void ComposeDayCell(IContainer container, DayCell cell, bool isWeekend)
    {
        var background = cell.Events.Count > 0 ? CalendarTheme.EventBackground
            : isWeekend ? CalendarTheme.WeekendBackground
            : CalendarTheme.WeekdayBackground;

        container
            .Background(background)
            .Border(CalendarTheme.CellBorderWidth)
            .BorderColor(CalendarTheme.CellBorder)
            .Padding(CalendarTheme.CellPadding)
            .Layers(layers =>
            {
                layers.PrimaryLayer()
                    .Text(cell.DayNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
                    .FontSize(CalendarTheme.DayNumberFontSize);

                if (cell.Events.Count > 0)
                {
                    layers.Layer()
                        .AlignBottom()
                        .AlignRight()
                        .Text(string.Join(", ", cell.Events.Select(e => e.Name)))
                        .FontSize(CalendarTheme.EventFontSize)
                        .Italic();
                }
            });
    }

    private string MonthTitle(YearMonth month) =>
        $"{Capitalize(culture.DateTimeFormat.GetMonthName(month.Month))} {month.Year}";

    private string Capitalize(string text) =>
        text.Length == 0 ? text : string.Concat(text[..1].ToUpper(culture), text[1..]);
}
