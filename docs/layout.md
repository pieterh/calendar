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

- A day with a public holiday is filled `#cce4f7` (light blue) instead of white/grey. Flag days and observances (Sinterklaas, Dierendag, …) do not change the fill.
- Event names are printed italic, 8 pt, right-aligned, bottom-right in the cell. Several events on one day are joined with `, `; a day that is both a public holiday and a flag day under the same name (Koningsdag, Bevrijdingsdag) is named once.

## Flag days

- A flag day gets a 12 × 8 pt tricolour (`#AE1C28`, white, `#21468B`, with a 0.3 pt `#dcdcdc` outline) in the top-right corner of the cell.
- On days with an orange wimpel (Koningsdag and the royal birthdays) a 2 pt `#FF7F00` strip sits directly above the flag.
- On 4 May (Dodenherdenking) the flag is drawn one flag-height (8 pt) lower: halfstok.

## Clock change

- The switch to summer time (last Sunday of March) and to winter time (last Sunday of October) gets a 12 × 12 pt clock icon in the top-right corner: a clock face with the hands at 12 and 3 and a curved arrow over it — clockwise for summer time, mirrored (counter-clockwise) for winter time. It is drawn as inline SVG in the text colour with a 0.9 pt stroke, so no icon font is needed.
- The icon shares the corner with the flag: if a day ever had both, the flag comes first and the icon sits 2 pt to its right.

All values live in `src/Calendar/Rendering/CalendarTheme.cs`.
