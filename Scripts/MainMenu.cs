using Godot;
using DungeonTour.Core;

public partial class MainMenu : Control
{
    private Button _startButton = default!;
    private Button _howButton = default!;
    private Button _exitButton = default!;
    private Button _loadButton = default!;
    private PanelContainer _howPopup = default!;
    private RichTextLabel _howText = default!;
    private Button _closeHowButton = default!;
    private ColorRect _fadeOverlay = default!;
    private const float FadeSeconds = 1.0f;


    public override void _Ready()
    {
        GD.Randomize();

        _startButton = GetNode<Button>("FadeGroup/Center/Card/Margin/Layout/Buttons/StartButton");
        _loadButton = GetNode<Button>("FadeGroup/Center/Card/Margin/Layout/Buttons/LoadButton");
        var torchA = GetNode<AnimatedSprite2D>("FadeGroup/Center/Card/Torch");
        torchA.Play("idle");
        var torchB = GetNode<AnimatedSprite2D>("FadeGroup/Center/Card/Torch2");
        torchB.Play("idle");
        torchA.SpeedScale = (float)GD.RandRange(2.5, 3.0);
        torchB.SpeedScale = (float)GD.RandRange(2.5, 3.0);
        StartTorchFlicker(torchA);
        StartTorchFlicker(torchB);

        var flameA = GetNode<AnimatedSprite2D>("FadeGroup/Center/Card/Flame");
        flameA.Play("default");
        var flameB = GetNode<AnimatedSprite2D>("FadeGroup/Center/Card/Flame2");
        flameB.Play("default");
        //flameA.SpeedScale = (float)GD.RandRange(2.5, 3.0);
        //flameB.SpeedScale = (float)GD.RandRange(2.5, 3.0);
        //StartTorchFlicker(flameA);
        //StartTorchFlicker(flameB);



        _howButton = GetNode<Button>("FadeGroup/Center/Card/Margin/Layout/Buttons/HowButton");
        _exitButton = GetNode<Button>("FadeGroup/Center/Card/Margin/Layout/Buttons/ExitButton");

        _startButton.Pressed += OnStartPressed;
        _loadButton.Pressed += OnLoadPressed;
        _howButton.Pressed += OnHowPressed;
        _exitButton.Pressed += OnExitPressed;
        _howPopup = GetNode<PanelContainer>("FadeGroup/HowPopup");
        _howText = GetNode<RichTextLabel>("FadeGroup/HowPopup/Margin/Layout/HowText");
        _closeHowButton = GetNode<Button>("FadeGroup/HowPopup/Margin/Layout/CloseHowButton");
        _startButton.GrabFocus();
        _fadeOverlay = GetNode<ColorRect>("FadeGroup/FadeOverlay");

        // disable load button if no save
        _loadButton.Disabled = !SaveManager.SaveExists();

        _fadeOverlay.Modulate = new Color(1, 1, 1, 1);
        CreateTween().TweenProperty(_fadeOverlay, "modulate:a", 0.0f, FadeSeconds);

        _closeHowButton.Pressed += CloseHowPopup;
    }



    private async void OnStartPressed()
    {
        _startButton.Disabled = true;
        _howButton.Disabled = true;
        _exitButton.Disabled = true;
        _loadButton.Disabled = true;

        var tween = CreateTween();
        tween.TweenProperty(_fadeOverlay, "modulate:a", 1.0f, FadeSeconds);

        tween.Finished += async () =>
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            GetTree().ChangeSceneToFile("res://Scenes/RunSetup.tscn");
        };
    }

    private async void OnLoadPressed()
    {
        // Attempt to load immediately so we can display an error if it fails.
        var saved = SaveManager.Load();
        if (saved == null)
        {
            _howPopup.Visible = true;
            _howText.Clear();
            _howText.AppendText("[b]No save found or failed to load.[/b]\n");
            _closeHowButton.GrabFocus();
            return;
        }

        // place loaded state for GameShell to consume
        RunConfig.LoadedState = saved;

        _startButton.Disabled = true;
        _howButton.Disabled = true;
        _exitButton.Disabled = true;
        _loadButton.Disabled = true;

        var tween = CreateTween();
        tween.TweenProperty(_fadeOverlay, "modulate:a", 1.0f, FadeSeconds);

        tween.Finished += async () =>
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

            GetTree().ChangeSceneToFile("res://Scenes/GameShell.tscn");
        };
    }

    private void StartTorchFlicker(AnimatedSprite2D torch)
    {
        torch.Modulate = new Color(1f, 1f, 1f, 1f);

        var tween = CreateTween();
        tween.SetLoops();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(torch, "modulate", new Color(1.08f, 1.02f, 0.95f, 1f), 0.35f);
        tween.TweenProperty(torch, "modulate", new Color(0.95f, 0.95f, 1.05f, 1f), 0.35f);
        tween.TweenProperty(torch, "modulate", new Color(1f, 1f, 1f, 1f), 0.35f);
    }





    private void OnHowPressed()
    {
        _howPopup.Visible = true;

        _howText.Clear();
        _howText.AppendText(
            "[b]HOW IT WORKS[/b]\n\n" +
            "• You are the tour guide.\n" +
            "• You must escort 8 tourists through 12 rooms.\n" +
            "• The dungeon will repeatedly ask you to fight, rest, shop, or deal with NPCs.\n\n" +
            "[b]CONTROLS[/b]\n\n" +
            "• Click buttons OR press A/B/C/D.\n" +
            "• Press T to view your tourists.\n\n" +
            "[b]IMPORTANT[/b]\n\n" +
            "• Tourists can be lost.\n" +
            "• The dungeon does not care.\n" +
            "• Liability waivers are not legally binding in Hell.\n"
        );

        _closeHowButton.GrabFocus();
    }

    private void CloseHowPopup()
    {
        _howPopup.Visible = false;
        _startButton.GrabFocus();
    }



    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
