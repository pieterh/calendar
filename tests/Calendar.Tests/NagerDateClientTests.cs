using System.Net;
using System.Text;
using System.Text.Json;
using Calendar.Events;
using Calendar.Model;

namespace Calendar.Tests;

public class NagerDateClientTests
{
    private const string SampleJson = """
        [
          {"date":"2026-04-27","localName":"Koningsdag","name":"King's Day","countryCode":"NL","fixed":false,"global":true,"counties":null,"launchYear":null,"types":["Public"]},
          {"date":"2026-05-05","localName":"Bevrijdingsdag","name":"Liberation Day","countryCode":"NL","fixed":true,"global":true,"counties":null,"launchYear":null,"types":["School","Authorities"]}
        ]
        """;

    [Fact]
    public void MapsDateAndLocalName()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, SampleJson);

        var holidays = new NagerDateClient(new HttpClient(handler)).GetHolidays(2026);

        Assert.Equal(
            [new CalendarEvent(new DateOnly(2026, 4, 27), "Koningsdag"), new CalendarEvent(new DateOnly(2026, 5, 5), "Bevrijdingsdag")],
            holidays);
    }

    [Fact]
    public void RequestsPublicHolidaysForYearAndCountry()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[]");

        new NagerDateClient(new HttpClient(handler)).GetHolidays(2027);

        Assert.Equal(new Uri("https://date.nager.at/api/v3/PublicHolidays/2027/NL"), handler.LastRequestUri);
    }

    [Fact]
    public void NonSuccessStatus_Throws()
    {
        var client = new NagerDateClient(new HttpClient(new FakeHandler(HttpStatusCode.InternalServerError, "")));
        Assert.Throws<HttpRequestException>(() => client.GetHolidays(2026));
    }

    [Fact]
    public void MalformedJson_Throws()
    {
        var client = new NagerDateClient(new HttpClient(new FakeHandler(HttpStatusCode.OK, "<html>")));
        Assert.Throws<JsonException>(() => client.GetHolidays(2026));
    }

    private sealed class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(Send(request, cancellationToken));
    }
}
