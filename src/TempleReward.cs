using System;
using Godot;

namespace Pikol93.CJ13;

public partial class TempleReward : Area3D, IInteractable
{
    private static readonly Random Random = new();

    [Export] public String displayableName;
    [Export] public String displayableDescription;

    public bool IsDisplayable => displayableName != null && displayableName != "";
    public string DisplayableName => displayableName ?? "";
    public string DisplayableDescription => displayableDescription ?? "";

    string IInteractable.Name => Name;

    public void Interact()
    {
        GD.Print($"Interacted with {Name}");
        var index = Random.Next(GameManager.MyDeck.Count);
        var card = GameManager.MyDeck[index];
        card.AttackEast += 1;
        card.AttackVertical += 1;
        card.AttackWest += 1;

        GameManager.GameAnimationTreeStateMachine.Travel("hide_rewards");
    }
}
