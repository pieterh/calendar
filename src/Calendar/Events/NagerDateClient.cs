using System.Globalization;
using System.Text.Json;
using Calendar.Model;

namespace Calendar.Events;

/// <summary>
/// Fetches public holidays for one year from the open Nager.Date API
/// (https://date.nager.at, no API key). Names are the local (Dutch) names.
/// </summary>
public sealed class NagerDateClient(HttpClient http)
{
    public const string CountryCode = "NL";
    public static readonly Uri BaseAddress = new("https://date.nager.at/api/v3/");

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>All holidays of <paramref name="year"/>, in the order the API returns them.</summary>
    /// <exception cref="HttpRequestException">The request failed or returned a non-success status.</exception>
    /// <exception cref="TaskCanceledException">The request timed out.</exception>
    /// <exception cref="JsonException">The response was not the expected JSON.</exception>
    public IReadOnlyList<CalendarEvent> GetHolidays(int year)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(BaseAddress, $"PublicHolidays/{year}/{CountryCode}"));
        using var response = http.Send(request);
        response.EnsureSuccessStatusCode();

        using var stream = response.Content.ReadAsStream();
        var holidays = JsonSerializer.Deserialize<List<NagerHoliday>>(stream, JsonOptions) ?? [];
        return holidays
            .Select(h => new CalendarEvent(DateOnly.ParseExact(h.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture), h.LocalName))
            .ToList();
    }

    // Only the fields we use; the API also returns name, countryCode, fixed, global, counties, launchYear, types.
    private sealed record NagerHoliday(string Date, string LocalName);
}
