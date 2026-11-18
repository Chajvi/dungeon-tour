using System.Linq;
using System.Collections.Generic;
using DungeonTour.Core;
using DungeonTour.Models;
using DungeonTour.Systems;

namespace DungeonTour.Content;

public sealed class DescriptionDeck
{
    private readonly Queue<string> _fight = new();
    private readonly Queue<string> _npc = new();
    private readonly Queue<string> _rest = new();
    private readonly Queue<string> _merchant = new();

    public DescriptionDeck()
        : this(null, null, null, null)
    {
    }

    // New ctor: accept explicit deck contents (preserves order when loading)
    public DescriptionDeck(IEnumerable<string>? fight, IEnumerable<string>? npc, IEnumerable<string>? rest, IEnumerable<string>? merchant)
    {
        if (fight != null && fight.Any())
        {
            foreach (var s in fight) _fight.Enqueue(s);
        }
        else
        {
            FillDeck(_fight, DescriptionBank.FightDescriptions);
        }

        if (npc != null && npc.Any())
        {
            foreach (var s in npc) _npc.Enqueue(s);
        }
        else
        {
            FillDeck(_npc, DescriptionBank.NpcDescriptions);
        }

        if (rest != null && rest.Any())
        {
            foreach (var s in rest) _rest.Enqueue(s);
        }
        else
        {
            FillDeck(_rest, DescriptionBank.RestDescriptions);
        }

        if (merchant != null && merchant.Any())
        {
            foreach (var s in merchant) _merchant.Enqueue(s);
        }
        else
        {
            FillDeck(_merchant, DescriptionBank.MerchantDescriptions);
        }
    }

    public string Draw(EncounterType type)
    {
        return type switch
        {
            EncounterType.Fight => DrawFrom(_fight, DescriptionBank.FightDescriptions),
            EncounterType.Npc => DrawFrom(_npc, DescriptionBank.NpcDescriptions),
            EncounterType.Rest => DrawFrom(_rest, DescriptionBank.RestDescriptions),
            EncounterType.Merchant => DrawFrom(_merchant, DescriptionBank.MerchantDescriptions),
            _ => "A corridor. Something is wrong. (Probably.)"
        };
    }

    // Expose current queue contents so SaveManager can persist them
    public List<string> GetFightDeck() => _fight.ToList();
    public List<string> GetNpcDeck() => _npc.ToList();
    public List<string> GetRestDeck() => _rest.ToList();
    public List<string> GetMerchantDeck() => _merchant.ToList();

    private static string DrawFrom(Queue<string> deck, IReadOnlyList<string> fallbackList)
    {
        if (deck.Count > 0)
            return deck.Dequeue();

        return fallbackList[Rng.Instance.Next(fallbackList.Count)];
    }

    private static void FillDeck(Queue<string> deck, IReadOnlyList<string> source)
    {
        var list = source.ToList();
        Shuffle(list);

        foreach (var s in list)
            deck.Enqueue(s);
    }

    private static void Shuffle(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Rng.Instance.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
