using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonTour.Core;

public static class RunConfig
{
    public static string SelectedDungeonId { get; set; } = "dungeon_01";

    // When loading a saved run we place the loaded GameState here, then GameShell will consume it.
    public static GameState? LoadedState { get; set; }
}