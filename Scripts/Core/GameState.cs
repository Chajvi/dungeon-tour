using System.Collections.Generic;
using DungeonTour.Content;
using DungeonTour.Models;

namespace DungeonTour.Core;

public sealed class GameState
{
    public Player Player { get; set; } = default!;
    public List<Tourist> Tourists { get; set; } = new();
    public Inventory Inventory { get; set; } = default!;
    public int RoomsToExit { get; set; }
    public int RoomIndex { get; set; }
    public string DungeonId { get; set; } = "dungeon_01";

    public bool GameOver { get; set; }
    public bool Won { get; set; }

    public HashSet<string> SeenNpcEventIds { get; set; } = new();
    public HashSet<string> TriggeredSpecialComboIds { get; set; } = new();
    public bool FirstNpcEncounterHappened { get; set; } = false;

    public DescriptionDeck DescriptionDeck { get; set; } = default!;
    public int BonusDamageNextFight { get; set; }
    public bool HasKevinsScrapBlade { get; set; }
    public int NonFightStreak { get; set; }
    public int RestStreak { get; set; }
    public bool SeenMargo { get; set; }
    public bool SeenKevin { get; set; }
    public bool SeenPam { get; set; }
    public bool HasPamMembership { get; set; }
    public int PamRefusals { get; set; }
    public bool SnusEventUsed { get; set; }

    public bool SnusTouristLeft { get; set; }
    public int SnusTouristIndex { get; set; } = -1;
    public int RoomsSinceRest { get; set; }

    public int RngSeed { get; set; }

    public List<EncounterOption>? PendingRoomOptions { get; set; }
}