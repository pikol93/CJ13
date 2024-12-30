using System;
using Godot;

namespace Pikol93.CJ13;

public partial class HandReward : Area3D, IInteractable
{
    [Export] public String displayableName;
    [Export] public String displayableDescription;

    public bool IsDisplayable => displayableName != null && displayableName != "";
    public string DisplayableName => displayableName ?? "";
    public string DisplayableDescription => displayableDescription ?? "";

    string IInteractable.Name => Name;

    public void Interact()
    {
        GD.Print($"Interacted with {Name}");

        GameManager.HandSize += 1;

        GameManager.GameAnimationTreeStateMachine.Travel("hide_rewards");
    }
}
