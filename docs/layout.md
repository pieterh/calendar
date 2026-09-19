# Page layout

The design follows the Kalender365 monthly sheet, minus the branding: no corner logo, no background watermark and no footer.

## Page

- A4 landscape, 841.89 × 595.28 pt, 20 pt margin on all sides.
- Fonts: Helvetica (falls back to the platform default sans-serif when not available).
- Everything sits in one white "card" with a thin light-grey border (`#d5d5d5`) and 12 pt rounded corners.

## Header

- 48 pt high bar across the full card width, vertical blue gradient `#2a8fdb` → `#0c5ea9`.
- Title centred, white, bold, 22 pt: month name in Dutch with a capital, then the year — `September 2026`.

## Grid

- 24 pt horizontal padding inside the card, 12 pt above the grid, 18 pt below.
- Columns: a 24 pt gutter for the ISO-8601 week number, then seven equal day columns.
- Day-name row (20 pt): `Maandag Dinsdag Woensdag Donderdag Vrijdag Zaterdag Zondag`, bold 9 pt, centred. The week starts on Monday.
- One row per week the month touches — four, five or six rows — sharing the remaining page height equally, so every month fills the page.
- Week number: 7 pt italic grey (`#666666`), vertically centred in the gutter.
- Every cell has a 0.5 pt border (`#dcdcdc`) and 4 pt padding. The day number is top-left in 8 pt. Cells before the 1st and after the last day are drawn empty.
- Saturday and Sunday columns are shaded `#efefef` for all rows, including empty cells.

## Events

- A day with a public event is filled `#cce4f7` (light blue) instead of white/grey.
- The event name is printed italic, 8 pt, bottom-right in the cell. Several events on one day are joined with `, `.

All values live in `src/Calendar/Rendering/CalendarTheme.cs`.
