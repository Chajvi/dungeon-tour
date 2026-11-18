using System;
using DungeonTour.Core;

namespace DungeonTour.Models;

public sealed class NpcChoice
{
    public string Text { get; }

    public Func<GameState, string> Resolve { get; }

    public NpcChoice(string text, Func<GameState, string> resolve)
    {
        Text = text;
        Resolve = resolve;
    }
}
