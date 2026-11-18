using System;
using System.Linq;
using System.Collections.Generic;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class NpcEventDatabase
{
    public static List<NpcEvent> GetAllEvents()
    {
        return new List<NpcEvent>
        {
            Event_MerthrudeBakery(),
            Event_PermitGoblin(),
            Event_GhostReceptionist(),
            Event_InfluencerRitual(),
            Event_TheWall(),
            Event_SuspiciousCoupon()
        };
    }

    private static NpcEvent Event_MerthrudeBakery()
    {
        var e = new NpcEvent(
            id: "npc_merthrude_bakery",
            title: "An elderly woman sits calmly beside a tray of cinnamon buns.",
            text: "\"Hello dear,\" she says.\n\n\"Would you like a bun?\""
        );

        e.Choices.Add(new NpcChoice(
            "Accept a bun politely",
            state =>
            {
                state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + 6);
                return "You eat the bun.\n\nIt's unbelievably good. (+6 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Ask what she's doing in a dungeon",
            state =>
            {
                return "\"Oh,\" she says. \"I'm here for closure.\"\n\nYou do not ask follow-up questions.";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Buy the whole tray (10 gold)",
            state =>
            {
                if (state.Inventory.Gold < 10)
                    return "You check your wallet.\n\nYou cannot afford the buns. This is your lowest moment.";

                state.Inventory.Gold -= 10;
                state.Inventory.Potions++;
                return "You buy the tray.\n\nShe gives you a bun-shaped potion. (+1 Potion)";
            }
        ));

        return e;
    }

    private static NpcEvent Event_PermitGoblin()
    {
        var e = new NpcEvent(
            id: "permit_goblin",
            title: "A goblin in a tiny vest blocks your path.",
            text: "\"Hello. Do you have a permit for your tour group?\"\n\nHe holds a clipboard that radiates authority."
        );

        e.Choices.Add(new NpcChoice(
            "Pay the permit fee (10 gold)",
            state =>
            {
                if (state.Inventory.Gold < 10)
                    return "You reach for gold. You have none. The goblin nods sadly, like a disappointed tax agency.";

                state.Inventory.Gold -= 10;
                return "You pay 10 gold. The goblin stamps your forehead with 'APPROVED'.";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Lie confidently",
            state =>
            {
                int roll = Rng.Instance.Next(1, 101);
                if (roll <= 55)
                {
                    state.Inventory.Gold += 8;
                    return "You lie with the confidence of a man who has never read a policy.\n\nThe goblin panics and pays YOU 8 gold as an apology.";
                }

                state.Player.Hp -= 6;
                return "You lie. The goblin narrows his eyes.\n\nHe hits you with the clipboard. (-6 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Run past him while yelling 'TEAM BUILDING!'",
            state =>
            {
                LoseRandomTourist(state);
                return "You sprint past.\n\nOne tourist gets left behind. They are now employed.";
            }
        ));

        return e;
    }

    private static NpcEvent Event_GhostReceptionist()
    {
        var e = new NpcEvent(
            id: "ghost_receptionist",
            title: "A ghost receptionist floats behind a desk.",
            text: "\"Welcome to the dungeon! Would you like to upgrade your experience?\""
        );

        e.Choices.Add(new NpcChoice(
            "Accept the 'Premium Tour' upgrade",
            state =>
            {
                state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + 10);
                return "The ghost hands you a complimentary towel.\n\nYou feel refreshed. (+10 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Ask if the upgrade includes insurance",
            state =>
            {
                state.Player.Hp -= 4;
                return "The ghost laughs.\n\nYou are slapped by a cold breeze. (-4 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Steal the pen on the desk",
            state =>
            {
                int roll = Rng.Instance.Next(1, 101);
                if (roll <= 50)
                {
                    state.Inventory.Potions++;
                    return "You steal the pen.\n\nIt turns out the pen is a potion. (+1 Potion)";
                }

                LoseRandomTourist(state);
                return "You steal the pen.\n\nA tourist steals the stapler. The dungeon claims them instantly.";
            }
        ));

        return e;
    }

    private static NpcEvent Event_InfluencerRitual()
    {
        var e = new NpcEvent(
            id: "influencer_ritual",
            title: "A stranger is filming a dungeon vlog.",
            text: "\"Hey guys, welcome back to my channel. Today we're doing a PRANK on a cursed altar.\""
        );

        e.Choices.Add(new NpcChoice(
            "Join the vlog",
            state =>
            {
                state.Inventory.Gold += 12;
                return "You do a forced smile and say 'Like and subscribe'.\n\nYou gain 12 gold from sponsorship.";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Stop them before they trigger something",
            state =>
            {
                int roll = Rng.Instance.Next(1, 101);
                if (roll <= 65)
                {
                    return "You stop them.\n\nFor once, nothing gets worse. A rare victory.";
                }

                state.Player.Hp -= 8;
                return "You try to stop them.\n\nThe altar explodes socially. (-8 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Encourage them to prank the altar",
            state =>
            {
                LoseRandomTourist(state);
                state.Inventory.Gold += 5;
                return "The altar is pranked.\n\nA tourist is lost, but you gain 5 gold in ad revenue.";
            }
        ));

        return e;
    }

    private static NpcEvent Event_TheWall()
    {
        var e = new NpcEvent(
            id: "the_wall",
            title: "Another tour guide is arguing with a wall.",
            text: "\"It moved. I swear it moved.\"\n\nThe wall stands there with confidence."
        );

        e.Choices.Add(new NpcChoice(
            "Agree that the wall moved",
            state =>
            {
                state.Inventory.Potions++;
                return "The guide feels validated and gives you a potion.\n\n(+1 Potion)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Tell them the wall is innocent",
            state =>
            {
                return "The guide whispers: \"That's what it wants you to think.\"\n\nYou leave quickly.";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Punch the wall to prove a point",
            state =>
            {
                state.Player.Hp -= 7;
                return "You punch the wall.\n\nThe wall wins. (-7 HP)";
            }
        ));

        return e;
    }

    private static NpcEvent Event_SuspiciousCoupon()
    {
        var e = new NpcEvent(
            id: "suspicious_coupon",
            title: "A hooded stranger offers you a coupon.",
            text: "\"This coupon gives you 50% off your next tragedy.\""
        );

        e.Choices.Add(new NpcChoice(
            "Accept the coupon",
            state =>
            {
                int roll = Rng.Instance.Next(1, 101);
                if (roll <= 50)
                {
                    state.Inventory.Gold += 10;
                    return "You accept the coupon.\n\nIt turns into gold. (+10 Gold)";
                }

                LoseRandomTourist(state);
                return "You accept the coupon.\n\nA tourist reads it. They are immediately removed from reality.";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Decline politely",
            state =>
            {
                state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + 5);
                return "You decline.\n\nThe stranger respects your boundaries. (+5 HP)";
            }
        ));

        e.Choices.Add(new NpcChoice(
            "Ask if it works on emotional damage",
            state =>
            {
                return "The stranger nods slowly.\n\n\"Especially emotional damage.\"";
            }
        ));

        return e;
    }

    private static void LoseRandomTourist(DungeonTour.Core.GameState state)
    {
        var alive = state.Tourists.Where(t => t.Alive).ToList();
        if (alive.Count == 0) return;

        var t = alive[Rng.Instance.Next(alive.Count)];
        t.Gone = true;
    }
}
