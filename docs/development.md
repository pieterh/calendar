# Development

## Prerequisites

- .NET 10 SDK (`brew install --cask dotnet-sdk` on macOS)

## Project structure

```
calendar.sln
src/Calendar/                 the CLI (assembly name: calendar)
  Program.cs                  System.CommandLine definition, entry point
  Cli/                        CalendarApp (orchestration), MonthArgumentParser, OverwritePrompt
  Model/                      YearMonth, MonthGrid (week rows, ISO weeks), CalendarEvent, DateRules (nth weekday, Easter)
  Events/                     IPublicEventProvider, NagerDateClient (API fetch), DutchFlagDays and DutchObservances (static tables), PublicEventList
  Rendering/                  CalendarTheme (all sizes/colours), CalendarDocument (QuestPDF)
  Output/                     OutputPathResolver (file names), PdfOpener (default viewer)
tests/Calendar.Tests/         xUnit tests for parsing, grid layout, file names and events
docs/                         this documentation
```

Dependencies: [QuestPDF](https://www.questpdf.com/) (Community licence) for PDF generation, [System.CommandLine](https://learn.microsoft.com/dotnet/standard/commandline/) for argument parsing.

## Build, test, run

```sh
dotnet build
dotnet test
dotnet run --project src/Calendar -- 2026-09 --temp
```

## Publish a single executable

```sh
dotnet publish src/Calendar -c Release -r osx-arm64 --self-contained false -p:PublishSingleFile=true -o dist
./dist/calendar --help
```

Use `-r osx-x64`, `linux-x64` or `win-x64` for other platforms. Copy `dist/calendar` somewhere on your `PATH`.

## Conventions

- `TreatWarningsAsErrors` is on for both projects.
- The PDF layout has no automated tests; check it visually with `--temp` after changing `Rendering/`.
- Layout constants belong in `CalendarTheme`, not inline in `CalendarDocument`.
