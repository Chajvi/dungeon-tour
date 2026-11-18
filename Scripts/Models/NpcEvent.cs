using System.Collections.Generic;
namespace DungeonTour.Models;

public sealed class NpcEvent
{
    public string Id { get; }
    public string Title { get; }
    public string Text { get; }

    public List<NpcChoice> Choices { get; } = new();

    public NpcEvent(string id, string title, string text)
    {
        Id = id;
        Title = title;
        Text = text;
    }
}
