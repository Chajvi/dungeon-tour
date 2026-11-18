using System;
using System.Linq;
using System.Threading.Tasks;
using DungeonTour.Core;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class CombatSystem
{
    public static async Task RunFight(GameState state)
    {
        state.NonFightStreak = 0;
        state.RestStreak = 0;
        var enemy = EnemyFactory.CreateEnemy(state);
        await RunFight(state, enemy, allowRun: true);
    }

    public static async Task RunNpcFight(GameState state, Enemy npcEnemy)
    {
        await RunFight(state, npcEnemy, allowRun: false);
    }

    private static async Task RunFight(GameState state, Enemy enemy, bool allowRun)
    {
        ConsoleShim.WriteLine("== FIGHT ==");
        ConsoleShim.WriteLine($"A {enemy.Name} appears!");

        if (!string.IsNullOrWhiteSpace(enemy.IntroText))
        {
            ConsoleShim.WriteLine();
            ConsoleShim.WriteLine(enemy.IntroText);
        }

        ConsoleShim.WriteLine();

        await Ui.Pause("Start");

        while (state.Player.Hp > 0 && enemy.Hp > 0)
        {
            ConsoleShim.Clear();
            Ui.Hud(state);

            ConsoleShim.WriteLine($"Enemy: {enemy.Name}  HP: {enemy.Hp}/{enemy.MaxHp}");
            ConsoleShim.WriteLine();

            if (enemy.Name == "Seagull With A Gun Licence")
            {
                int roll = Rng.Instance.Next(1, 101);

                if (roll <= 50)
                {
                    var present = state.Tourists.Where(t => t.Alive && !t.Gone).ToList();

                    if (present.Count > 0)
                    {
                        int idx = Rng.Instance.Next(present.Count);
                        var quitter = present[idx];

                        quitter.Gone = true;

                        ConsoleShim.WriteLine("The seagull screams like a car alarm.");
                        ConsoleShim.WriteLine($"{quitter.Name} covers their ears and flees into the darkness.");
                        ConsoleShim.WriteLine();
                    }
                }
            }

            // -------------------------
            // ENEMY DECIDES (hidden)
            // -------------------------
            bool enemyWillDefendThisTurn = false;

            int enemyDecisionRoll = Rng.Instance.Next(1, 101);
            if (enemyDecisionRoll <= 50)
                enemyWillDefendThisTurn = true;

            // -------------------------
            // PLAYER CHOICE
            // -------------------------
            ConsoleShim.WriteLine("1) Attack");
            ConsoleShim.WriteLine("2) Defend");
            Ui.LogLine("3) Drink [img=40x40]res://Assets/Icons/Drink.png[/img]");

            if (allowRun)
                ConsoleShim.WriteLine("4) Try to run");

            ConsoleShim.WriteLine();
            ConsoleShim.Write("Choose: ");

            int action = allowRun
                ? await Ui.ReadIntInRange(1, 4)
                : await Ui.ReadIntInRange(1, 3);

            ConsoleShim.WriteLine();

            bool playerDefended = false;

            // -------------------------
            // PLAYER TURN
            // -------------------------
            if (action == 1)
            {
                int baseDamage = state.Player.BaseDamage + state.BonusDamageNextFight;

                int dmg = Damage.Roll(baseDamage, variance: 3);

                if (enemyWillDefendThisTurn)
                {
                    dmg = Math.Max(0, dmg - 4);
                    ConsoleShim.WriteLine($"The {enemy.Name} defends!");
                    ConsoleShim.WriteLine("[i](Reduced damage)[/i]");
                    ConsoleShim.WriteLine();
                }

                enemy.Hp -= dmg;

                ConsoleShim.WriteLine($"You attack. Damage: {dmg}.");

                if (state.BonusDamageNextFight > 0)
                    ConsoleShim.WriteLine($"(Boost applied: +{state.BonusDamageNextFight})");
            }
            else if (action == 2)
            {
                playerDefended = true;
                ConsoleShim.WriteLine("You defend. Your stance radiates liability waiver energy.");
            }
            else if (action == 3)
            {
                if (state.Inventory.Potions <= 0)
                {
                    ConsoleShim.WriteLine("No potions left. Only fear.");
                }
                else if (state.Player.Hp >= state.Player.MaxHp)
                {
                    ConsoleShim.WriteLine("You are already at full HP. The potion stares back at you.");
                }
                else
                {
                    state.Inventory.Potions--;

                    int heal = (int)Math.Round(state.Player.MaxHp * 0.4);
                    if (heal < 10) heal = 10;

                    int before = state.Player.Hp;
                    state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + heal);
                    int actual = state.Player.Hp - before;

                    ConsoleShim.WriteLine($"You drink a potion. Heal: {actual}. Potions left: {state.Inventory.Potions}.");
                    ConsoleShim.WriteLine($"HP: {state.Player.Hp}/{state.Player.MaxHp}");
                }
            }
            else if (allowRun && action == 4)
            {
                int roll = Rng.Instance.Next(1, 101);
                if (roll <= 45)
                {
                    ConsoleShim.WriteLine("You run. The dungeon judges you silently.");

                    state.BonusDamageNextFight = 0;

                    return;
                }
                else
                {
                    ConsoleShim.WriteLine("You try to run but your legs refuse due to union rules.");
                }
            }

            if (enemy.Hp <= 0)
                break;

            // -------------------------
            // ENEMY TURN
            // -------------------------
            if (enemyWillDefendThisTurn)
            {
                ConsoleShim.WriteLine();
                ConsoleShim.WriteLine("Press any key...");
                await Ui.Pause("Continue");
                continue;
            }

            int enemyDmg = Damage.Roll(enemy.BaseDamage, variance: 3);
            if (playerDefended) enemyDmg = Math.Max(0, enemyDmg - 4);

            state.Player.Hp -= enemyDmg;

            Ui.HitShake();

            ConsoleShim.WriteLine($"The {enemy.Name} attacks. You take {enemyDmg} damage.");

            ConsoleShim.WriteLine();
            ConsoleShim.WriteLine("Press any key...");
            await Ui.Pause("Continue");
        }

        state.BonusDamageNextFight = 0;

        if (state.Player.Hp <= 0)
            return;

        if (enemy.Hp <= 0)
        {
            int gold = Rng.Instance.Next(8, 17);
            state.Inventory.Gold += gold;

            ConsoleShim.WriteLine();
            ConsoleShim.WriteLine($"Victory. You collect {gold} gold.");
            await Ui.Pause("Continue");
        }
    }
}
