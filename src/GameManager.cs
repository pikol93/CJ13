using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public partial class GameManager : Node
{
    private static readonly TimeSpan DefaultPlayerInteractionBlockage = TimeSpan.FromSeconds(3);

    public static DateTime? PlayerInteractionBlockTime { get; set; }
    public static TimeSpan? PlayerInteractionBlockageOverride { get; set; }

    public static TimeSpan PlayerInteractionBlockage
    {
        get => PlayerInteractionBlockageOverride ?? DefaultPlayerInteractionBlockage;
    }
    public static bool InteractionAtAnyTimeOverride { get; set; } = false;

    public static bool CanPlayerInteract
    {
        get => InteractionAtAnyTimeOverride
            || PlayerInteractionBlockTime == null
            || PlayerInteractionBlockTime?.Add(PlayerInteractionBlockage) < DateTime.Now;
    }

    public static AnimationTree GameAnimationTree { get; private set; }
    public static AnimationNodeStateMachinePlayback GameAnimationTreeStateMachine => (AnimationNodeStateMachinePlayback)GameAnimationTree.Get("parameters/playback").AsGodotObject();

    public static List<Card> MyDeck { get; private set; }
    public static int EnemyCardCount { get; set; } = 3;

    public override void _Ready()
    {
        GameAnimationTree = (AnimationTree)GetTree().GetNodesInGroup("game_animation_tree")[0];
        MyDeck = Deck.GenerateDeck(5);
    }

    public override void _Input(InputEvent ev)
    {
        if (Input.IsActionJustPressed("toggle_interaction_at_any_time"))
        {
            InteractionAtAnyTimeOverride = !InteractionAtAnyTimeOverride;
            GD.Print($"Toggled interaction override: {InteractionAtAnyTimeOverride}");
        }
    }

    public static void ValidateDeck()
    {
        MyDeck.RemoveAll(card => !IsInstanceValid(card) || card.IsQueuedForDeletion());
    }
}
