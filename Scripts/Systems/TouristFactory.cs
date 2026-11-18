using System.Linq;
using System.Collections.Generic;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class TouristFactory
{
    private static readonly List<Tourist> TouristPool = new()
    {
        new Tourist("Bertha", TouristTrait.PassiveAggressive),
        new Tourist("Merle", TouristTrait.ConspiracyTheorist),
        new Tourist("Keith", TouristTrait.KnowItAll),
        new Tourist("Brenda", TouristTrait.Crybaby),
        new Tourist("Chad", TouristTrait.AdrenalineJunkie),
        new Tourist("Sophie", TouristTrait.Influencer),
        new Tourist("Gunnar", TouristTrait.Spiritual),
        new Tourist("Penny", TouristTrait.Reckless),

        new Tourist("Agnes", TouristTrait.PassiveAggressive),
        new Tourist("Ralph", TouristTrait.KnowItAll),
        new Tourist("Stella", TouristTrait.Crybaby),
        new Tourist("Vince", TouristTrait.Reckless),
        new Tourist("Freya", TouristTrait.Spiritual),
        new Tourist("Mallory", TouristTrait.Influencer),
        new Tourist("Otto", TouristTrait.ConspiracyTheorist),
        new Tourist("Jasmine", TouristTrait.AdrenalineJunkie),

        new Tourist("Derek", TouristTrait.KnowItAll),
        new Tourist("Nova", TouristTrait.Influencer),
        new Tourist("Kyle", TouristTrait.Reckless),
        new Tourist("Beatrice", TouristTrait.PassiveAggressive),
        new Tourist("Marcus", TouristTrait.AdrenalineJunkie),
        new Tourist("Tiffany", TouristTrait.Crybaby),
        new Tourist("Oscar", TouristTrait.ConspiracyTheorist),
        new Tourist("Iris", TouristTrait.Spiritual),

        new Tourist("Eleanor", TouristTrait.PassiveAggressive),
        new Tourist("Wendy", TouristTrait.Crybaby),
        new Tourist("Harold", TouristTrait.KnowItAll),
        new Tourist("Gloria", TouristTrait.Spiritual),
        new Tourist("Dwayne", TouristTrait.Reckless),
        new Tourist("Sasha", TouristTrait.Influencer),
    };

    public static List<Tourist> CreateParty(int count)
    {
        var freshPool = TouristPool
            .Select(t => new Tourist(t.Name, t.Trait))
            .ToList();

        var party = new List<Tourist>();

        for (int i = 0; i < count; i++)
        {
            if (freshPool.Count == 0)
                break;

            int idx = Rng.Instance.Next(freshPool.Count);
            party.Add(freshPool[idx]);
            freshPool.RemoveAt(idx);
        }

        return party;
    }
}