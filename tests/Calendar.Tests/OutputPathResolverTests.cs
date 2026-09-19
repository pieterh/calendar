using Calendar.Model;
using Calendar.Output;

namespace Calendar.Tests;

public class OutputPathResolverTests
{
    private static readonly string Cwd = Path.Combine(Path.GetTempPath(), "cwd");
    private static readonly string Temp = Path.Combine(Path.GetTempPath(), "tmp");

    [Fact]
    public void SingleMonth_DefaultName() =>
        Assert.Equal("calendar-202609.pdf", OutputPathResolver.DefaultFileName([new(2026, 9)]));

    [Fact]
    public void MultipleMonths_FirstAndLast() =>
        Assert.Equal(
            "calendar-202609-202612.pdf",
            OutputPathResolver.DefaultFileName([new(2026, 9), new(2026, 10), new(2026, 11), new(2026, 12)]));

    [Fact]
    public void Default_GoesToCurrentDirectory() =>
        Assert.Equal(
            Path.Combine(Cwd, "calendar-202609.pdf"),
            OutputPathResolver.Resolve([new(2026, 9)], null, false, Cwd, Temp));

    [Fact]
    public void Temp_GoesToTempDirectory() =>
        Assert.Equal(
            Path.Combine(Temp, "calendar-202609.pdf"),
            OutputPathResolver.Resolve([new(2026, 9)], null, true, Cwd, Temp));

    [Fact]
    public void ExplicitRelativePath_IsResolvedAgainstCurrentDirectory() =>
        Assert.Equal(
            Path.Combine(Cwd, "out", "x.pdf"),
            OutputPathResolver.Resolve([new(2026, 9)], Path.Combine("out", "x.pdf"), false, Cwd, Temp));

    [Fact]
    public void ExplicitAbsolutePath_IsKept()
    {
        var absolute = Path.Combine(Path.GetTempPath(), "elsewhere", "x.pdf");
        Assert.Equal(absolute, OutputPathResolver.Resolve([new(2026, 9)], absolute, false, Cwd, Temp));
    }

    [Fact]
    public void OutputAndTemp_Conflict() =>
        Assert.Throws<ArgumentException>(() =>
            OutputPathResolver.Resolve([new(2026, 9)], "x.pdf", true, Cwd, Temp));
}
