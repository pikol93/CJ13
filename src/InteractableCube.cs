using Godot;
using System;

namespace Pikol93.CJ13;

public partial class InteractableCube : Node, DisplayableInteractable
{
    public string DisplayableName => $"Interactable cube {Name}";

    public string DisplayableDescription => "Designed for testing.";

    public void OnMouseEntered()
    {
        GD.Print($"Mouse entered {Name}");
    }

    public void OnMouseExited()
    {
        GD.Print($"Mouse exited {Name}");
    }
}
