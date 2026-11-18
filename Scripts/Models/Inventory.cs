namespace DungeonTour.Models;

public sealed class Inventory
{
    public int Potions { get; set; }
    public int Gold { get; set; }

    public bool HasTouristMap { get; set; }

    public Inventory(int potions, int gold)
    {
        Potions = potions;
        Gold = gold;
    }
}
