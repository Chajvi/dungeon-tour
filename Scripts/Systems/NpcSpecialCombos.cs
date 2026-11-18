using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DungeonTour.Core;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class NpcSpecialCombos
{
    public sealed class SpecialComboInfo
    {
        public string ComboId { get; }
        public string OptionText { get; }

        // Which normal NpcEvent this combo should "consume"/block for the run
        public string? BlocksNpcEventId { get; }

        public SpecialComboInfo(string comboId, string optionText, string? blocksNpcEventId)
        {
            ComboId = comboId;
            OptionText = optionText;
            BlocksNpcEventId = blocksNpcEventId;
        }
    }

    private sealed class ComboDef
    {
        public string ComboId { get; }
        public string OptionText { get; }
        public string RequiredTouristName { get; }
        public string? BlocksNpcEventId { get; }
        public Func<GameState, Task> RunAsync { get; }

        public ComboDef(
            string comboId,
            string optionText,
            string requiredTouristName,
            string? blocksNpcEventId,
            Func<GameState, Task> runAsync)
        {
            ComboId = comboId;
            OptionText = optionText;
            RequiredTouristName = requiredTouristName;
            BlocksNpcEventId = blocksNpcEventId;
            RunAsync = runAsync;
        }
    }

    // Single source of truth for:
    // - who enables the combo (RequiredTouristName)
    // - what option text to show
    // - which normal NPC event should be blocked (BlocksNpcEventId)
    // - what actually runs
    private static readonly List<ComboDef> _defs = new()
    {
        // Merthrude bakery (npc_merthrude_bakery)
        new ComboDef(
            comboId: "combo_bertha_merthrude_fight",
            optionText: "A cinnamon bun duel is about to happen (NPC)",
            requiredTouristName: "Bertha",
            blocksNpcEventId: "npc_merthrude_bakery",
            runAsync: Run_BerthaMerthrudeFight
        ),
        new ComboDef(
            comboId: "combo_chad_mid_buns_tourist_loss",
            optionText: "An elderly woman guards cinnamon buns (NPC)",
            requiredTouristName: "Chad",
            blocksNpcEventId: "npc_merthrude_bakery",
            runAsync: state =>
            {
                Run_ChadMidBuns(state);
                return Task.CompletedTask;
            }
        ),

        // Ghost receptionist (ghost_receptionist)
        new ComboDef(
            comboId: "combo_agnes_manager_fight",
            optionText: "A ghost receptionist offers \"service\" (NPC)",
            requiredTouristName: "Agnes",
            blocksNpcEventId: "ghost_receptionist",
            runAsync: Run_AgnesManagerFight
        ),
        new ComboDef(
            comboId: "combo_sophie_sponsorship_gold",
            optionText: "A ghost receptionist offers an \"upgrade\" (NPC)",
            requiredTouristName: "Sophie",
            blocksNpcEventId: "ghost_receptionist",
            runAsync: state =>
            {
                Run_SophieSponsorship(state);
                return Task.CompletedTask;
            }
        ),

        // Influencer ritual (influencer_ritual)
        new ComboDef(
            comboId: "combo_brenda_content_demon_fight",
            optionText: "A dungeon influencer is filming a ritual (NPC)",
            requiredTouristName: "Brenda",
            blocksNpcEventId: "influencer_ritual",
            runAsync: Run_BrendaContentDemonFight
        ),

        // Permit goblin (permit_goblin)
        new ComboDef(
            comboId: "combo_penny_tripadvisor_refund",
            optionText: "A goblin with a clipboard blocks your path (NPC)",
            requiredTouristName: "Penny",
            blocksNpcEventId: "permit_goblin",
            runAsync: state =>
            {
                Run_PennyRefund(state);
                return Task.CompletedTask;
            }
        ),
        new ComboDef(
            comboId: "combo_merle_double_fee",
            optionText: "A goblin demands a permit fee (NPC)",
            requiredTouristName: "Merle",
            blocksNpcEventId: "permit_goblin",
            runAsync: state =>
            {
                Run_MerleDoubleFee(state);
                return Task.CompletedTask;
            }
        ),

        // The wall (the_wall)
        new ComboDef(
            comboId: "combo_keith_wall_potion",
            optionText: "A tour guide is arguing with a wall (NPC)",
            requiredTouristName: "Keith",
            blocksNpcEventId: "the_wall",
            runAsync: state =>
            {
                Run_KeithWallPotion(state);
                return Task.CompletedTask;
            }
        ),

        // Suspicious coupon (suspicious_coupon)
        new ComboDef(
            comboId: "combo_beatrice_coupon_gold",
            optionText: "A hooded stranger offers you a coupon (NPC)",
            requiredTouristName: "Beatrice",
            blocksNpcEventId: "suspicious_coupon",
            runAsync: state =>
            {
                Run_BeatriceCouponGold(state);
                return Task.CompletedTask;
            }
        ),
        new ComboDef(
            comboId: "combo_gunnar_coupon_damage",
            optionText: "A hooded stranger offers you a cursed coupon (NPC)",
            requiredTouristName: "Gunnar",
            blocksNpcEventId: "suspicious_coupon",
            runAsync: state =>
            {
                Run_GunnarCouponDamage(state);
                return Task.CompletedTask;
            }
        ),
    };

    // -------------------------
    // Used by NodeGenerator
    // -------------------------
    public static SpecialComboInfo? GetAvailableSpecialComboForParty(GameState state)
    {
        var aliveNames = state.Tourists
            .Where(t => t.Alive && !t.Gone)
            .Select(t => t.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var d in _defs)
        {
            if (state.TriggeredSpecialComboIds.Contains(d.ComboId))
                continue;

            if (!aliveNames.Contains(d.RequiredTouristName))
                continue;

            return new SpecialComboInfo(d.ComboId, d.OptionText, d.BlocksNpcEventId);
        }

        return null;
    }

    // -------------------------
    // Used by NpcSystem
    // -------------------------
    public static async Task<bool> TryRunSpecificCombo(GameState state, string comboId)
    {
        // 1) Validate comboId FIRST (don’t accidentally "consume" unknown ids)
        var def = _defs.FirstOrDefault(d => d.ComboId.Equals(comboId, StringComparison.OrdinalIgnoreCase));
        if (def == null)
            return false;

        // 2) Already ran this run?
        if (state.TriggeredSpecialComboIds.Contains(def.ComboId))
            return false;

        // 3) Mark triggered
        state.TriggeredSpecialComboIds.Add(def.ComboId);

        // 4) Block the normal event that this combo replaces/consumes
        if (!string.IsNullOrWhiteSpace(def.BlocksNpcEventId))
            state.SeenNpcEventIds.Add(def.BlocksNpcEventId);

        // 5) Run the combo
        await def.RunAsync(state);
        return true;
    }

    // -------------------------
    // Helpers
    // -------------------------
    private static void LoseRandomTourist(GameState state, string reasonLine)
    {
        var candidates = state.Tourists.Where(t => t.Alive && !t.Gone).ToList();
        if (candidates.Count == 0) return;

        var t = candidates[Rng.Instance.Next(candidates.Count)];
        t.Gone = true;

        Ui.LogLine("");
        Ui.LogLine($"{t.Name} {reasonLine}");
    }

    // -------------------------
    // 1) Bertha + Merthrude (Fight)
    // -------------------------
    private static async Task Run_BerthaMerthrudeFight(GameState state)
    {
        ConsoleShim.WriteLine("You spot an elderly woman sitting calmly on a stool.");
        ConsoleShim.WriteLine("She is holding a tray of cinnamon buns like a weapon.");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("\"My name is Merthrude,\" she says.");
        ConsoleShim.WriteLine();

        ConsoleShim.WriteLine("Bertha steps forward.");
        ConsoleShim.WriteLine("\"YOU.\"");
        ConsoleShim.WriteLine("\"YOU STOLE MY MAN.\"");
        ConsoleShim.WriteLine("\"WITH YOUR BETTER BUNS.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("Merthrude sighs.");
        ConsoleShim.WriteLine("\"Bertha... It was 1973.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("They begin to fight with the violence of two 90-year-old legends.");
        ConsoleShim.WriteLine();

        await Ui.Pause();

        var enemy = EnemyFactory.CreateNpcEnemy("Merthrude (Bun Duelist)", state.RoomIndex);
        await CombatSystem.RunNpcFight(state, enemy);
    }

    // -------------------------
    // 2) Sophie + Ghost Receptionist (Reward)
    // -------------------------
    private static void Run_SophieSponsorship(GameState state)
    {
        ConsoleShim.WriteLine("Sophie immediately starts filming.");
        ConsoleShim.WriteLine("\"Hey guys! I'm here with a REAL ghost receptionist!\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The ghost pauses.");
        ConsoleShim.WriteLine("\"...We do brand deals now.\"");
        ConsoleShim.WriteLine();

        state.Inventory.Gold += 20;

        ConsoleShim.WriteLine("You receive 20 gold in sponsorship money.");
        ConsoleShim.WriteLine("Your tourists clap. Nobody knows why.");
    }

    // -------------------------
    // 3) Keith + The Wall (Reward)
    // -------------------------
    private static void Run_KeithWallPotion(GameState state)
    {
        ConsoleShim.WriteLine("Keith steps forward and clears his throat.");
        ConsoleShim.WriteLine("\"Actually, this wall is load-bearing.\"");
        ConsoleShim.WriteLine("\"If it moved, it would violate several standards.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The wall seems... impressed?");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("A hidden compartment opens and drops something on your foot.");

        state.Inventory.Potions++;

        ConsoleShim.WriteLine("You gained 1 potion.");
    }

    // -------------------------
    // 4) Gunnar + Coupon (Disaster)
    // -------------------------
    private static void Run_GunnarCouponDamage(GameState state)
    {
        ConsoleShim.WriteLine("Gunnar squints at the coupon.");
        ConsoleShim.WriteLine("\"This has a dark aura.\"");
        ConsoleShim.WriteLine("\"Like... customer service.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("Everyone ignores him.");
        ConsoleShim.WriteLine();

        state.Player.Hp -= 6;

        ConsoleShim.WriteLine("The coupon steals a tiny piece of your soul.");
        ConsoleShim.WriteLine("(-6 HP)");
    }

    // -------------------------
    // 5) Merle + Permit Goblin (Disaster)
    // -------------------------
    private static void Run_MerleDoubleFee(GameState state)
    {
        ConsoleShim.WriteLine("Merle points at the goblin.");
        ConsoleShim.WriteLine("\"This is a paid actor.\"");
        ConsoleShim.WriteLine("\"WAKE UP, SHEEP.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The goblin blinks.");
        ConsoleShim.WriteLine("\"Double fee.\"");

        int fee = 20;

        if (state.Inventory.Gold < fee)
        {
            ConsoleShim.WriteLine();
            ConsoleShim.WriteLine("You cannot pay.");
            LoseRandomTourist(state, "gets taken as a 'processing fee'.");
            return;
        }

        state.Inventory.Gold -= fee;
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine($"You pay {fee} gold.");
    }

    // -------------------------
    // 6) Brenda + Influencer Ritual (Fight)
    // -------------------------
    private static async Task Run_BrendaContentDemonFight(GameState state)
    {
        ConsoleShim.WriteLine("Brenda starts crying instantly.");
        ConsoleShim.WriteLine("Not a cute cry.");
        ConsoleShim.WriteLine("A real, full-body sob.");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The influencer gasps.");
        ConsoleShim.WriteLine("\"OMG. This is PERFECT content.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The altar responds.");
        ConsoleShim.WriteLine("A demon made of algorithms crawls out.");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("\"CONTENT... MUST... CONTINUE...\"");
        ConsoleShim.WriteLine();

        await Ui.Pause();

        var enemy = EnemyFactory.CreateNpcEnemy("Content Demon (Engagement)", state.RoomIndex);
        await CombatSystem.RunNpcFight(state, enemy);
    }

    // -------------------------
    // 7) Chad + Merthrude (Disaster)
    // -------------------------
    private static void Run_ChadMidBuns(GameState state)
    {
        ConsoleShim.WriteLine("Chad takes a bite of the cinnamon bun.");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("\"Mid.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The dungeon goes silent.");
        ConsoleShim.WriteLine("Merthrude smiles politely.");
        ConsoleShim.WriteLine();

        LoseRandomTourist(state, "leaves the tour out of second-hand embarrassment.");
    }

    // -------------------------
    // 8) Agnes + Ghost Receptionist (Fight)
    // -------------------------
    private static async Task Run_AgnesManagerFight(GameState state)
    {
        ConsoleShim.WriteLine("Agnes leans forward.");
        ConsoleShim.WriteLine("\"I'd like to speak to your manager.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The ghost receptionist freezes.");
        ConsoleShim.WriteLine("\"...We have one.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("A larger ghost rises from behind the desk.");
        ConsoleShim.WriteLine("It radiates corporate rage.");
        ConsoleShim.WriteLine();

        await Ui.Pause();

        var enemy = EnemyFactory.CreateNpcEnemy("Manager Spirit (Customer Service)", state.RoomIndex);
        await CombatSystem.RunNpcFight(state, enemy);
    }

    // -------------------------
    // 9) Penny + Permit Goblin (Reward)
    // -------------------------
    private static void Run_PennyRefund(GameState state)
    {
        ConsoleShim.WriteLine("Penny cracks her knuckles.");
        ConsoleShim.WriteLine("\"Listen. I WILL leave a review.\"");
        ConsoleShim.WriteLine("\"A long one.\"");
        ConsoleShim.WriteLine("\"With photos.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The goblin goes pale.");
        ConsoleShim.WriteLine("\"Refund.\"");

        state.Inventory.Gold += 10;

        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("You receive 10 gold.");
    }

    // -------------------------
    // 10) Beatrice + Coupon (Reward)
    // -------------------------
    private static void Run_BeatriceCouponGold(GameState state)
    {
        ConsoleShim.WriteLine("Beatrice takes the coupon with two fingers.");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("\"No thank you.\"");
        ConsoleShim.WriteLine("\"But yes.\"");
        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The hooded stranger looks uncomfortable.");
        ConsoleShim.WriteLine("The coupon trembles.");

        state.Inventory.Gold += 15;

        ConsoleShim.WriteLine();
        ConsoleShim.WriteLine("The coupon pays you 15 gold to please stop.");
    }
}
