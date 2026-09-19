# Public events

Events are supplied by `IPublicEventProvider` (`src/Calendar/Events/`). The renderer asks the provider for the events of each month and highlights the matching cells (see [layout.md](layout.md)). There are two sources: public holidays fetched from Nager.Date, and the Dutch flag days from a static table. `CalendarApp.LoadPublicEvents` merges both per year into one `PublicEventList`.

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

## Source: vlaginstructie (flag days)

The days on which the Dutch flag is flown are not holidays and not in the API. They follow the official *vlaginstructie* for government buildings, which citizens may use as a guideline; it is a fixed list that changes only rarely, so `DutchFlagDays` holds it as a static table (`DutchFlagDays.ForYear(year)`) and nothing is fetched. Sources:

- [Rijksoverheid — Wanneer kan ik de vlag uithangen?](https://www.rijksoverheid.nl/vraag-en-antwoord/grondwet-en-statuut/wanneer-kan-ik-de-vlag-uithangen-en-wat-is-de-vlaginstructie)
- [Het Koninklijk Huis — Vlaginstructie](https://www.koninklijkhuis.nl/onderwerpen/vlaggen-en-vlaginstructie/vlaginstructie)

| Date | Alternative | Occasion | Flag |
|---|---|---|---|
| 31 Jan | 1 Feb | Verjaardag prinses Beatrix | oranje wimpel |
| 27 Apr | 26 Apr | Koningsdag | oranje wimpel |
| 4 May | – | Dodenherdenking | halfstok (18:00 until sunset) |
| 5 May | – | Bevrijdingsdag | |
| 17 May | 18 May | Verjaardag koningin Máxima | oranje wimpel |
| last Saturday of June | – | Veteranendag | |
| 15 Aug | 16 Aug | Formeel einde Tweede Wereldoorlog | |
| third Tuesday of September | – | Prinsjesdag (vlag alleen in Den Haag) | |
| 7 Dec | 8 Dec | Verjaardag prinses Amalia (prinses van Oranje) | oranje wimpel |
| 15 Dec | 16 Dec | Koninkrijksdag | |

The alternative date applies only when the date falls on a Sunday or a generally recognised Christian holiday (Goede Vrijdag, Pasen, Hemelvaartsdag, Pinksteren, Kerst). In practice that means Sundays, plus 17 May, which can coincide with Hemelvaartsdag or Tweede Pinksterdag (e.g. 2027). Easter is computed locally in `DateRules.EasterSunday`, so flag days are correct even when the API is unreachable. The instruction flags Prinsjesdag only in Den Haag; the day is kept on the calendar with that restriction in its name.

Each flag day is a `CalendarEvent` with `Kind = FlagDay` and a `FlagInstruction` (`Full`, `WithPennant`, `HalfMast`). Koningsdag and Bevrijdingsdag are also returned by Nager.Date; both events are kept, and `DayCell.Names` prints the name once. Such a day is drawn with the holiday fill *and* the flag marker.

## Follow-up

Possible next steps, without changing the renderer:

1. **Custom events** from a config file (e.g. `~/.config/calendar/events.toml` or a `--events <file>` option), merged with the public ones through a composite provider.
2. Optional `--no-events` flag and a way to select a country (`NagerDateClient.CountryCode` is currently fixed to `NL`).
3. Non-holiday observances: Moederdag, Vaderdag, Sinterklaas, begin/end of daylight saving time.
