using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DungeonTour.Core;

public interface IUiBridge
{
    void LogLine(string text);
    void ClearLog();

    void SetHud(object state);

    Task<int> WaitForChoiceAsync(IReadOnlyList<string> options);
    Task WaitForContinueAsync(string continueText = "Continue");

    // NEW: screen shake
    void HitShake();
}

public static class Ui
{
    private static IUiBridge? _bridge;

    public static void Bind(IUiBridge bridge)
    {
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
    }

    private static IUiBridge Bridge =>
        _bridge ?? throw new InvalidOperationException(
            "Ui is not bound. Call Ui.Bind(...) from GameShell._Ready() before using Ui."
        );

    // -------------------------
    // UI output helpers
    // -------------------------
    public static void LogLine(string text) => Bridge.LogLine(text);
    public static void ClearLog() => Bridge.ClearLog();

    // -------------------------
    // NEW: hit shake
    // -------------------------
    public static void HitShake() => Bridge.HitShake();

    // -------------------------
    // Old console-like API, now async
    // -------------------------
    public static Task Pause(string continueText = "Continue")
        => Bridge.WaitForContinueAsync(continueText);

    public static void Hud(object state)
        => Bridge.SetHud(state);

    public static async Task<int> ReadIntInRange(int min, int max)
    {
        if (max < min) throw new ArgumentOutOfRangeException(nameof(max), "max must be >= min");

        var options = new List<string>();
        for (int i = min; i <= max; i++)
            options.Add(i.ToString());

        int idx = await Bridge.WaitForChoiceAsync(options);
        return min + idx;
    }

    public static Task<int> ReadLetterChoiceIndex(int optionCount)
    {
        if (optionCount < 1 || optionCount > 4)
            throw new ArgumentOutOfRangeException(nameof(optionCount), "optionCount must be 1..4.");

        var options = new List<string>();
        for (int i = 0; i < optionCount; i++)
            options.Add(((char)('A' + i)).ToString());

        return Bridge.WaitForChoiceAsync(options);
    }


    public static Task<int> ReadChoiceIndex(int optionCount)
    {
        if (optionCount < 1 || optionCount > 4)
            throw new ArgumentOutOfRangeException(nameof(optionCount), "optionCount must be 1..4.");

        var options = new List<string>();
        for (int i = 0; i < optionCount; i++)
            options.Add(((char)('A' + i)).ToString());

        return Bridge.WaitForChoiceAsync(options);
    }

    public static void PrintTitle() { }
    public static void PrintEnding(object _ = null!) { }
}
