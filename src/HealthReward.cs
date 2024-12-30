using System;
using Godot;

namespace Pikol93.CJ13;

public partial class HealthReward : Area3D, IInteractable
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
        GameManager.MyDeck[index].Health += 2;

        GameManager.GameAnimationTreeStateMachine.Travel("hide_rewards");
    }
}
