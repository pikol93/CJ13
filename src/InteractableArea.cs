using System;
using Godot;

namespace Pikol93.CJ13;

public partial class InteractableArea : Area3D, IInteractable
{
    [Export] public String targetAnimation;
    [Export] public bool freeSelfOnInteraction;
    [Export] public String displayableName;
    [Export] public String displayableDescription;

    public bool IsDisplayable => displayableName != null && displayableName != "";
    public string DisplayableName => displayableName ?? "";
    public string DisplayableDescription => displayableDescription ?? "";

    public void Interact()
    {
        GD.Print($"Interacted with {Name}");
        if (targetAnimation == null || targetAnimation == "")
        {
            GD.PrintErr($"No animation set for node {Name}");
            return;
        }

        GameManager.GameAnimationTreeStateMachine.Travel(targetAnimation);
        if (freeSelfOnInteraction)
        {
            QueueFree();
        }
    }
}
