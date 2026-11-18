using System.Collections.Generic;
using DungeonTour.Models;

namespace DungeonTour.Content;

public static class DungeonDatabase
{
    public static List<DungeonDefinition> GetAll()
    {
        return new List<DungeonDefinition>
        {
            new DungeonDefinition(
                id: "dungeon_01",
                name: "The Welcome Tour",
                roomsToExit: 12,
                baseEnemyHp: 18,
                baseEnemyDamage: 5
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Budget Minotaur",
                        "It stomps forward wearing a paper crown.\nA sticky note reads: \"MINOTAUR (CHEAPER VERSION)\""
                    ),
                    new EnemyDefinition(
                        "Unlicensed Skeleton",
                        "It's holding a clipboard.\nIt does not have a permit for being alive."
                    ),
                    new EnemyDefinition(
                        "Moist Goblin",
                        "It crawls out of a puddle.\nIt is wet in a way that feels intentional."
                    ),
                    new EnemyDefinition(
                        "Angry Decorative Vase",
                        "It falls off a shelf.\nIt lands upright.\nIt is furious."
                    ),
                    new EnemyDefinition(
                        "Tax-Evading Slime",
                        "It oozes into view.\nIt hisses: \"I have never filed a single document.\""
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_02",
                name: "The Food Court Catacombs",
                roomsToExit: 12,
                baseEnemyHp: 20,
                baseEnemyDamage: 6
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Grease Elemental",
                        "It bubbles up from the floor.\nIt smells like fries and poor decisions."
                    ),
                    new EnemyDefinition(
                        "Feral Hotdog",
                        "It skitters forward.\nIt is technically food.\nBut it is also angry."
                    ),
                    new EnemyDefinition(
                        "Soda Fountain Spirit",
                        "A hiss fills the corridor.\nCarbonation becomes a threat."
                    ),
                    new EnemyDefinition(
                        "Expired Pretzel Knight",
                        "It stands in stale glory.\nSalt flakes fall like snow.\nIt raises its twisted blade."
                    ),
                    new EnemyDefinition(
                        "Deep Fry Wraith",
                        "A heat shimmer crawls toward you.\nIt whispers:\n\"CRISPY...\""
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_03",
                name: "The Museum of Regret",
                roomsToExit: 12,
                baseEnemyHp: 20,
                baseEnemyDamage: 6
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Overdue Library Ghost",
                        "It rises, holding a book.\nIt points at you.\n\"SHHHHH.\""
                    ),
                    new EnemyDefinition(
                        "Cursed Exhibit Guard",
                        "It steps out of the shadows.\nIts badge reads: \"VOLUNTEER.\""
                    ),
                    new EnemyDefinition(
                        "Haunted Brochure Stand",
                        "Pamphlets flutter like wings.\nThe stand creaks:\n\"TAKE ONE.\""
                    ),
                    new EnemyDefinition(
                        "The Audio Guide (Possessed)",
                        "A small device clicks on.\nA voice whispers:\n\"You are not supposed to be here.\""
                    ),
                    new EnemyDefinition(
                        "Glass Case Mimic",
                        "The display case cracks open.\nSomething inside smiles."
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_04",
                name: "The Customer Service Labyrinth",
                roomsToExit: 12,
                baseEnemyHp: 22,
                baseEnemyDamage: 7
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Manager Spirit",
                        "It materializes in a blazer.\nIt radiates corporate rage."
                    ),
                    new EnemyDefinition(
                        "Complaint Elemental",
                        "It forms from hundreds of whispered grievances.\nIt demands escalation."
                    ),
                    new EnemyDefinition(
                        "Refund Golem",
                        "It lumbers forward.\nIts chest is filled with receipts."
                    ),
                    new EnemyDefinition(
                        "Queue Demon",
                        "It stretches infinitely.\nIt blocks the corridor like a line that never ends."
                    ),
                    new EnemyDefinition(
                        "Hold Music Phantom",
                        "A melody begins.\nIt is cheerful.\nIt is evil."
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_05",
                name: "The Spa of Eternal Moisture",
                roomsToExit: 12,
                baseEnemyHp: 22,
                baseEnemyDamage: 7
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Scented Slime",
                        "It slides toward you.\nIt smells like lavender.\nIt should not smell like lavender."
                    ),
                    new EnemyDefinition(
                        "Sauna Wraith",
                        "Steam pours from the corridor.\nA figure appears inside it.\nSweating is mandatory."
                    ),
                    new EnemyDefinition(
                        "Towel Ghoul",
                        "It shambles forward.\nIt is wrapped in towels like armor.\nThey are damp."
                    ),
                    new EnemyDefinition(
                        "Essential Oil Warlock",
                        "It raises a tiny bottle.\nThe scent hits your soul.\nYou feel judged."
                    ),
                    new EnemyDefinition(
                        "Cucumber Mask Horror",
                        "It emerges with slices on its face.\nIt whispers:\n\"SELF CARE.\""
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_06",
                name: "The Gift Shop Crypt",
                roomsToExit: 12,
                baseEnemyHp: 24,
                baseEnemyDamage: 8
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Sentient Keychain Swarm",
                        "Tiny metal rings clatter.\nThey move like insects.\nThey want your pockets."
                    ),
                    new EnemyDefinition(
                        "Souvenir Mimic",
                        "It looks like a harmless trinket.\nThen it opens its mouth."
                    ),
                    new EnemyDefinition(
                        "Postcard Poltergeist",
                        "Postcards spin in the air.\nEach one shows your death from a different angle."
                    ),
                    new EnemyDefinition(
                        "Discount Demon",
                        "It's definitely a demon...but it looks... cheaper.\nIt whispers: \"I was on sale.\""
                    ),
                    new EnemyDefinition(
                        "Cash Register Gremlin",
                        "It crawls onto the counter.\nIt slams the drawer shut.\n\"NO REFUNDS.\""
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_07",
                name: "The Aquarium of Bad Decisions",
                roomsToExit: 12,
                baseEnemyHp: 24,
                baseEnemyDamage: 8
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "Seagull With A Gun Licence",
                        "It lands.\nIt shows you a laminated gun licence.\nThen it screams like a car alarm."
                    ),
                    new EnemyDefinition(
                        "Wet Paper Golem",
                        "It shuffles forward.\nIt is made of wet paper.\nIt is still somehow dangerous."
                    ),
                    new EnemyDefinition(
                        "Tank Glass Horror",
                        "A crack spreads across the glass.\nSomething inside taps politely.\nThen harder."
                    ),
                    new EnemyDefinition(
                        "Eel With A Lawsuit",
                        "It slithers into view.\nIt carries legal intent.\nAnd electricity."
                    ),
                    new EnemyDefinition(
                        "Moist Goblin (Aquatic)",
                        "It crawls out of a tank.\nIt is wetter than the original.\nThat feels impossible."
                    ),
                }
            },

            new DungeonDefinition(
                id: "dungeon_08",
                name: "The VIP Attraction",
                roomsToExit: 12,
                baseEnemyHp: 26,
                baseEnemyDamage: 9
            )
            {
                EnemyPool =
                {
                    new EnemyDefinition(
                        "The Velvet Minotaur",
                        "It steps forward.\nIt is wearing velvet.\nIt is expensive.\nIt is still a minotaur."
                    ),
                    new EnemyDefinition(
                        "Premium Skeleton",
                        "It rattles confidently.\nIt looks like it has a subscription."
                    ),
                    new EnemyDefinition(
                        "Gold-Plated Slime",
                        "It oozes forward.\nIt sparkles.\nIt is capitalism given form."
                    ),
                    new EnemyDefinition(
                        "Influencer Demon",
                        "It appears with a ring light.\nIt whispers:\n\"LIKE AND SUBSCRIBE.\""
                    ),
                    new EnemyDefinition(
                        "Final Liability Beast",
                        "It emerges like a final boss.\nSomewhere, a waiver signs itself."
                    ),
                }
            },
        };
    }

    public static DungeonDefinition GetById(string id)
    {
        foreach (var d in GetAll())
        {
            if (d.Id == id)
                return d;
        }

        return GetAll()[0];
    }
}