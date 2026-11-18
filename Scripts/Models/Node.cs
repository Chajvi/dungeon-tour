using System.Collections.Generic;
namespace DungeonTour.Models;

public sealed class Node
{
    public List<EncounterOption> Options { get; } = new();
}
