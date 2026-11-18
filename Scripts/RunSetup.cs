using Godot;
using System;
using System.Linq;
using DungeonTour.Content;
using DungeonTour.Core;
using DungeonTour.Models;

public partial class RunSetup : Control
{
    private ItemList _dungeonList = default!;
    private Button _startButton = default!;
    private Button _backButton = default!;
    private RichTextLabel _infoLabel = default!;

    public override void _Ready()
    {
        _dungeonList = GetNode<ItemList>("FadeGroup/Root/Center/Card/Margin/Layout/DungeonList");
        _infoLabel = GetNode<RichTextLabel>("FadeGroup/Root/Center/Card/Margin/Layout/InfoLabel");
        _startButton = GetNode<Button>("FadeGroup/Root/Center/Card/Margin/Layout/Buttons/StartButton");
        _backButton = GetNode<Button>("FadeGroup/Root/Center/Card/Margin/Layout/Buttons/BackButton");

        _startButton.Pressed += OnStartPressed;
        _backButton.Pressed += OnBackPressed;

        _dungeonList.ItemSelected += OnDungeonSelected;

        PopulateDungeonList();
        SelectDefaultDungeon();
    }

    private void PopulateDungeonList()
    {
        _dungeonList.Clear();

        var all = DungeonDatabase.GetAll();
        if (all == null || all.Count == 0)
        {
            _infoLabel.Clear();
            _infoLabel.AppendText("[b]No dungeons found[/b]\n");
            _startButton.Disabled = true;
            return;
        }

        foreach (var d in all)
            _dungeonList.AddItem(d.Name);

        _startButton.Disabled = false;
    }

    private void SelectDefaultDungeon()
    {
        var all = DungeonDatabase.GetAll();
        if (all == null || all.Count == 0)
            return;

        int idx = all.FindIndex(d => d.Id == "dungeon_01");
        if (idx < 0) idx = 0;

        _dungeonList.Select(idx);
        UpdateDetailsForIndex(idx);
    }

    private void OnDungeonSelected(long index)
    {
        UpdateDetailsForIndex((int)index);
    }

    private void UpdateDetailsForIndex(int index)
    {
        var all = DungeonDatabase.GetAll();
        if (all == null || all.Count == 0)
            return;

        if (index < 0 || index >= all.Count)
            return;

        var d = all[index];

        _infoLabel.Clear();

        _infoLabel.AppendText($"[b]{d.Name}[/b]\n");
        _infoLabel.AppendText($"Rooms: {d.RoomsToExit}\n");
        _infoLabel.AppendText($"Enemy HP: {d.BaseEnemyHp}+ scaling\n");
        _infoLabel.AppendText($"Enemy Damage: {d.BaseEnemyDamage}+ scaling\n\n");

        if (d.EnemyPool.Count > 0)
        {
            _infoLabel.AppendText("[b]Enemies:[/b]\n");

            foreach (var e in d.EnemyPool.Take(6))
                _infoLabel.AppendText($"• {e.Name}\n");

            if (d.EnemyPool.Count > 6)
                _infoLabel.AppendText("• ...\n");
        }
    }

    private void OnStartPressed()
    {
        var all = DungeonDatabase.GetAll();
        if (all == null || all.Count == 0)
            return;

        int selected = _dungeonList.GetSelectedItems().FirstOrDefault();

        if (selected < 0 || selected >= all.Count)
            selected = 0;

        var picked = all[selected];
        RunConfig.SelectedDungeonId = picked.Id;

        GetTree().ChangeSceneToFile("res://Scenes/GameShell.tscn");
    }

    private void OnBackPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
    }
}