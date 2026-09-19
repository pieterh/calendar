# Public events

Events are supplied by `IPublicEventProvider` (`src/Calendar/Events/`). The renderer asks the provider for the events of each month and highlights the matching cells (see [layout.md](layout.md)).

## Current state

`DutchPublicEvents` contains a fixed list with a single entry:

| Date | Event |
|---|---|
| 2026-09-15 | Prinsjesdag |

No other holidays are shown yet.

## Follow-up

Planned for a next iteration, without changing the renderer:

1. **Computed Dutch holidays** in `DutchPublicEvents` for any year:
   - fixed dates: Nieuwjaarsdag (1 Jan), Koningsdag (27 Apr, or 26 Apr when the 27th is a Sunday), Bevrijdingsdag (5 May), Eerste/Tweede Kerstdag (25/26 Dec)
   - Easter-based: Goede Vrijdag, Eerste/Tweede Paasdag, Hemelvaartsdag, Eerste/Tweede Pinksterdag
   - Prinsjesdag: third Tuesday of September
   - possibly non-holiday observances: Moederdag, Vaderdag, Sinterklaas, begin/end of daylight saving time
2. **Custom events** from a config file (e.g. `~/.config/calendar/events.toml` or a `--events <file>` option), merged with the public ones through a composite provider.
3. Optional `--no-events` flag and a way to select a country/culture.
