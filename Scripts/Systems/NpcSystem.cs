using DungeonTour.Core;
using DungeonTour.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DungeonTour.Systems;

public static class NpcSystem
{
    public static async Task RunNpc(GameState state, string? specialComboId)
    {
        Ui.ClearLog();
        Ui.LogLine("[b]== NPC EVENT ==[/b]");
        Ui.LogLine("");

        state.NonFightStreak++;
        state.RestStreak = 0;

        // -------------------------
        // Boredom penalty
        // -------------------------
        if (state.NonFightStreak >= 3)
        {
            Ui.LogLine("Your tourists start whispering.");
            Ui.LogLine("\"Are we actually doing the dungeon or just networking?\"");
            Ui.LogLine("");

            foreach (var t in state.Tourists.Where(t => t.Alive && !t.Gone))
            {
                t.Morale -= 15;

                if (t.Morale <= 0)
                {
                    t.Morale = 0;
                    t.Gone = true;
                    Ui.LogLine($"{t.Name} leaves. They refuse to die of boredom.");
                }
            }

            Ui.LogLine("");
            await Ui.Pause("Continue");
            Ui.ClearLog();
        }

        // -------------------------
        // SNUS COMEBACK (chance)
        // -------------------------
        if (state.SnusTouristLeft && state.SnusTouristIndex >= 0)
        {
            int roll = Rng.Instance.Next(0, 4);
            if (roll == 0)
            {
                bool returned = await TryReturnSnusTouristAsync(state);
                if (returned)
                    return;
            }
        }

        // -------------------------
        // Special combos (if injected)
        // -------------------------
        if (!string.IsNullOrWhiteSpace(specialComboId))
        {
            bool ran = await NpcSpecialCombos.TryRunSpecificCombo(state, specialComboId);
            if (ran)
                return;
        }

        // -------------------------
        // Normal NPC events
        // -------------------------
        var all = NpcEventDatabase.GetAllEvents();
        var available = all.Where(e => !state.SeenNpcEventIds.Contains(e.Id)).ToList();

        if (available.Count == 0)
        {
            Ui.LogLine("You encounter a mysterious stranger.");
            Ui.LogLine("They say something vague and unhelpful.");
            Ui.LogLine("");
            Ui.LogLine("Nothing happens. The universe remains disappointed.");
            await Ui.Pause("Continue");
            return;
        }

        var ev = available[Rng.Instance.Next(available.Count)];
        state.SeenNpcEventIds.Add(ev.Id);

        Ui.LogLine($"[b]{ev.Title}[/b]");
        Ui.LogLine("");
        Ui.LogLine(ev.Text);
        Ui.LogLine("");

        // -------------------------
        // Special: Event_TheWall tourist leaves
        // -------------------------
        if (ev.Id == "the_wall")
        {
            var leaving = PickRandomLivingNotGoneTourist(state);
            if (leaving != null)
            {
                leaving.Gone = true;

                Ui.LogLine("");
                Ui.LogLine($"{leaving.Name} stares at the wall.");
                Ui.LogLine("They whisper: \"This... changes everything.\"");
                Ui.LogLine($"{leaving.Name} leaves to become a tour guide.");
                Ui.LogLine("");
            }
        }

        // -------------------------
        // Choices (limit to 4)
        // -------------------------
        int maxChoices = Math.Min(4, ev.Choices.Count);

        for (int i = 0; i < maxChoices; i++)
        {
            char label = (char)('A' + i);
            Ui.LogLine($"{label}) {ev.Choices[i].Text}");
        }

        Ui.LogLine("");
        Ui.LogLine("Choose:");

        int choiceIndex = await Ui.ReadChoiceIndex(maxChoices);

        var chosen = ev.Choices[choiceIndex];
        string outcome = chosen.Resolve(state);

        Ui.LogLine("");
        Ui.LogLine(outcome);

        await Ui.Pause("Continue");
    }

    // -------------------------
    // Snus tourist return event (AUTO)
    // -------------------------
    private static async Task<bool> TryReturnSnusTouristAsync(GameState state)
    {
        if (state.SnusTouristIndex < 0 || state.SnusTouristIndex >= state.Tourists.Count)
            return false;

        var t = state.Tourists[state.SnusTouristIndex];

        if (!t.Alive)
            return false;

        if (!t.Gone)
            return false;

        Ui.ClearLog();

        Ui.LogLine("[b]== NPC EVENT ==[/b]");
        Ui.LogLine("");
        Ui.LogLine($"You encounter someone in the darkness.");
        Ui.LogLine($"It is [b]{t.Name}[/b].");
        Ui.LogLine("");
        Ui.LogLine("They are holding a plastic bag like a sacred relic.");
        Ui.LogLine("Inside: snus. So much snus.");
        Ui.LogLine("");
        Ui.LogLine($"{t.Name}: \"I got it.\"");
        Ui.LogLine($"{t.Name}: \"I don’t know how long I was gone.\"");
        Ui.LogLine($"{t.Name}: \"Time doesn’t work the same in the snus store.\"");
        Ui.LogLine("");

        // Return
        t.Gone = false;
        t.Morale = Math.Max(40, t.Morale);

        // Clear snus state
        state.SnusTouristLeft = false;
        state.SnusTouristIndex = -1;

        // Bonus reward
        int bonusRoll = Rng.Instance.Next(0, 2);
        if (bonusRoll == 0)
        {
            state.Inventory.Potions++;
            Ui.LogLine($"{t.Name} hands you a potion.");
            Ui.LogLine("\"They gave me this for free. I think it’s illegal.\"");
        }
        else
        {
            state.Inventory.Gold += 10;
            Ui.LogLine($"{t.Name} hands you crumpled cash.");
            Ui.LogLine("\"I don’t remember paying. But I have a receipt.\"");
        }

        Ui.LogLine("");
        Ui.LogLine($"{t.Name} rejoins the tour.");
        Ui.LogLine("Your tourists stare in horror and admiration.");

        await Ui.Pause("Continue");
        return true;
    }

    private static Tourist? PickRandomLivingNotGoneTourist(GameState state)
    {
        var living = state.Tourists.Where(t => t.Alive && !t.Gone).ToList();
        if (living.Count == 0)
            return null;

        int idx = Rng.Instance.Next(living.Count);
        return living[idx];
    }
}
