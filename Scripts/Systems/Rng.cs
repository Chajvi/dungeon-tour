using System;

namespace DungeonTour.Systems;

public static class Rng
{
    public static Random Instance { get; private set; } = new Random(GenerateSeed());

    public static int CurrentSeed { get; private set; }

    public static void SetSeed(int seed)
    {
        CurrentSeed = seed;
        Instance = new Random(seed);
    }

    public static int GenerateSeed()
    {
        CurrentSeed = Guid.NewGuid().GetHashCode();
        return CurrentSeed;
    }
}