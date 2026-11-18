using DungeonTour.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DungeonTour.Systems;

public static class RestSystem
{
    public static void RunRest(GameState state)
    {
        Ui.ClearLog();
        Ui.LogLine("[b]== REST ==[/b]");
        Ui.LogLine("");

        state.RestStreak++;
        state.NonFightStreak++;
        state.RoomsSinceRest = 0;

        int heal = Rng.Instance.Next(8, 15);
        state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + heal);

        Ui.LogLine($"You rest. Heal: {heal}.");
        Ui.LogLine("Your tourists stop screaming for a short, legally significant moment.");
        Ui.LogLine("");

        if (state.RestStreak >= 2)
        {
            Ui.LogLine("Your tourists start pacing in circles.");
            Ui.LogLine("\"Did we pay for a dungeon tour or a nap retreat?\"");
            Ui.LogLine("");

            foreach (var t in state.Tourists.Where(t => t.Alive && !t.Gone))
            {
                t.Morale -= 20;

                if (t.Morale <= 0)
                {
                    t.Morale = 0;
                    t.Gone = true;
                    Ui.LogLine($"{t.Name} gets up and leaves. Permanently.");
                }
            }

            Ui.LogLine("");
        }

        // Morale recovery
        var alive = state.Tourists.Where(t => t.Alive && !t.Gone).ToList();
        foreach (var t in alive)
            t.Morale = Math.Min(100, t.Morale + Rng.Instance.Next(8, 16));

        Ui.LogLine("Your tourists regain a small amount of hope.");
    }

    // -------------------------
    // Snus leave trigger
    // -------------------------
    public static async Task TryTriggerSnusLeaveAsync(GameState state)
    {
        if (state.SnusEventUsed)
            return;

        if (state.SnusTouristLeft)
            return;

        if (state.RoomsSinceRest < 5)
            return;

        var leaving = state.Tourists.Where(t => t.Alive && !t.Gone).ToList();
        if (leaving.Count == 0)
            return;

        var t = leaving[Rng.Instance.Next(leaving.Count)];

        t.Gone = true;
        state.SnusTouristLeft = true;

        state.SnusEventUsed = true;
        state.SnusTouristIndex = state.Tourists.IndexOf(t);

        Ui.ClearLog();
        Ui.LogLine("[b]== TOURIST EVENT ==[/b]");
        Ui.LogLine("");
        Ui.LogLine($"{t.Name} stops walking.");
        Ui.LogLine("");
        Ui.LogLine($"{t.Name}: \"I forgot to buy snus.\"");
        Ui.LogLine($"{t.Name}: \"I have to go home.\"");
        Ui.LogLine("");
        Ui.LogLine("They leave with the confidence of someone making this your problem.");
        Ui.LogLine("");
        Ui.LogLine("[i](They may return later.)[/i]");

        state.RestStreak = 0;
        state.NonFightStreak = 0;

        await Ui.Pause("Continue");
    }

}
