namespace DungeonTour.Models;

public sealed class Enemy
{
    public string Name { get; }
    public int MaxHp { get; }
    public int Hp { get; set; }
    public int BaseDamage { get; }
    public string IntroText { get; set; }

    public Enemy(string name, int hp, int baseDamage)
    {
        Name = name;
        MaxHp = hp;
        Hp = hp;
        BaseDamage = baseDamage;
    }
}
