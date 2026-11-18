using System;
namespace DungeonTour.Systems;

public static class Damage
{
    public static int Roll(int baseDamage, int variance)
    {
        return Math.Max(0, baseDamage + Rng.Instance.Next(-variance, variance + 1));
    }
}
