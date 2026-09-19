# Usage

```
calendar [<months>...] [-o|--output <file>] [--open] [--temp] [-f|--force]
```

## Months

Zero or more month arguments; each page of the PDF is one month. With no argument the current month is used.

| Form | Meaning | Example (today = September 2026) |
|---|---|---|
| `yyyy-MM` | That exact month | `2026-09` → September 2026, `2025-12` → December 2025 |
| `M` or `MM` | The next occurrence of that month: this year if it has not passed yet, otherwise next year | `9` → September 2026, `10` → October 2026, `1` → January 2027 |
| `a..b` | Inclusive range; each end uses the rules above. If a bare-number end would fall before the start it rolls into the next year | `9..12` → Sep–Dec 2026, `9..2` → Sep 2026–Feb 2027, `2026-11..2027-02` |

Arguments may be mixed and repeated; the result is sorted and de-duplicated, e.g. `calendar 1 2026-10 9` gives September 2026, October 2026, January 2027.

## Options

| Option | Description |
|---|---|
| `-o, --output <file>` | Write to this file (relative to the current directory). Cannot be combined with `--temp`. |
| `--open` | After writing, open the PDF with the default application (`open` on macOS, `xdg-open` on Linux, shell association on Windows). |
| `--temp` | Write to the system temp folder instead of the current directory and open it. Handy for print-and-forget: the folder is cleaned up by the OS. Implies `--open`. |
| `-f, --force` | Overwrite an existing file without asking. |
| `-h, --help` | Show help. |
| `--version` | Show version. |

## Output file name

Without `--output` the file is named:

- one month: `calendar-yyyyMM.pdf`, e.g. `calendar-202609.pdf`
- several months: `calendar-<first>-<last>.pdf`, e.g. `calendar-202609-202612.pdf`

## Overwriting

If the target file already exists the tool asks `File '…' exists. Overwrite? [y/N]` before generating anything. Answering anything other than `y`/`yes` cancels (exit code 1). When stdin is not a terminal the answer is always no; use `--force` in scripts.

## Exit codes

| Code | Meaning |
|---|---|
| 0 | PDF written (and opened, if requested) |
| 1 | Cancelled at the overwrite prompt, or the file could not be written/opened |
| 2 | Usage error: invalid month argument, `--output` together with `--temp`, unknown option |

## Examples

```sh
calendar                          # September 2026 → ./calendar-202609.pdf
calendar 2027-01 -o january.pdf   # explicit name
calendar 10..12 --open            # Q4 2026, three pages, opened afterwards
calendar 1..6 --temp              # Jan–Jun 2027 to the temp folder, opened, nothing to clean up
calendar 9 -f                     # overwrite ./calendar-202609.pdf without asking
```
