using System;
using System.Collections.Generic;
using System.Linq;
using DungeonTour.Models;

namespace DungeonTour.Content;

public static class DungeonRegistry
{
    private static readonly List<DungeonDefinition> _all = DungeonDatabase.GetAll();

    public static IReadOnlyList<DungeonDefinition> All => _all;

    public static DungeonDefinition Get(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return _all[0];

        var d = _all.FirstOrDefault(x =>
            x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        return d ?? _all[0];
    }

    public static bool Exists(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _all.Any(x =>
            x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }
}
