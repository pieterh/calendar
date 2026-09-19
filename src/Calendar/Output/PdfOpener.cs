using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Calendar.Output;

/// <summary>Opens a file with the platform's default application.</summary>
public static class PdfOpener
{
    public static void Open(string path)
    {
        ProcessStartInfo startInfo;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            startInfo = new ProcessStartInfo("open", [path]);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            startInfo = new ProcessStartInfo(path) { UseShellExecute = true };
        }
        else
        {
            startInfo = new ProcessStartInfo("xdg-open", [path]);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not start a viewer for '{path}'.");
    }
}
