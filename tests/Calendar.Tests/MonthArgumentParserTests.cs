using Calendar.Cli;
using Calendar.Model;

namespace Calendar.Tests;

public class MonthArgumentParserTests
{
    // All tests run "today" = 19 September 2026.
    private static readonly MonthArgumentParser Parser = new(new DateOnly(2026, 9, 19));

    private static YearMonth[] Parse(params string[] args) => Parser.Parse(args).ToArray();

    [Fact]
    public void NoArguments_YieldsCurrentMonth() =>
        Assert.Equal([new YearMonth(2026, 9)], Parse());

    [Theory]
    [InlineData("2026-09", 2026, 9)]
    [InlineData("2027-01", 2027, 1)]
    [InlineData("2025-12", 2025, 12)] // explicit past months are allowed
    public void YearMonth_IsParsedLiterally(string text, int year, int month) =>
        Assert.Equal([new YearMonth(year, month)], Parse(text));

    [Theory]
    [InlineData("9", 2026, 9)]   // current month → this year
    [InlineData("09", 2026, 9)]
    [InlineData("10", 2026, 10)] // later this year
    [InlineData("12", 2026, 12)]
    [InlineData("1", 2027, 1)]   // already passed → next year
    [InlineData("01", 2027, 1)]
    [InlineData("8", 2027, 8)]
    public void BareMonth_ResolvesToNextOccurrence(string text, int year, int month) =>
        Assert.Equal([new YearMonth(year, month)], Parse(text));

    [Fact]
    public void Range_OfYearMonths_IsInclusive() =>
        Assert.Equal(
            [new(2026, 11), new(2026, 12), new(2027, 1), new(2027, 2)],
            Parse("2026-11..2027-02"));

    [Fact]
    public void Range_OfBareMonths_RollsEndIntoNextYear() =>
        Assert.Equal(
            [new(2026, 9), new(2026, 10), new(2026, 11), new(2026, 12), new(2027, 1), new(2027, 2)],
            Parse("9..2"));

    [Fact]
    public void Range_SingleMonth_IsAllowed() =>
        Assert.Equal([new YearMonth(2026, 10)], Parse("10..10"));

    [Fact]
    public void Range_MixedForms_IsAllowed() =>
        Assert.Equal([new(2026, 9), new(2026, 10), new(2026, 11)], Parse("2026-09..11"));

    [Fact]
    public void Range_ExplicitEndBeforeStart_Throws() =>
        Assert.Throws<FormatException>(() => Parse("2026-12..2026-09"));

    [Fact]
    public void List_IsSortedAndDeduplicated() =>
        Assert.Equal(
            [new(2026, 9), new(2026, 10), new(2027, 1)],
            Parse("1", "2026-10", "9", "2026-09"));

    [Theory]
    [InlineData("0")]
    [InlineData("13")]
    [InlineData("2026")]
    [InlineData("2026-13")]
    [InlineData("2026-9")]
    [InlineData("sep")]
    [InlineData("2026-09..")]
    [InlineData("..2026-09")]
    [InlineData("")]
    [InlineData("--bogus")]
    [InlineData("-x")]
    public void Invalid_Throws(string text) =>
        Assert.Throws<FormatException>(() => Parse(text));
}
