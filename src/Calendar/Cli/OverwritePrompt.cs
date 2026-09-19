namespace Calendar.Cli;

/// <summary>Asks the user whether an existing file may be overwritten.</summary>
public static class OverwritePrompt
{
    /// <returns><c>true</c> if the user answered yes. Without an interactive console the answer is no.</returns>
    public static bool Confirm(string path, TextWriter output, TextReader input)
    {
        if (Console.IsInputRedirected)
        {
            output.WriteLine($"File '{path}' exists. Use --force to overwrite.");
            return false;
        }

        output.Write($"File '{path}' exists. Overwrite? [y/N] ");
        var answer = input.ReadLine()?.Trim();
        return answer is not null && (answer.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                                      answer.Equals("yes", StringComparison.OrdinalIgnoreCase));
    }
}
