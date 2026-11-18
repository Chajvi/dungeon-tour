namespace DungeonTour.Core;

public static class ConsoleShim
{
    private static string _currentLine = "";

    public static void WriteLine(string text = "")
    {
        if (_currentLine.Length > 0)
        {
            Ui.LogLine(_currentLine + text);
            _currentLine = "";
            return;
        }

        Ui.LogLine(text);
    }

    public static void Write(string text)
    {
        _currentLine += text;
    }

    public static void Clear()
    {
        _currentLine = "";
        Ui.ClearLog();
    }

    public static void ReadKey(bool intercept = false)
    {
    }

    public static string? ReadLine()
    {
        return null;
    }
}
