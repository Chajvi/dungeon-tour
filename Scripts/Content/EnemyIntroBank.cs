using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonTour.Content;

public static class EnemyIntroBank
{
    private static readonly Dictionary<string, string> _introByEnemyName =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // -------------------------
            // Dungeon 1 enemies (existing)
            // -------------------------
            {
                "Budget Minotaur",
                "It stomps forward wearing a paper crown.\nA sticky note reads: \"MINOTAUR (CHEAPER VERSION)\""
            },
            {
                "Unlicensed Skeleton",
                "It's holding a clipboard.\nIt does not have a permit for being alive."
            },
            {
                "Moist Goblin",
                "It crawls out of a puddle.\nIt is wet in a way that feels intentional."
            },
            {
                "Angry Decorative Vase",
                "It falls off a shelf.\nIt lands upright.\nIt is furious."
            },
            {
                "Tax-Evading Slime",
                "It oozes into view.\nIt hisses: \"I have never filed a single document.\""
            },
            {
                "Cursed Roomba (Ancient)",
                "The dusty machine awakens.\nIt begins vacuuming your soul.\nIts warranty expired 900 years ago."
            },
            {
                "Bat With Opinions",
                "It swoops down and immediately starts heckling you.\nIt has strong feelings about your leadership."
            },
            {
                "Malpractice Sorcerer",
                "They appear in a stained robe.\nThe robe says: \"TRUST ME, I'M CERTIFIED.\""
            },
            {
                "Microtransaction Imp",
                "It grins.\nA menu pops up in your mind:\n[b]PAY 4.99 GOLD TO UNLOCK ATTACKING[/b]"
            },
            {
                "Sentient Coupon Book",
                "It flutters open on its own.\nIt screams:\n[b]LIMITED TIME OFFER: YOUR DEATH[/b]"
            },
            {
                "Aggressively Normal Man",
                "He approach.\nHe looks like someone who works in accounting.\nHe is holding a brick with confidence."
            },
            {
                "Horrible Little Guy",
                "It's small, horrible and crawls out of the wall.\nYou can't explain why it makes you angry.\nBut it does."
            },
            {
                "Suspiciously Sharp Spoon",
                "It slides across the floor.\nIt is sharpened to a lethal edge.\nIt has chosen violence."
            },
            {
                "Seagull With A Gun Licence",
                "It lands.\nIt shows you a laminated gun licence.\nThen it screams like a car alarm."
            },
            {
                "Discount Demon",
                "It's definately a demon...but it looks... cheaper.\nIt whispers: \"I was on sale.\""
            },
            {
                "Wet Paper Golem",
                "It shuffles forward.\nIt is made of wet paper.\nIt is still somehow dangerous."
            },
            {
                "Haunted Tax Form",
                "It spins in the air.\nIt demands personal information.\nYou feel your spirit leaving your body."
            },
            {
                "Overdue Library Ghost",
                "It rises, holding a book.\nIt points at you.\n\"SHHHHH.\""
            },
            {
                "Violent Coat Rack",
                "It stands perfectly still.\nThen it sprints at you.\nYou hate that it can sprint."
            },
            {
                "Chair Mimic (IKEA Edition)",
                "The chair unfolds itself.\nIt has too many parts.\nIt lunges.\nSome assembly required."
            },

            // -------------------------
            // DungeonDatabase new enemies
            // (placeholders for now, but REAL structure)
            // -------------------------
            { "Grease Elemental", "It bubbles up from the floor.\nIt smells like liability." },
            { "Feral Hotdog", "It rolls forward.\nIt is not cooked.\nIt is angry." },
            { "Soda Fountain Spirit", "A hiss of carbonation echoes.\nThen it attacks." },
            { "Expired Pretzel Knight", "It raises a stale weapon.\nThe crunch is threatening." },
            { "Deep Fry Wraith", "It floats forward.\nIt radiates hot oil and regret." },

            { "Cursed Exhibit Guard", "It steps out of the darkness.\nIt demands you stop touching things." },
            { "Haunted Brochure Stand", "Pamphlets flutter like wings.\nYou feel judged." },
            { "The Audio Guide (Possessed)", "A calm voice whispers:\n\"Please proceed to your death.\""},
            { "Glass Case Mimic", "The display case opens.\nIt was never meant to." },

            { "Manager Spirit", "It appears with a clipboard.\nIt demands to speak to YOUR manager." },
            { "Complaint Elemental", "It manifests as pure dissatisfaction.\nIt is unstoppable." },
            { "Refund Golem", "It lumbers forward.\nIt wants your receipt." },
            { "Queue Demon", "It forms behind you.\nThe line is eternal." },
            { "Hold Music Phantom", "Soft jazz plays.\nYou begin to panic." },

            { "Scented Slime", "It oozes forward.\nIt smells like lavender lies." },
            { "Sauna Wraith", "The air gets hotter.\nSomething breathes behind you." },
            { "Towel Ghoul", "It approaches holding a damp towel.\nYou do not want it." },
            { "Essential Oil Warlock", "It raises a tiny bottle.\nYou feel your sinuses closing." },
            { "Cucumber Mask Horror", "It stares through slices of cucumber.\nIt knows your flaws." },

            { "Sentient Keychain Swarm", "Tiny souvenirs rattle.\nThey swarm like debt." },
            { "Souvenir Mimic", "It looks harmless.\nThen it opens." },
            { "Postcard Poltergeist", "Postcards fly at your face.\nEach one says: \"Wish you were dead\"." },
            { "Cash Register Gremlin", "It giggles.\nIt demands payment.\nYou don't know for what." },

            { "Tank Glass Horror", "Something presses against the glass.\nThen the glass presses back." },
            { "Eel With A Lawsuit", "It slithers forward.\nIt has legal representation." },
            { "Moist Goblin (Aquatic)", "It emerges soaking wet.\nIt is somehow wetter than before." },

            { "The Velvet Minotaur", "It steps forward.\nIt is luxurious.\nIt is still a minotaur." },
            { "Premium Skeleton", "It rattles confidently.\nIt has better bones than you." },
            { "Gold-Plated Slime", "It glitters.\nIt oozes with wealth and malice." },
            { "Influencer Demon", "It appears with perfect lighting.\nIt demands engagement." },
            { "Final Liability Beast", "The dungeon itself seems to breathe.\nThis is the fine print." },
        };

    public static string GetIntro(string enemyName)
    {
        if (string.IsNullOrWhiteSpace(enemyName))
            return "";

        if (_introByEnemyName.TryGetValue(enemyName, out var intro))
            return intro;

        return $"It appears.\nIt looks ready to ruin your day.";
    }
}
