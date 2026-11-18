namespace DungeonTour.Models;

public sealed class Player
{
    public string Name { get; }
    public int MaxHp { get; }
    public int Hp { get; set; }
    public int BaseDamage { get; set; }

    public Player(string name, int maxHp, int baseDamage)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        BaseDamage = baseDamage;
    }
}
