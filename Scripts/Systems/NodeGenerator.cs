using System;
using System.Linq;
using System.Collections.Generic;
using DungeonTour.Core;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class NodeGenerator
{
    public static Node GenerateNode(GameState state)
    {
        int optionCount = Rng.Instance.Next(2, 4);
        var node = new Node();

        var pool = BuildWeightedPool(state);

        var pickedTypes = new List<EncounterType>();
        while (pickedTypes.Count < optionCount)
        {
            var t = pool[Rng.Instance.Next(pool.Count)];

            if (t == EncounterType.Merchant && pickedTypes.Contains(EncounterType.Merchant)) continue;
            if (t == EncounterType.Rest && pickedTypes.Contains(EncounterType.Rest)) continue;

            pickedTypes.Add(t);
        }

        // -------------------------
        // NEW: Never allow ALL options to be Fight.
        // If that happens, replace exactly 1 option with a random non-fight,
        // where NPC/Rest/Merchant are equally likely.
        // -------------------------
        bool allFight = pickedTypes.All(t => t == EncounterType.Fight);

        if (allFight && pickedTypes.Count > 0)
        {
            int replaceIndex = Rng.Instance.Next(pickedTypes.Count);

            int roll = Rng.Instance.Next(0, 3); // 0,1,2
            EncounterType replacement =
                roll == 0 ? EncounterType.Npc :
                roll == 1 ? EncounterType.Rest :
                EncounterType.Merchant;

            pickedTypes[replaceIndex] = replacement;
        }

        var usedDescriptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var t in pickedTypes)
        {
            string desc = DrawUniqueForNode(state, t, usedDescriptions);
            usedDescriptions.Add(desc);

            node.Options.Add(new EncounterOption(t, desc));
        }

        InjectSpecialNpcOptionIfPossible(state, node);

        return node;
    }

    private static void InjectSpecialNpcOptionIfPossible(GameState state, Node node)
    {
        int npcIndex = node.Options.FindIndex(o => o.Type == EncounterType.Npc);
        if (npcIndex == -1)
            return;

        var special = NpcSpecialCombos.GetAvailableSpecialComboForParty(state);
        if (special == null)
            return;

        node.Options[npcIndex] = new EncounterOption(
            type: EncounterType.Npc,
            description: special.OptionText,
            specialComboId: special.ComboId
        );
    }

    private static string DrawUniqueForNode(GameState state, EncounterType type, HashSet<string> used)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            string d = state.DescriptionDeck.Draw(type);
            if (!used.Contains(d))
                return d;
        }

        return state.DescriptionDeck.Draw(type);
    }

    private static List<EncounterType> BuildWeightedPool(GameState state)
    {
        int fightWeight = 8 + Math.Min(6, state.RoomIndex / 2);
        int npcWeight = 6;
        int restWeight = 3;
        int merchantWeight = 4;

        var pool = new List<EncounterType>();
        pool.AddRange(Enumerable.Repeat(EncounterType.Fight, fightWeight));
        pool.AddRange(Enumerable.Repeat(EncounterType.Npc, npcWeight));
        pool.AddRange(Enumerable.Repeat(EncounterType.Rest, restWeight));
        pool.AddRange(Enumerable.Repeat(EncounterType.Merchant, merchantWeight));

        return pool;
    }
}
