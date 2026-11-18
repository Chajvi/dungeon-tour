using System.Collections.Generic;

namespace DungeonTour.Content;

public static class DescriptionBank
{
    public static IReadOnlyList<string> FightDescriptions { get; } = new List<string>
    {
        "You hear movement ahead. It does not sound friendly.",
        "Something steps into your path with hostile intent.",
        "The corridor suddenly feels like an arena.",
        "A presence approaches. Your tourists start whispering prayers.",
        "A shape blocks the way. It wants violence.",
        "You hear claws on stone. Then silence. Then breathing.",
        "A shadow peels itself off the wall and advances.",
        "The dungeon decides it is time for a performance review.",
        "A figure emerges. It looks ready to argue with your skeleton.",
        "Something is waiting for you, like it paid rent here.",
        "The air shifts. Something is about to lunge.",
        "Your group steps forward and immediately regrets it.",
        "You notice too late that you're not alone in this hallway.",
        "A hostile sound echoes closer. Your tourists stop smiling.",
        "A sudden impact in the dark. Something charges.",
        "A low growl rolls through the corridor like thunder.",
        "You catch a glimpse of teeth. Too many teeth.",
        "A violent silhouette steps into view.",
        "Something appears and makes it very clear this is not a negotiation.",
        "The dungeon offers you a mandatory combat experience.",
        "A threat reveals itself, as if on cue.",
        "You feel watched. Then attacked.",
        "Something rushes you before you can finish thinking.",
        "The corridor narrows. There is only one way forward: fight.",
        "A hostile presence makes the first move."
    };

    public static IReadOnlyList<string> NpcDescriptions { get; } = new List<string>
    {
        "Someone is already here, waiting as if they expected you.",
        "A figure stands by the path, strangely calm.",
        "You hear a voice from ahead. It sounds confident.",
        "A small scene has been set up in the corridor: a person, a prop, a problem.",
        "A stranger steps into view with the energy of a conversation you can't escape.",
        "You notice a presence that doesn't feel immediately violent.",
        "A silhouette gestures at you, like you're late for an appointment.",
        "Someone offers you attention. That is rarely free in a dungeon.",
        "A figure emerges with something to say, sell, or demand.",
        "A stranger blocks your way — not with a weapon, but with audacity.",
        "You see someone sitting in the dark like this is normal.",
        "A voice calls out. Your tourists freeze.",
        "A person steps forward with the confidence of bureaucracy.",
        "You sense a social encounter forming against your will.",
        "Someone is here, and they are about to involve you in their situation.",
        "A figure waves. You do not trust it.",
        "A calm presence invites you closer. That feels wrong.",
        "A stranger watches your group like they're choosing a target.",
        "You spot a person who looks far too comfortable down here.",
        "A figure offers you something: a deal, a story, or a trap.",
        "A voice speaks your direction. It sounds like trouble.",
        "You stumble into someone else's moment.",
        "A stranger appears with the confidence of a scripted event.",
        "Someone approaches like they have authority over this hallway.",
        "You encounter a figure who clearly has an agenda."
    };

    public static IReadOnlyList<string> RestDescriptions { get; } = new List<string>
    {
        "You find a pocket of quiet where nothing immediately attacks you.",
        "A small area looks safe enough to breathe for a moment.",
        "The dungeon goes still, as if giving you permission to recover.",
        "You find a space that feels less hostile than the rest of the corridor.",
        "A rare calm settles over the group.",
        "You discover a place where your tourists stop panicking for a moment.",
        "The air is warmer here. It feels almost kind.",
        "A resting spot appears like the dungeon is mocking you.",
        "You find somewhere to sit without being bitten (probably).",
        "A quiet corner invites you to regroup.",
        "You stumble into a spot that feels like a break room for the doomed.",
        "You find a place where the dungeon seems to ignore you.",
        "The corridor opens into a calm area with no immediate threats.",
        "A strange sense of safety settles over the group.",
        "You find a small space that feels like a temporary truce."
    };

    public static IReadOnlyList<string> MerchantDescriptions { get; } = new List<string>
    {
        "You spot a small stall set up where no stall should exist.",
        "Someone is selling something in the corridor like this is normal.",
        "A figure waves at you with the confidence of a salesman.",
        "A merchant presence appears, as if summoned by capitalism.",
        "You notice a makeshift shop tucked into the shadows.",
        "A cart stands ahead. Someone is waiting beside it.",
        "A vendor has set up shop in the most inappropriate location imaginable.",
        "A suspicious business opportunity presents itself.",
        "You see a stand with goods laid out like bait.",
        "A seller watches your group with hungry optimism.",
        "A shopkeeper smiles like they know your wallet personally.",
        "You find someone offering supplies. The prices feel cursed.",
        "A merchant-like figure gestures you closer.",
        "A stall appears with the energy of a scam, but also convenience.",
        "You encounter someone who looks ready to trade."
    };
}
