using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using DungeonTour.Models;
using Godot;
using DungeonTour.Content;
using DungeonTour.Systems;

namespace DungeonTour.Core;

public static class SaveManager
{
    private const string SaveFileRelative = "user://save.json";

    private static string GetSavePath()
        => ProjectSettings.GlobalizePath(SaveFileRelative);

    public static bool Save(GameState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        try
        {
            // Capture current seed before serializing
            state.RngSeed = Rng.CurrentSeed;

            var dto = ToDto(state);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(dto, options);
            File.WriteAllText(GetSavePath(), json);
            GD.Print("[SaveManager] Game saved.");
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SaveManager] Save failed: {ex}");
            return false;
        }
    }

    public static GameState? Load()
    {
        try
        {
            string path = GetSavePath();
            if (!File.Exists(path))
            {
                GD.Print("[SaveManager] No save file found.");
                return null;
            }

            string json = File.ReadAllText(path);

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            var dto = JsonSerializer.Deserialize<SaveData>(json, options);
            if (dto == null)
            {
                GD.PrintErr("[SaveManager] Failed to deserialize save data.");
                return null;
            }

            var state = FromDto(dto);

            // Restore the RNG seed so encounters are deterministic
            Rng.SetSeed(state.RngSeed);

            GD.Print("[SaveManager] Game loaded.");
            return state;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SaveManager] Load failed: {ex}");
            return null;
        }
    }

    public static bool SaveExists()
    {
        try
        {
            return File.Exists(GetSavePath());
        }
        catch
        {
            return false;
        }
    }

    public static bool DeleteSave()
    {
        try
        {
            string path = GetSavePath();
            if (File.Exists(path))
                File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[SaveManager] Delete failed: {ex}");
            return false;
        }
    }

    // -------------------------
    // DTO mapping
    // -------------------------
    private static SaveData ToDto(GameState s)
    {
        return new SaveData
        {
            DungeonId = s.DungeonId,
            RoomsToExit = s.RoomsToExit,
            RoomIndex = s.RoomIndex,
            GameOver = s.GameOver,
            Won = s.Won,
            RngSeed = s.RngSeed,

            Player = new PlayerDto
            {
                Name = s.Player.Name,
                MaxHp = s.Player.MaxHp,
                Hp = s.Player.Hp,
                BaseDamage = s.Player.BaseDamage
            },

            Inventory = new InventoryDto
            {
                Potions = s.Inventory.Potions,
                Gold = s.Inventory.Gold,
                HasTouristMap = s.Inventory.HasTouristMap
            },

            Tourists = s.Tourists.ConvertAll(t => new TouristDto
            {
                Name = t.Name,
                Trait = t.Trait,
                Alive = t.Alive,
                Gone = t.Gone,
                Morale = t.Morale
            }),

            SeenNpcEventIds = new List<string>(s.SeenNpcEventIds),
            TriggeredSpecialComboIds = new List<string>(s.TriggeredSpecialComboIds),
            FirstNpcEncounterHappened = s.FirstNpcEncounterHappened,

            BonusDamageNextFight = s.BonusDamageNextFight,
            HasKevinsScrapBlade = s.HasKevinsScrapBlade,
            NonFightStreak = s.NonFightStreak,
            RestStreak = s.RestStreak,
            SeenMargo = s.SeenMargo,
            SeenKevin = s.SeenKevin,
            SeenPam = s.SeenPam,
            HasPamMembership = s.HasPamMembership,
            PamRefusals = s.PamRefusals,
            SnusEventUsed = s.SnusEventUsed,
            SnusTouristLeft = s.SnusTouristLeft,
            SnusTouristIndex = s.SnusTouristIndex,
            RoomsSinceRest = s.RoomsSinceRest,

            // Persist current deck contents so loads resume exactly
            DescriptionDeckFight = s.DescriptionDeck?.GetFightDeck() ?? new List<string>(),
            DescriptionDeckNpc = s.DescriptionDeck?.GetNpcDeck() ?? new List<string>(),
            DescriptionDeckRest = s.DescriptionDeck?.GetRestDeck() ?? new List<string>(),
            DescriptionDeckMerchant = s.DescriptionDeck?.GetMerchantDeck() ?? new List<string>(),

            // Persist the room options the player was looking at
            PendingRoomOptions = s.PendingRoomOptions?.ConvertAll(o => new EncounterOptionDto
            {
                Type = o.Type,
                Description = o.Description,
                SpecialComboId = o.SpecialComboId
            }) ?? new List<EncounterOptionDto>()
        };
    }

    private static GameState FromDto(SaveData dto)
    {
        var s = new GameState();

        s.DungeonId = dto.DungeonId;
        s.RoomsToExit = dto.RoomsToExit;
        s.RoomIndex = dto.RoomIndex;
        s.GameOver = dto.GameOver;
        s.Won = dto.Won;
        s.RngSeed = dto.RngSeed;

        s.Player = new Player(dto.Player.Name, dto.Player.MaxHp, dto.Player.BaseDamage)
        {
            Hp = dto.Player.Hp
        };

        s.Inventory = new Inventory(dto.Inventory.Potions, dto.Inventory.Gold)
        {
            HasTouristMap = dto.Inventory.HasTouristMap
        };

        s.Tourists = new List<Tourist>();
        foreach (var t in dto.Tourists)
        {
            var tour = new Tourist(t.Name, t.Trait)
            {
                Alive = t.Alive,
                Gone = t.Gone,
                Morale = t.Morale
            };
            s.Tourists.Add(tour);
        }

        s.SeenNpcEventIds = dto.SeenNpcEventIds != null ? new HashSet<string>(dto.SeenNpcEventIds) : new HashSet<string>();
        s.TriggeredSpecialComboIds = dto.TriggeredSpecialComboIds != null ? new HashSet<string>(dto.TriggeredSpecialComboIds) : new HashSet<string>();
        s.FirstNpcEncounterHappened = dto.FirstNpcEncounterHappened;

        s.BonusDamageNextFight = dto.BonusDamageNextFight;
        s.HasKevinsScrapBlade = dto.HasKevinsScrapBlade;
        s.NonFightStreak = dto.NonFightStreak;
        s.RestStreak = dto.RestStreak;
        s.SeenMargo = dto.SeenMargo;
        s.SeenKevin = dto.SeenKevin;
        s.SeenPam = dto.SeenPam;
        s.HasPamMembership = dto.HasPamMembership;
        s.PamRefusals = dto.PamRefusals;
        s.SnusEventUsed = dto.SnusEventUsed;
        s.SnusTouristLeft = dto.SnusTouristLeft;
        s.SnusTouristIndex = dto.SnusTouristIndex;
        s.RoomsSinceRest = dto.RoomsSinceRest;

        // recreate transient/deck data using persisted order if available
        s.DescriptionDeck = new DescriptionDeck(
            dto.DescriptionDeckFight ?? new List<string>(),
            dto.DescriptionDeckNpc ?? new List<string>(),
            dto.DescriptionDeckRest ?? new List<string>(),
            dto.DescriptionDeckMerchant ?? new List<string>());

        // Restore pending room options
        if (dto.PendingRoomOptions != null && dto.PendingRoomOptions.Count > 0)
        {
            s.PendingRoomOptions = dto.PendingRoomOptions.ConvertAll(o =>
                new EncounterOption(o.Type, o.Description, o.SpecialComboId));
        }

        return s;
    }

    // -------------------------
    // DTO classes
    // -------------------------
    private sealed class SaveData
    {
        public string DungeonId { get; set; } = "dungeon_01";
        public int RoomsToExit { get; set; }
        public int RoomIndex { get; set; }
        public bool GameOver { get; set; }
        public bool Won { get; set; }
        public int RngSeed { get; set; }

        public PlayerDto Player { get; set; } = new();
        public InventoryDto Inventory { get; set; } = new();
        public List<TouristDto> Tourists { get; set; } = new();

        public List<string> SeenNpcEventIds { get; set; } = new();
        public List<string> TriggeredSpecialComboIds { get; set; } = new();
        public bool FirstNpcEncounterHappened { get; set; }

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
        public int SnusTouristIndex { get; set; }
        public int RoomsSinceRest { get; set; }

        // Persisted deck contents (queue order)
        public List<string> DescriptionDeckFight { get; set; } = new();
        public List<string> DescriptionDeckNpc { get; set; } = new();
        public List<string> DescriptionDeckRest { get; set; } = new();
        public List<string> DescriptionDeckMerchant { get; set; } = new();

        // Persisted room options
        public List<EncounterOptionDto> PendingRoomOptions { get; set; } = new();
    }

    private sealed class PlayerDto
    {
        public string Name { get; set; } = "Guide";
        public int MaxHp { get; set; }
        public int Hp { get; set; }
        public int BaseDamage { get; set; }
    }

    private sealed class InventoryDto
    {
        public int Potions { get; set; }
        public int Gold { get; set; }
        public bool HasTouristMap { get; set; }
    }

    private sealed class TouristDto
    {
        public string Name { get; set; } = "";
        public TouristTrait Trait { get; set; }
        public bool Alive { get; set; }
        public bool Gone { get; set; }
        public int Morale { get; set; }
    }

    private sealed class EncounterOptionDto
    {
        public EncounterType Type { get; set; }
        public string Description { get; set; } = "";
        public string? SpecialComboId { get; set; }
    }
}