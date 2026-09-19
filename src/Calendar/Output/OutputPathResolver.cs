using Calendar.Model;

namespace Calendar.Output;

/// <summary>Decides where the PDF is written.</summary>
public static class OutputPathResolver
{
    public const string Prefix = "calendar-";
    public const string Extension = ".pdf";

    /// <summary>
    /// <c>calendar-yyyyMM.pdf</c> for a single month, <c>calendar-yyyyMM-yyyyMM.pdf</c> (first-last) for several.
    /// </summary>
    public static string DefaultFileName(IReadOnlyList<YearMonth> months)
    {
        ArgumentOutOfRangeException.ThrowIfZero(months.Count);
        var first = months[0];
        var last = months[^1];
        return first == last
            ? $"{Prefix}{first}{Extension}"
            : $"{Prefix}{first}-{last}{Extension}";
    }

    /// <param name="explicitPath">Value of <c>--output</c>, or null.</param>
    /// <param name="useTemp">Whether <c>--temp</c> was given.</param>
    /// <param name="currentDirectory">Directory used for the default name.</param>
    /// <param name="tempDirectory">Directory used with <c>--temp</c>.</param>
    public static string Resolve(
        IReadOnlyList<YearMonth> months,
        string? explicitPath,
        bool useTemp,
        string currentDirectory,
        string tempDirectory)
    {
        if (explicitPath is not null && useTemp)
        {
            throw new ArgumentException("--output and --temp cannot be combined.");
        }

        if (explicitPath is not null)
        {
            return Path.GetFullPath(explicitPath, currentDirectory);
        }

        var directory = useTemp ? tempDirectory : currentDirectory;
        return Path.Combine(directory, DefaultFileName(months));
    }
}
