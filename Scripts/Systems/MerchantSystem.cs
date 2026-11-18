using System;
using System.Linq;
using System.Threading.Tasks;
using DungeonTour.Core;
using DungeonTour.Models;

namespace DungeonTour.Systems;

public static class MerchantSystem
{
    private enum MerchantType
    {
        SisterMargo = 0,
        Kevin = 1,
        Pam = 2
    }

    public static async Task RunMerchant(GameState state)
    {
        Ui.ClearLog();

        MerchantType merchant = RollMerchant();

        Ui.LogLine($"[i]DEBUG: Rolled merchant = {merchant}[/i]");
        Ui.LogLine("");

        switch (merchant)
        {
            case MerchantType.SisterMargo:
                await RunSisterMargo(state);
                return;

            case MerchantType.Kevin:
                await RunKevin(state);
                return;

            case MerchantType.Pam:
                await RunPam(state);
                return;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // -------------------------
    // Merchant: Sister Margo
    // -------------------------
    private static async Task RunSisterMargo(GameState state)
    {
        Ui.LogLine("[b]== MERCHANT ==[/b]");

        state.NonFightStreak++;
        state.RestStreak = 0;

        if (state.NonFightStreak >= 3)
        {
            Ui.LogLine("");
            Ui.LogLine("Your tourists start looking around, talking over eachother.");
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
        }

        if (!state.SeenMargo)
        {
            state.SeenMargo = true;

            Ui.LogLine("Sister Margo of the Sterile Cloth raises one eyebrow.");
            Ui.LogLine("");

            Ui.LogLine("Tourist: \"I AIN’T GETTING NO VACCINATIONS.\"");
            Ui.LogLine("Tourist: \"Ooh I hope she has the GOOD bandaids.\"");
            Ui.LogLine("Tourist: \"I don’t trust doctors. I know how to use Google.\"");
            Ui.LogLine("Tourist: \"I don’t want a potion. I want a Capri-Sun and a lie.\"");
            Ui.LogLine("Tourist: \"I’m not letting her look at my ankles.\"");
            Ui.LogLine("");

            await Ui.Pause("Continue");
            Ui.ClearLog();
        }
        else
        {
            Ui.LogLine("Oh no it's her again.");
            Ui.LogLine("Tourist: \"I'm still not letting her look at my ankles.\"");
            Ui.LogLine("");

            await Ui.Pause("Continue");
            Ui.ClearLog();
        }


        Ui.LogLine("[b]Sister Margo of the Sterile Cloth[/b]");
        Ui.LogLine("A nun-like figure stands behind a folding table of bandages.");
        Ui.LogLine("Her eyes say: you are a lawsuit waiting to happen.");
        Ui.LogLine("");

        bool done = false;
        while (!done)
        {
            Ui.LogLine($"Gold: {state.Inventory.Gold} | Potions: {state.Inventory.Potions}");
            Ui.LogLine("");
            Ui.LogLine("1) Sterile Bandage Roll (10 gold)  [Heal +12]");
            Ui.LogLine("2) Adrenaline Shot (18 gold)       [Damage boost]");
            Ui.LogLine("3) Painkillers (14 gold)           [Big heal, morale hit]");
            Ui.LogLine("4) Leave");
            Ui.LogLine("");

            int c = await Ui.ReadIntInRange(1, 4);
            Ui.ClearLog();

            switch (c)
            {
                case 1:
                    if (!CanAfford(state, 10)) break;
                    state.Inventory.Gold -= 10;
                    HealPlayer(state, 12);
                    Ui.LogLine("Sister Margo wraps you in sterile cloth with the rage of a professional.");
                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;

                case 2:
                    if (!CanAfford(state, 18)) break;
                    state.Inventory.Gold -= 18;
                    state.BonusDamageNextFight += 17;
                    Ui.LogLine("You take the adrenaline shot.");
                    Ui.LogLine("You feel invincible. That is medically incorrect.");
                    Ui.LogLine("");
                    Ui.LogLine("[i](Damage boost not implemented yet — placeholder effect.)[/i]");
                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;

                case 3:
                    if (!CanAfford(state, 14)) break;
                    state.Inventory.Gold -= 14;

                    HealPlayer(state, 20);

                    var living = state.Tourists.Where(t => t.Alive).ToList();
                    if (living.Count > 0)
                    {
                        int idx = Rng.Instance.Next(living.Count);
                        living[idx].Morale = Math.Max(0, living[idx].Morale - 10);
                        Ui.LogLine($"One tourist looks at the pills and whispers: \"Are we supposed to take those?\"");
                        Ui.LogLine($"{living[idx].Name} loses morale.");
                    }
                    else
                    {
                        Ui.LogLine("Nobody is alive enough to judge your painkiller choices.");
                    }

                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;

                default:
                    done = true;
                    break;
            }
        }
    }

    // -------------------------
    // Merchant: Kevin
    // -------------------------
    private static async Task RunKevin(GameState state)
    {
        Ui.LogLine("[b]== MERCHANT ==[/b]");

        state.NonFightStreak++;
        state.RestStreak = 0;

        if (state.NonFightStreak >= 3)
        {
            Ui.LogLine("");
            Ui.LogLine("Your tourists start looking around, bored out of their minds.");
            Ui.LogLine("\"Are we shopping or surviving?\"");
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
        }

        if (!state.SeenKevin)
        {
            state.SeenKevin = true;

            Ui.LogLine("Kevin appears.");
            Ui.LogLine("Nobody knows from where.");
            Ui.LogLine("");

            Ui.LogLine("Tourist: \"Is that… just a guy?\"");
            Ui.LogLine("Tourist: \"This man has never washed a hoodie.\"");
            Ui.LogLine("Tourist: \"If he sells me a sword I’m leaving a good review.\"");
            Ui.LogLine("Tourist: \"He looks like he owns a vape store.\"");
            Ui.LogLine("");

            var eleanor = state.Tourists.FirstOrDefault(t => t.Alive && !t.Gone && t.Name == "Eleanor");
            if (eleanor != null)
            {
                Ui.LogLine("");
                Ui.LogLine("Eleanor: \"Oh my god. Kevin, WHERE IS MY CHILD SUPPORT!\"");
                Ui.LogLine("Kevin: \"Uh. Okay. Wow. Publicly?\"");
                Ui.LogLine("Kevin: \"Fine. Here. Take it. Please stop yelling.\"");
                Ui.LogLine("");

                state.Inventory.Gold += 20;
                Ui.LogLine("[b]Child Support +20[/b]");
            }


            await Ui.Pause("Continue");
            Ui.ClearLog();
        }
        else
        {
            Ui.LogLine("Kevin is still here.");
            Ui.LogLine("");

            await Ui.Pause("Continue");
            Ui.ClearLog();
        }


        Ui.LogLine("[b]Kevin[/b]");
        Ui.LogLine("Kevin is wearing a lanyard.");
        Ui.LogLine("He is somehow running a pop-up shop inside the dungeon.");
        Ui.LogLine("");

        bool done = false;
        while (!done)
        {
            int alive = state.Tourists.Count(t => t.Alive);

            Ui.LogLine($"Gold: {state.Inventory.Gold} | Potions: {state.Inventory.Potions} | Tourists: {alive}/{state.Tourists.Count}");
            Ui.LogLine("");
            Ui.LogLine("1) Buy Potion (10 gold)            [Heal item]");
            Ui.LogLine("2) Sharpened Scrap Blade (22 gold) [Damage boost]");
            Ui.LogLine("3) Buy Tourist Voucher (30 gold)   [Only if party not full]");
            Ui.LogLine("4) Leave");
            Ui.LogLine("");

            int c = await Ui.ReadIntInRange(1, 4);
            Ui.ClearLog();

            switch (c)
            {
                case 1:
                    if (!CanAfford(state, 10)) break;
                    state.Inventory.Gold -= 10;
                    state.Inventory.Potions++;
                    Ui.LogLine("Kevin hands you a potion like it's a subscription renewal.");
                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;

                case 2:
                    if (state.HasKevinsScrapBlade)
                    {
                        Ui.LogLine("Kevin: \"Nope. One per customer. Corporate said so.\"");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                    if (!CanAfford(state, 22)) break;

                    state.Inventory.Gold -= 22;

                    state.Player.BaseDamage += 4;
                    state.HasKevinsScrapBlade = true;

                    Ui.LogLine("Kevin sells you a sharpened scrap blade.");
                    Ui.LogLine("It is 80% rust and 20% confidence.");
                    Ui.LogLine("Kevin: \"Congratulations. You are now a premium customer.\"");

                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;


                case 3:
                    if (alive >= state.Tourists.Count)
                    {
                        Ui.LogLine("Kevin: \"You already have a full group. I can't upsell you.\"");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                    if (!CanAfford(state, 30)) break;

                    state.Inventory.Gold -= 30;

                    var dead = state.Tourists.FirstOrDefault(t => !t.Alive);
                    if (dead == null)
                    {
                        Ui.LogLine("Kevin looks confused. \"Wait. Where would I even put them?\"");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                    var newGuy = TouristFactory.CreateParty(1).FirstOrDefault();
                    if (newGuy == null)
                    {
                        Ui.LogLine("Kevin checks his clipboard. \"Sorry. We're out of tourists.\"");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                    dead.Name = newGuy.Name;
                    dead.Trait = newGuy.Trait;
                    dead.Alive = true;
                    dead.Morale = 60;

                    Ui.LogLine("Kevin snaps his fingers.");
                    Ui.LogLine($"A new tourist appears: [b]{dead.Name}[/b].");
                    Ui.LogLine("They look confused, but legally consenting.");
                    await Ui.Pause("Continue");
                    Ui.ClearLog();
                    break;

                default:
                    done = true;
                    break;
            }
        }
    }

    // -------------------------
    // Merchant: Pam
    // -------------------------
    private static async Task RunPam(GameState state)
    {
        Ui.LogLine("[b]== MERCHANT ==[/b]");

        state.NonFightStreak++;
        state.RestStreak = 0;

        if (state.NonFightStreak >= 3)
        {
            Ui.LogLine("");
            Ui.LogLine("Your tourists start looking around, bored out of their minds.");
            Ui.LogLine("\"Are we shopping or surviving?\"");
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
        }

        // -------------------------
        // Pam intro (first time vs repeat)
        // -------------------------
        if (!state.SeenPam)
        {
            state.SeenPam = true;

            Ui.LogLine("Pam, Head of Merch, is waiting.");
            Ui.LogLine("She smiles like someone who has never refunded anything in her life.");
            Ui.LogLine("");

            Ui.LogLine("Tourist: \"Oh no… she’s a manager.\"");
            Ui.LogLine("Tourist: \"I KNOW she’s gonna ask if I’m a member.\"");
            Ui.LogLine("Tourist: \"She’s about to upsell me into the afterlife.\"");
            Ui.LogLine("Tourist: \"She looks like she could sell a sword to a ghost.\"");
            Ui.LogLine("Tourist: \"I feel like I’m about to get scammed professionally.\"");
            Ui.LogLine("");

            await Ui.Pause("Continue");
            Ui.ClearLog();
        }
        else
        {
            Ui.LogLine("Pam is here again.");
            Ui.LogLine("Tourist: \"I’m not making eye contact this time.\"");
            Ui.LogLine("");

            await Ui.Pause("Continue");
            Ui.ClearLog();
        }

        // -------------------------
        // Pam description
        // -------------------------
        Ui.LogLine("[b]Pam, Head of Merch[/b]");
        Ui.LogLine("Pam has a cash register, a tote bag wall, and the aura of corporate dominance.");
        Ui.LogLine("There is absolutely no reason for this to exist here.");
        Ui.LogLine("");

        // -------------------------
        // Membership scam (only once, if not already member)
        // -------------------------
        if (!state.HasPamMembership)
        {
            bool doneWithMembership = false;

            while (!doneWithMembership)
            {
                Ui.LogLine("Pam: \"Before we begin — do you want to become a member?\"");
                Ui.LogLine("");

                Ui.LogLine("1) Yes");
                Ui.LogLine("2) No");
                Ui.LogLine("");

                int m = await Ui.ReadIntInRange(1, 2);
                Ui.ClearLog();

                if (m == 1)
                {
                    state.HasPamMembership = true;

                    Ui.LogLine("Pam: \"Perfect. Welcome to the Pam Rewards Program.\"");
                    Ui.LogLine("Pam: \"You are now saving money.\"");
                    Ui.LogLine("");

                    state.Inventory.Gold += 15;
                    Ui.LogLine("[b]Welcome Bonus +15[/b]");
                    Ui.LogLine("");

                    foreach (var t in state.Tourists.Where(t => t.Alive && !t.Gone))
                        t.Morale = Math.Max(0, t.Morale - 2);

                    Ui.LogLine("Your tourists look violated.");
                    Ui.LogLine("");

                    await Ui.Pause("Continue");
                    Ui.ClearLog();

                    doneWithMembership = true;
                }
                else
                {
                    state.PamRefusals++;

                    if (state.PamRefusals >= 3)
                    {
                        Ui.LogLine("Pam: \"Fine.\"");
                        Ui.LogLine("Pam: \"But don’t come crying to me when you could’ve saved 0.3 gold.\"");
                        Ui.LogLine("");
                        Ui.LogLine("Pam is visibly offended by your lack of brand loyalty.");
                        Ui.LogLine("");

                        await Ui.Pause("Continue");
                        Ui.ClearLog();

                        doneWithMembership = true;
                    }
                    else
                    {
                        Ui.LogLine("Pam: \"Okay.\"");
                        Ui.LogLine("Pam: \"Are you [i]sure[/i]?\"");
                        Ui.LogLine("Pam: \"Because members save.\"");
                        Ui.LogLine("");

                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                    }
                }
            }
        }

        // -------------------------
        // Shop loop
        // -------------------------
        bool done = false;
        while (!done)
        {
            Ui.LogLine($"Gold: {state.Inventory.Gold} | Potions: {state.Inventory.Potions}");
            Ui.LogLine("");
            Ui.LogLine("1) Hot Chocolate (12 gold)         [Heal +10 morale]");
            Ui.LogLine("2) Foam Sword of Destiny (16 gold) [Damage boost]");
            Ui.LogLine("3) 'I Survived The Dungeon' Shirt (20 gold) [Special]");
            Ui.LogLine("4) Leave");
            Ui.LogLine("");

            int c = await Ui.ReadIntInRange(1, 4);
            Ui.ClearLog();

            switch (c)
            {
                case 1:
                    {
                        int cost = 12;

                        if (!CanAfford(state, cost)) break;
                        state.Inventory.Gold -= cost;
                        if (state.Inventory.Gold > 0)
                        {
                            state.Inventory.Gold -= 1;
                            Ui.LogLine("[i]Receipt Fee -1[/i]");
                        }

                        HealPlayer(state, 10);
                        foreach (var t in state.Tourists.Where(t => t.Alive && !t.Gone))
                            t.Morale = Math.Min(100, t.Morale + 10);

                        Ui.LogLine("Pam hands out tiny cups of hot chocolate.");
                        Ui.LogLine("It tastes like sugar, lies, and brand identity.");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                case 2:
                    {
                        int cost = 16;

                        if (!CanAfford(state, cost)) break;

                        state.Inventory.Gold -= cost;

                        if (state.Inventory.Gold > 0)
                        {
                            state.Inventory.Gold -= 1;
                            Ui.LogLine("[i]Receipt Fee -1[/i]");
                        }

                        state.BonusDamageNextFight += 15;

                        Ui.LogLine("Pam sells you a foam sword.");
                        Ui.LogLine("It squeaks when you swing it.");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                case 3:
                    {
                        int cost = 20;

                        if (!CanAfford(state, cost)) break;

                        state.Inventory.Gold -= cost;

                        if (state.Inventory.Gold > 0)
                        {
                            state.Inventory.Gold -= 1;
                            Ui.LogLine("[i]Receipt Fee -1[/i]");
                        }

                        state.Inventory.HasTouristMap = true;

                        Ui.LogLine("Pam forces you into a t-shirt that reads:");
                        Ui.LogLine("[b]I SURVIVED THE DUNGEON[/b]");
                        Ui.LogLine("");
                        Ui.LogLine("You feel empowered. Your tourists feel marketed to.");
                        await Ui.Pause("Continue");
                        Ui.ClearLog();
                        break;
                    }

                default:
                    done = true;
                    break;
            }
        }
    }


    // -------------------------
    // Helpers
    // -------------------------
    private static MerchantType RollMerchant()
    {
        int r = _merchantRng.Next(0, 3);
        return (MerchantType)r;
    }

    private static bool CanAfford(GameState state, int cost)
    {
        if (state.Inventory.Gold < cost)
        {
            Ui.LogLine("Merchant: \"Ah. You are browsing with your imagination today.\"");
            Ui.LogLine("");
            return false;
        }

        return true;
    }

    private static void HealPlayer(GameState state, int amount)
    {
        if (amount <= 0) return;

        int before = state.Player.Hp;
        state.Player.Hp = Math.Min(state.Player.MaxHp, state.Player.Hp + amount);
        int healed = state.Player.Hp - before;

        Ui.LogLine($"You recover [b]{healed} HP[/b].");
    }

    private static readonly Random _merchantRng = new Random(Guid.NewGuid().GetHashCode());
}