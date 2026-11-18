#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DungeonTour.Core;
using DungeonTour.Models;
using DungeonTour.Systems;
using DungeonTour.Content;

public partial class GameShell : Control, IUiBridge
{
    // -------------------------
    // UI Nodes
    // -------------------------
    private Control _mainPanel = default!;

    private Label _titleLabel = default!;
    private RichTextLabel _hudLabel = default!;
    private RichTextLabel _logText = default!;

    private Button _optionA = default!;
    private Button _optionB = default!;
    private Button _optionC = default!;
    private Button _optionD = default!;

    private PanelContainer _rosterPopup = default!;
    private RichTextLabel _rosterText = default!;
    private Button _closeRosterButton = default!;

    // In-game menu (simple modal)
    private PanelContainer _menuPopup = default!;
    private Button _menuResumeButton = default!;
    private Button _menuSaveButton = default!;
    private Button _menuLoadButton = default!;
    private Button _menuQuitButton = default!;
    private bool _menuOpen = false;

    // Keep a backup of pending options while the menu is open so we can restore them
    private List<string>? _menuSavedOptions;
    private bool _menuSavedHasPendingChoice;

    // -------------------------
    // Audio
    // -------------------------
    private AudioStreamPlayer _sfxClick = default!;
    private AudioStreamPlayer _sfxSelect = default!;
    private AudioStreamPlayer _sfxHit = default!;
    private AudioStreamPlayer _music = default!;

    // -------------------------
    // UI bridge state
    // -------------------------
    private readonly List<Button> _optionButtons = new();
    private bool _rosterOpen = false;

    private TaskCompletionSource<int>? _pendingChoice;
    private List<string> _pendingOptions = new();

    // -------------------------
    // Game state
    // -------------------------
    private GameState? _state;

    // -------------------------
    // Shake state
    // -------------------------
    private Vector2 _mainPanelBasePos;
    private Tween? _shakeTween;

    public override void _Ready()
    {
        GrabNodes();
        WireEvents();
        StartMusic();

        Ui.Bind(this);

        _mainPanelBasePos = _mainPanel.Position;

        StartGameAsync().ContinueWith(t =>
        {
            if (t.Exception != null)
                GD.PrintErr(t.Exception.ToString());
        });
    }

    // -------------------------
    // Main entry
    // -------------------------
    private async Task StartGameAsync()
    {
        _state = CreateNewGameState();
        bool isLoadedGame = _state.RoomIndex > 0;

        GD.Print($"[RUN] SelectedDungeonId={RunConfig.SelectedDungeonId} StateDungeonId={_state.DungeonId} RoomsToExit={_state.RoomsToExit} RoomIndex={_state.RoomIndex} isLoaded={isLoadedGame} HP={_state.Player.Hp}/{_state.Player.MaxHp} Gold={_state.Inventory.Gold} Potions={_state.Inventory.Potions} RngSeed={_state.RngSeed}");

        _titleLabel.Text = "Dungeon Tour: Liability Edition";

        _logText.Clear();

        if (isLoadedGame)
        {
            AddLogLine("[b]Save loaded.[/b]");
            AddLogLine($"Resuming from room {_state.RoomIndex + 1}/{_state.RoomsToExit}.");
            AddLogLine("");
        }
        else
        {
            AddLogLine("[b]Welcome, tour guide.[/b]");
            AddLogLine("You lead a group of tourists into a dungeon that was not built for joy.");
            AddLogLine("");
        }

        UpdateHud(_state!);

        await Ui.Pause(isLoadedGame ? "Continue" : "Start");

        await RunDungeonAsync();
    }

    // -------------------------
    // Main loop
    // -------------------------
    private async Task RunDungeonAsync()
    {
        if (_state == null) throw new InvalidOperationException("Game state not initialized.");

        while (!_state.GameOver)
        {
            if (_state.RoomIndex >= _state.RoomsToExit)
            {
                _state.GameOver = true;
                _state.Won = true;
                break;
            }

            if (_state.Player.Hp <= 0)
            {
                _state.GameOver = true;
                _state.Won = false;
                break;
            }

            UpdateHud(_state);

            _state.RoomsSinceRest++;
            await RestSystem.TryTriggerSnusLeaveAsync(_state);

            // Use saved room options if available (from a loaded save), otherwise generate new ones
            List<EncounterOption> roomOptions;
            if (_state.PendingRoomOptions != null && _state.PendingRoomOptions.Count > 0)
            {
                roomOptions = _state.PendingRoomOptions;
                GD.Print($"[RUN] Restored {roomOptions.Count} saved room options for room {_state.RoomIndex + 1}");
            }
            else
            {
                DungeonTour.Models.Node node = NodeGenerator.GenerateNode(_state);
                roomOptions = node.Options.Take(3).ToList();
            }

            // Store current options so they survive a save mid-choice
            _state.PendingRoomOptions = new List<EncounterOption>(roomOptions);

            ClearLog();
            AddLogLine($"[b]Room {_state.RoomIndex + 1}/{_state.RoomsToExit}[/b]");
            AddLogLine("Choose a path:");
            AddLogLine("");

            var optionTexts = roomOptions
                .Select(o => o.Description)
                .ToList();

            bool canUsePotion = _state.Inventory.Potions > 0 && _state.Player.Hp < _state.Player.MaxHp;

            if (canUsePotion)
            {
                int healPreview = (int)Math.Round(_state.Player.MaxHp * 0.4);
                if (healPreview < 10) healPreview = 10;

                optionTexts.Add($"Drink Soda (+{healPreview} HP)");
            }

            for (int i = 0; i < optionTexts.Count; i++)
            {
                char label = (char)('A' + i);

                // Show potion icon in the log for the potion option
                if (canUsePotion && i == optionTexts.Count - 1)
                {
                    int healPreview = (int)Math.Round(_state.Player.MaxHp * 0.4);
                    if (healPreview < 10) healPreview = 10;
                    AddLogLine($"{label}) Use [img=40x40]res://Assets/Icons/Drink.png[/img] (+{healPreview} HP)");
                }
                else
                {
                    AddLogLine($"{label}) {optionTexts[i]}");
                }
            }

            AddLogLine("");
            AddLogLine("Choose:");

            int choiceIndex = await Ui.ReadLetterChoiceIndex(optionTexts.Count);

            if (canUsePotion && choiceIndex == roomOptions.Count)
            {
                _state.Inventory.Potions--;

                int heal = (int)Math.Round(_state.Player.MaxHp * 0.4);
                if (heal < 10) heal = 10;

                int before = _state.Player.Hp;
                _state.Player.Hp = Math.Min(_state.Player.MaxHp, _state.Player.Hp + heal);
                int actual = _state.Player.Hp - before;

                ClearLog();
                AddLogLine($"You drink a potion. Heal: {actual}. Potions left: {_state.Inventory.Potions}.");
                AddLogLine($"HP: {_state.Player.Hp}/{_state.Player.MaxHp}");

                UpdateHud(_state);

                await Ui.Pause("Continue");

                continue;
            }

            EncounterOption picked = roomOptions[choiceIndex];

            // Player made their choice — clear pending options so next room generates fresh ones
            _state.PendingRoomOptions = null;

            ClearLog();
            AddLogLine(picked.Description);
            AddLogLine("");

            await RunEncounterAsync(picked);

            if (_state.Player.Hp <= 0)
            {
                _state.GameOver = true;
                _state.Won = false;
                break;
            }

            _state.RoomIndex++;

            UpdateHud(_state);
            await Ui.Pause("Continue");

            ClearLog();
        }

        ClearLog();

        if (_state.Won)
        {
            AddLogLine("[b]You made it out.[/b]");
            AddLogLine("Your tourists are alive enough to sue you.");
        }
        else
        {
            AddLogLine("[b]You died in the dungeon.[/b]");
            AddLogLine("The tourists will remember this in court.");
        }

        await Ui.Pause("Quit");
        GetTree().Quit();
    }

    private async Task RunEncounterAsync(EncounterOption option)
    {
        if (_state == null) throw new InvalidOperationException("Game state not initialized.");

        switch (option.Type)
        {
            case EncounterType.Fight:
                await CombatSystem.RunFight(_state);
                break;

            case EncounterType.Rest:
                RestSystem.RunRest(_state);
                break;

            case EncounterType.Merchant:
                await MerchantSystem.RunMerchant(_state);
                break;

            case EncounterType.Npc:
                await NpcSystem.RunNpc(_state, option.SpecialComboId);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // -------------------------
    // Create new game
    // -------------------------
    private GameState CreateNewGameState()
    {
        // If MainMenu placed a loaded state in RunConfig, consume it here.
        if (RunConfig.LoadedState != null)
        {
            var loaded = RunConfig.LoadedState;
            RunConfig.LoadedState = null;

            // Ensure transient fields are valid
            loaded.DescriptionDeck ??= new DescriptionDeck();
            loaded.SeenNpcEventIds ??= new HashSet<string>();
            loaded.TriggeredSpecialComboIds ??= new HashSet<string>();

            // Make sure HUD-usable fields exist
            loaded.Player ??= new Player("Guide", 40, 8);
            loaded.Inventory ??= new Inventory(0, 0);
            loaded.Tourists ??= TouristFactory.CreateParty(8);

            // Ensure RoomsToExit is set from dungeon definition if available
            var loadedDungeon = DungeonDatabase.GetAll().FirstOrDefault(d => d.Id == loaded.DungeonId);
            loaded.RoomsToExit = loadedDungeon?.RoomsToExit ?? loaded.RoomsToExit;

            // RNG seed was already restored in SaveManager.Load()

            GD.Print($"[GameShell] Consuming loaded state. DungeonId={loaded.DungeonId} RoomIndex={loaded.RoomIndex}");
            return loaded;
        }

        // New game — generate and lock in a seed
        int seed = Rng.GenerateSeed();
        Rng.SetSeed(seed);

        var state = new GameState();
        state.RngSeed = seed;

        // Viktig: detta är valet från RunSetup
        state.DungeonId = RunConfig.SelectedDungeonId;

        // Hämta dungeon-data
        var dungeon = DungeonDatabase.GetAll()
            .FirstOrDefault(d => d.Id == state.DungeonId);

        // Fallback om något är fel
        if (dungeon == null)
        {
            GD.PrintErr($"[GameShell] Dungeon '{state.DungeonId}' not found. Falling back to dungeon_01.");
            dungeon = DungeonDatabase.GetAll().FirstOrDefault(d => d.Id == "dungeon_01");
            state.DungeonId = dungeon?.Id ?? "dungeon_01";
        }

        state.Player = new Player("Guide", 40, 8);
        state.Inventory = new Inventory(2, 20);

        // Om dungeon finns: ta RoomsToExit från den
        state.RoomsToExit = dungeon?.RoomsToExit ?? 12;

        state.RoomIndex = 0;

        state.GameOver = false;
        state.Won = false;

        state.DescriptionDeck = new DescriptionDeck();

        state.Tourists = TouristFactory.CreateParty(8);

        foreach (var t in state.Tourists)
        {
            t.Alive = true;
            t.Morale = 70;
        }

        return state;
    }

    // -------------------------
    // Setup
    // -------------------------
    private void GrabNodes()
    {
        _mainPanel = GetNode<Control>("Background/MainPanel");

        _titleLabel = GetNode<Label>("Background/MainPanel/Margin/Layout/TitleLabel");
        _hudLabel = GetNode<RichTextLabel>("Background/MainPanel/Margin/Layout/HudLabel");
        _logText = GetNode<RichTextLabel>("Background/MainPanel/Margin/Layout/LogPanel/LogScroll/LogContent/LogText");

        _optionA = GetNode<Button>("Background/MainPanel/Margin/Layout/OptionsPanel/OptionsBox/OptionA");
        _optionB = GetNode<Button>("Background/MainPanel/Margin/Layout/OptionsPanel/OptionsBox/OptionB");
        _optionC = GetNode<Button>("Background/MainPanel/Margin/Layout/OptionsPanel/OptionsBox/OptionC");
        _optionD = GetNode<Button>("Background/MainPanel/Margin/Layout/OptionsPanel/OptionsBox/OptionD");

        _rosterPopup = GetNode<PanelContainer>("RosterPopup");
        _rosterText = GetNode<RichTextLabel>("RosterPopup/VBoxContainer/RosterText");
        _closeRosterButton = GetNode<Button>("RosterPopup/VBoxContainer/CloseRosterButton");

        // Menu nodes — add a PanelContainer named "MenuPopup" to the GameShell scene with the below child structure:
        // MenuPopup/VBoxContainer/ResumeButton
        // MenuPopup/VBoxContainer/SaveButton
        // MenuPopup/VBoxContainer/LoadButton
        // MenuPopup/VBoxContainer/QuitButton
        _menuPopup = GetNode<PanelContainer>("MenuPopup");
        _menuResumeButton = GetNode<Button>("MenuPopup/VBoxContainer/ResumeButton");
        _menuSaveButton = GetNode<Button>("MenuPopup/VBoxContainer/SaveButton");
        _menuLoadButton = GetNode<Button>("MenuPopup/VBoxContainer/LoadButton");
        _menuQuitButton = GetNode<Button>("MenuPopup/VBoxContainer/QuitButton");

        _sfxClick = GetNode<AudioStreamPlayer>("SfxClick");
        _sfxSelect = GetNode<AudioStreamPlayer>("SfxSelect");
        _sfxHit = GetNode<AudioStreamPlayer>("SfxHit");
        _music = GetNode<AudioStreamPlayer>("Music");

        _optionButtons.Clear();
        _optionButtons.Add(_optionA);
        _optionButtons.Add(_optionB);
        _optionButtons.Add(_optionC);
        _optionButtons.Add(_optionD);
    }

    private void WireEvents()
    {
        _optionA.Pressed += () => OnOptionPressed(0);
        _optionB.Pressed += () => OnOptionPressed(1);
        _optionC.Pressed += () => OnOptionPressed(2);
        _optionD.Pressed += () => OnOptionPressed(3);

        foreach (var b in _optionButtons)
            b.MouseEntered += () => PlayClick();

        _closeRosterButton.Pressed += () =>
        {
            PlayClick();
            CloseRoster();
        };

        // Menu wiring
        _menuResumeButton.Pressed += () =>
        {
            PlayClick();
            CloseMenu();
        };
        _menuSaveButton.Pressed += () =>
        {
            PlayClick();
            OnMenuSavePressed();
        };
        _menuLoadButton.Pressed += () =>
        {
            PlayClick();
            OnMenuLoadPressed();
        };
        _menuQuitButton.Pressed += () =>
        {
            PlayClick();
            OnQuitToMainPressed();
        };
    }

    private void StartMusic()
    {
        if (_music == null) return;
        if (!_music.IsConnected(AudioStreamPlayer.SignalName.Finished, Callable.From(OnMusicFinished)))
            _music.Finished += OnMusicFinished;
        if (!_music.Playing)
            _music.Play();
    }

    private void OnMusicFinished()
    {
        if (_music == null) return;
        _music.Play();
    }

    // -------------------------
    // UI helpers
    // -------------------------
    private void AddLogLine(string text)
    {
        _logText.AppendText(text + "\n");
        CallDeferred(nameof(ScrollLogToBottom));
    }

    private void ScrollLogToBottom()
    {
        _logText.ScrollToLine(_logText.GetLineCount());
    }

    private void SetOptions(List<string> optionTexts)
    {
        for (int i = 0; i < _optionButtons.Count; i++)
        {
            _optionButtons[i].Visible = false;
            _optionButtons[i].Disabled = true;
            _optionButtons[i].Text = "";
        }

        for (int i = 0; i < optionTexts.Count && i < 4; i++)
        {
            _optionButtons[i].Visible = true;
            _optionButtons[i].Disabled = false;
            _optionButtons[i].Text = optionTexts[i];
        }
    }

    private void ClearOptions()
    {
        for (int i = 0; i < _optionButtons.Count; i++)
        {
            _optionButtons[i].Visible = false;
            _optionButtons[i].Disabled = true;
            _optionButtons[i].Text = "";
        }
    }

    // -------------------------
    // Input
    // -------------------------
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent) return;
        if (!keyEvent.Pressed || keyEvent.Echo) return;

        // Toggle in-game menu (map "ui_gamemenu" to Esc in InputMap)
        if (Input.IsActionJustPressed("ui_gamemenu"))
        {
            PlayClick();
            if (_menuOpen) CloseMenu();
            else OpenMenu();
            return;
        }

        if (Input.IsActionJustPressed("ui_roster"))
        {
            PlayClick();
            if (_rosterOpen) CloseRoster();
            else OpenRoster();
            return;
        }

        // keep ui_quit as an immediate quit (or remap if you want different behaviour)
        if (Input.IsActionJustPressed("ui_quit"))
        {
            if (_rosterOpen)
            {
                PlayClick();
                CloseRoster();
                return;
            }

            GetTree().Quit();
            return;
        }

        if (_rosterOpen || _menuOpen) return;

        if (Input.IsActionJustPressed("ui_options_a")) { OnOptionPressed(0); return; }
        if (Input.IsActionJustPressed("ui_options_b")) { OnOptionPressed(1); return; }
        if (Input.IsActionJustPressed("ui_options_c")) { OnOptionPressed(2); return; }
        if (Input.IsActionJustPressed("ui_options_d")) { OnOptionPressed(3); return; }

        if (Input.IsActionJustPressed("ui_num_1")) { OnOptionPressed(0); return; }
        if (Input.IsActionJustPressed("ui_num_2")) { OnOptionPressed(1); return; }
        if (Input.IsActionJustPressed("ui_num_3")) { OnOptionPressed(2); return; }
        if (Input.IsActionJustPressed("ui_num_4")) { OnOptionPressed(3); return; }
    }

    private void OnOptionPressed(int index)
    {
        if (index < 0 || index >= _optionButtons.Count) return;
        GD.Print($"[UI] Click option {index}. PendingChoice={(_pendingChoice != null)} PendingOptions={_pendingOptions.Count}");

        var b = _optionButtons[index];
        if (!b.Visible || b.Disabled) return;

        PlaySelect();

        if (_pendingChoice != null)
        {
            if (_pendingOptions.Count == 0) return;
            if (index >= _pendingOptions.Count) return;

            var tcs = _pendingChoice;
            _pendingChoice = null;

            GD.Print($"[UI] Resolving choice index={index}");
            tcs.TrySetResult(index);
            GD.Print($"[UI] Choice resolved. PendingChoice now null={(_pendingChoice == null)}");
            ClearOptions();
            return;
        }
    }

    // -------------------------
    // Roster
    // -------------------------
    private void OpenRoster()
    {
        if (_state == null)
            return;

        _rosterOpen = true;
        _rosterPopup.Visible = true;

        _rosterText.Clear();
        _rosterText.AppendText("[font_size=32][b]Tourists[/b][/font_size]\n\n");

        foreach (var t in _state.Tourists)
        {
            string status = "";

            if (t.Gone)
                status = " [color=#aaaa44](GONE)[/color]";
            else if (!t.Alive)
                status = " [color=#aa4444](DEAD)[/color]";

            _rosterText.AppendText($"• {t.Name}{status} — Morale {t.Morale}\n");
        }
    }

    private void CloseRoster()
    {
        _rosterOpen = false;
        _rosterPopup.Visible = false;
    }

    // -------------------------
    // In-game menu
    // -------------------------
    private void OpenMenu()
    {
        _menuOpen = true;
        _menuPopup.Visible = true;

        // save current option state so we can restore it later
        _menuSavedOptions = _pendingOptions != null ? new List<string>(_pendingOptions) : null;
        _menuSavedHasPendingChoice = _pendingChoice != null;

        // hide options behind menu
        ClearOptions();
    }

    private void CloseMenu()
    {
        _menuOpen = false;
        _menuPopup.Visible = false;

        // restore options if we had before opening menu
        if (_menuSavedOptions != null && _menuSavedOptions.Count > 0)
        {
            _pendingOptions = new List<string>(_menuSavedOptions);
            SetOptions(_pendingOptions);
        }
        else
        {
            ClearOptions();
        }

        _menuSavedOptions = null;
        _menuSavedHasPendingChoice = false;
    }

    private void OnMenuSavePressed()
    {
        if (_state == null) return;

        bool ok = SaveManager.Save(_state);
        if (ok)
            AddLogLine("[b]Game saved.[/b]");
        else
            AddLogLine("[b]Save failed.[/b]");

        CloseMenu();
    }

    private void OnMenuLoadPressed()
    {
        var loaded = SaveManager.Load();
        if (loaded == null)
        {
            AddLogLine("[b]No save or load failed.[/b]");
            CloseMenu();
            return;
        }

        CloseMenu();

        // Restart the entire scene so RunDungeonAsync begins fresh with the loaded state.
        RunConfig.LoadedState = loaded;
        GetTree().ChangeSceneToFile("res://Scenes/GameShell.tscn");
    }

    private void OnQuitToMainPressed()
    {
        // Optional: auto-save before quitting to main
        // if (_state != null) SaveManager.Save(_state);

        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }

    // -------------------------
    // HUD
    // -------------------------
    private static string GetHpColor(int hp, int maxHp)
    {
        double ratio = (double)hp / maxHp;
        if (ratio <= 0.1)
            return "#dd4444"; // red
        if (ratio <= 0.5)
            return "#dddd44"; // yellow
        return "#44dd44";     // green
    }

    private void UpdateHud(GameState state)
    {
        int alive = state.Tourists.Count(t => t.Alive && !t.Gone);
        string hpColor = GetHpColor(state.Player.Hp, state.Player.MaxHp);

        _hudLabel.Clear();
        _hudLabel.AppendText(
            $"HP [color={hpColor}]{state.Player.Hp}/{state.Player.MaxHp}[/color]   " +
            $"Gold [color=#f5c842]{state.Inventory.Gold}[/color]   " +
            $"[img=40x40]res://Assets/Icons/Drink.png[/img] {state.Inventory.Potions}   " +
            $"Tourists {alive}/{state.Tourists.Count}   " +
            $"Room {state.RoomIndex + 1}/{state.RoomsToExit}");
    }

    // -------------------------
    // Audio helpers
    // -------------------------
    private void PlayClick() => _sfxClick?.Play();
    private void PlaySelect() => _sfxSelect?.Play();
    private void PlayHit() => _sfxHit?.Play();

    // -------------------------
    // IUiBridge
    // -------------------------
    public void LogLine(string text) => AddLogLine(text);
    public void ClearLog() => _logText.Clear();

    public void SetHud(object state)
    {
        if (state is GameState gs)
            UpdateHud(gs);
    }

    public Task<int> WaitForChoiceAsync(IReadOnlyList<string> options)
    {
        GD.Print($"[UI] WaitForChoiceAsync called with {options.Count} options: {string.Join(" | ", options)}");
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (_state != null)
            UpdateHud(_state);

        if (options.Count < 1 || options.Count > 4)
            throw new ArgumentOutOfRangeException(nameof(options), "UI supports 1..4 options.");

        if (_pendingChoice != null)
        {
            _pendingChoice = null;
            _pendingOptions.Clear();
            ClearOptions();
        }

        _pendingOptions = new List<string>(options);
        SetOptions(_pendingOptions);

        _pendingChoice = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        GD.Print("[UI] WaitForChoiceAsync: pending choice created");
        return _pendingChoice.Task;
    }

    public async Task WaitForContinueAsync(string continueText = "Continue")
    {
        int _ = await WaitForChoiceAsync(new List<string> { continueText });
    }

    public void HitShake()
    {
        if (_shakeTween != null && _shakeTween.IsRunning())
        {
            _shakeTween.Kill();
            _mainPanel.Position = _mainPanelBasePos;
        }

        float strength = 10f;
        float stepTime = 0.04f;

        _shakeTween = CreateTween();
        _shakeTween.SetEase(Tween.EaseType.Out);
        _shakeTween.SetTrans(Tween.TransitionType.Sine);

        _shakeTween.TweenProperty(_mainPanel, "position", _mainPanelBasePos + new Vector2(-strength, 0), stepTime);
        _shakeTween.TweenProperty(_mainPanel, "position", _mainPanelBasePos + new Vector2(strength, 0), stepTime);
        _shakeTween.TweenProperty(_mainPanel, "position", _mainPanelBasePos + new Vector2(-strength * 0.6f, 0), stepTime);
        _shakeTween.TweenProperty(_mainPanel, "position", _mainPanelBasePos + new Vector2(strength * 0.6f, 0), stepTime);
        _shakeTween.TweenProperty(_mainPanel, "position", _mainPanelBasePos, stepTime);
    }
}
