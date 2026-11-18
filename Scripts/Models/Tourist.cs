namespace DungeonTour.Models;

public sealed class Tourist
{
    public string Name { get; set; }
    public TouristTrait Trait { get; set; }

    public bool Alive { get; set; } = true;
    public bool Gone { get; set; }
    public int Morale { get; set; } = 60;

    public Tourist(string name, TouristTrait trait)
    {
        Name = name;
        Trait = trait;
    }
}