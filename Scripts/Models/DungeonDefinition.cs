using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonTour.Models;

public sealed class DungeonDefinition
{
    public string Id { get; }
    public string Name { get; }

    public int RoomsToExit { get; }

    public int BaseEnemyHp { get; }
    public int BaseEnemyDamage { get; }

    public List<EnemyDefinition> EnemyPool { get; } = new();

    public DungeonDefinition(
        string id,
        string name,
        int roomsToExit,
        int baseEnemyHp,
        int baseEnemyDamage)
    {
        Id = id;
        Name = name;
        RoomsToExit = roomsToExit;
        BaseEnemyHp = baseEnemyHp;
        BaseEnemyDamage = baseEnemyDamage;
    }
}
