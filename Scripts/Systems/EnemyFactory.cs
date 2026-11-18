using System;
using DungeonTour.Content;
using DungeonTour.Core;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class EnemyFactory
{
    public static Enemy CreateEnemy(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        DungeonDefinition dungeon = DungeonDatabase.GetById(state.DungeonId);
        if (dungeon.EnemyPool.Count == 0)
            dungeon = DungeonDatabase.GetAll()[0];

        EnemyDefinition picked = dungeon.EnemyPool[Rng.Instance.Next(dungeon.EnemyPool.Count)];

        int roomIndex = state.RoomIndex;
        int hp = dungeon.BaseEnemyHp + roomIndex * 2 + Rng.Instance.Next(0, 7);
        int dmg = dungeon.BaseEnemyDamage + Math.Min(6, roomIndex / 3) + Rng.Instance.Next(0, 3);

        return new Enemy(picked.Name, hp, dmg)
        {
            IntroText = picked.Intro
        };
    }

    public static Enemy CreateNpcEnemy(string npcName, int roomIndex)
    {
        int hp = 22 + roomIndex * 2 + Rng.Instance.Next(0, 6);
        int dmg = 6 + Math.Min(6, roomIndex / 3) + Rng.Instance.Next(0, 3);

        return new Enemy(npcName, hp, dmg)
        {
            IntroText = $"[i]{npcName} steps forward.[/i]"
        };
    }
}
