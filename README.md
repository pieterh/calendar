# calendar

A small .NET 10 command-line tool that generates a printable PDF calendar: A4 landscape, one month per page, ISO week numbers, shaded weekends, Dutch public holidays (fetched from the open [Nager.Date](https://date.nager.at) API; without network access the calendar is generated without holidays and a warning is printed), the official flag days and well-known observances such as Sinterklaas and the clock changes.

```sh
calendar                      # current month → ./calendar-202609.pdf
calendar 10 11 --open         # October and November, then open in the PDF viewer
calendar 2026-09..2027-01 --temp   # five pages, written to the temp folder and opened
```

## Build and run

Requires the .NET 10 SDK (`brew install --cask dotnet-sdk` on macOS; if a new shell cannot find `dotnet`, add `export PATH="$PATH:/usr/local/share/dotnet"` to `~/.zshrc`). From the repository root:

```sh
dotnet build                                          # build both projects
dotnet test                                           # run the unit tests
dotnet run --project src/Calendar -- 2026-09 --temp   # build and run the CLI; arguments go after --
```

To get a standalone `calendar` command you can put on your `PATH`:

```sh
dotnet publish src/Calendar -c Release -r osx-arm64 --self-contained false -p:PublishSingleFile=true -o dist
./dist/calendar --help
```

## Documentation

- [Usage](docs/usage.md) — arguments, options, exit codes
- [Page layout](docs/layout.md) — what the page looks like and why
- [Public events](docs/public-events.md) — where the holidays come from, offline behaviour, planned follow-up
- [Development](docs/development.md) — build, test, publish, project structure
