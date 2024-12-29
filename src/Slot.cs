using Godot;

namespace Pikol93.CJ13;

public partial class Slot : Area3D, IInteractable
{
    private MeshInstance3D mesh;
    private StandardMaterial3D invisibleMaterial;
    private StandardMaterial3D availableMaterial;
    private StandardMaterial3D unavailableMaterial;

    public bool IsDisplayable => false;

    public string DisplayableName => "";

    public string DisplayableDescription => "";

    string IInteractable.Name => Name;

    private bool isInteractable = false;

    public int row = -1;
    public int column = -1;

    public void Interact()
    {
        if (!isInteractable)
        {
            GD.Print($"Slot {row} {column} is not interactable");
            return;
        }

        GetNode<Board>("../../..").OnSlotSelected(row, column);
    }

    public void MakeInvisibleUnpickable()
    {
        mesh.SetSurfaceOverrideMaterial(0, invisibleMaterial);
        isInteractable = false;
        InputRayPickable = false;
    }

    public void MakeInvisiblePickable()
    {
        mesh.SetSurfaceOverrideMaterial(0, invisibleMaterial);
        isInteractable = true;
        InputRayPickable = true;
    }

    public void MakeAvailable()
    {
        mesh.SetSurfaceOverrideMaterial(0, availableMaterial);
        isInteractable = true;
        InputRayPickable = true;
    }

    public void MakeUnavailable()
    {
        mesh.SetSurfaceOverrideMaterial(0, unavailableMaterial);
        isInteractable = false;
        InputRayPickable = true;
    }

    public override void _Ready()
    {
        mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        invisibleMaterial = GD.Load<StandardMaterial3D>("res://materials/slot_invisible.tres");
        availableMaterial = GD.Load<StandardMaterial3D>("res://materials/slot_available.tres");
        unavailableMaterial = GD.Load<StandardMaterial3D>("res://materials/slot_unavailable.tres");
    }
}
