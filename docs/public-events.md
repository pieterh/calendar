# Public events

Events are supplied by `IPublicEventProvider` (`src/Calendar/Events/`). The renderer asks the provider for the events of each month and highlights the matching cells (see [layout.md](layout.md)).

## Source: Nager.Date

Dutch public holidays are fetched at run time from the open [Nager.Date](https://date.nager.at) API, which needs no API key:

```
GET https://date.nager.at/api/v3/PublicHolidays/{year}/NL
```

`NagerDateClient` calls this once for every distinct year on the calendar and maps each entry's `date` and `localName` (the Dutch name) to a `CalendarEvent`. Everything the API returns is shown, including days that are not statutory days off for everyone (Goede Vrijdag, Bevrijdingsdag). For 2026 that is:

| Date | Event |
|---|---|
| 01-01 | Nieuwjaarsdag |
| 04-03 | Goede Vrijdag |
| 04-05 / 04-06 | Eerste / Tweede Paasdag |
| 04-27 | Koningsdag |
| 05-05 | Bevrijdingsdag |
| 05-14 | Hemelvaartsdag |
| 05-24 / 05-25 | Eerste / Tweede Pinksterdag |
| 12-25 / 12-26 | Eerste / Tweede Kerstdag |

The fetched events are wrapped in a `PublicEventList`, an in-memory provider, so the PDF renderer itself never touches the network.

### When the API is unreachable

There is no cache. If a year cannot be fetched (offline, timeout after 10 s, API down, unexpected response) the tool prints `warning: could not fetch public holidays for <year>: …` to stderr and still generates the calendar; that year then has no highlighted days. The exit code stays 0.

### Privacy

The request contains only the year and the country code. `date.nager.at` is served through Cloudflare (from the Amsterdam edge in the Netherlands); the origin's location is not published. The project is MIT-licensed and can be self-hosted with `docker run -p 80:8080 nager/nager-date` if that matters — change `NagerDateClient.BaseAddress` to point at it.

## Follow-up

Possible next steps, without changing the renderer:

1. **Custom events** from a config file (e.g. `~/.config/calendar/events.toml` or a `--events <file>` option), merged with the public ones through a composite provider. Prinsjesdag (third Tuesday of September) is not a holiday and not in the API; it could be added as a local rule the same way.
2. Optional `--no-events` flag and a way to select a country (`NagerDateClient.CountryCode` is currently fixed to `NL`).
3. Non-holiday observances: Moederdag, Vaderdag, Sinterklaas, begin/end of daylight saving time.
