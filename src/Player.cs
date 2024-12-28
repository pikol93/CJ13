using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Godot;

namespace Pikol93.CJ13;

public partial class Player : Node3D
{
    [Export] public float mouseSensitivity = 0.005f;
    [Export] public float lerpWeight = 0.12f;
    [Export] public float minVerticalAngle = -Mathf.Pi / 2.1f;
    [Export] public float maxVerticalAngle = Mathf.Pi / 2.1f;

    public static Player Instance { get; private set; }

    public Vector3 ExpectedLookTarget { get; set; } = new Vector3(0.0f, 1.0f, 0.0f);
    private Camera3D Camera { get; set; }

    public override void _Ready()
    {
        Instance = this;
        Camera = GetNode<Camera3D>("Camera");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("manual_look"))
        {
            CursorManager.SetDrag();
            return;
        }

        if (Input.IsActionJustReleased("manual_look"))
        {
            CursorManager.SetArrow();
        }

        var difference = ExpectedLookTarget - Camera.GlobalPosition;
        var distance = new Vector2(difference.X, difference.Z).Length();

        var normalizedOnHorizontalPlane = -new Vector2(-difference.Z, difference.X).Angle();
        var normalizedOnVerticalPlane = -new Vector2(distance, difference.Y).Angle();

        var newRotationHorizontal = Mathf.LerpAngle(Rotation.Y, normalizedOnHorizontalPlane, 0.1f);
        var newRotationVertical = Mathf.LerpAngle(Camera.Rotation.X, -normalizedOnVerticalPlane, 0.1f);

        Rotation = new Vector3(0.0f, newRotationHorizontal, 0.0f);
        Camera.Rotation = new Vector3(newRotationVertical, 0.0f, 0.0f);
    }

    public override void _Input(InputEvent ev)
    {
        if (ev is InputEventMouseMotion mouseMotion)
        {
            if (Input.IsActionPressed("manual_look"))
            {
                HandleLookingAround(mouseMotion);
            }
            else
            {
                HandleHoveringOverInteractables(mouseMotion);
            }
        }
        else if (ev is InputEventMouseButton mouseButton)
        {
            if (mouseButton.IsActionPressed("select"))
            {
                HandleSelectingInteractables(mouseButton);
            }
        }
    }

    private void HandleLookingAround(InputEventMouseMotion mouseMotion)
    {
        var diff = -(mouseMotion.Relative * mouseSensitivity);

        var newRotationHorizontal = Rotation.Y + diff.X;
        var newRotationVertical = Mathf.Clamp(Camera.Rotation.X + diff.Y, minVerticalAngle, maxVerticalAngle);

        Rotation = new Vector3(0.0f, newRotationHorizontal, 0.0f);
        Camera.Rotation = new Vector3(newRotationVertical, 0.0f, 0.0f);
    }

    private void HandleHoveringOverInteractables(InputEventMouseMotion mouseMotion)
    {
        var interactable = FindInteractable();
        GD.Print(interactable?.ToString());
    }

    private void HandleSelectingInteractables(InputEventMouseButton mouseButton)
    {
        FindInteractable();
    }

    private Interactable FindInteractable()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var mousePosition = GetViewport().GetMousePosition();
        var rayParams = new PhysicsRayQueryParameters3D
        {
            From = Camera.GlobalPosition,
            To = Camera.ProjectPosition(mousePosition, 1024.0f),
            CollisionMask = 2,
            CollideWithAreas = true,
        };

        var result = spaceState.IntersectRay(rayParams);
        if (result == null)
        {
            return null;
        }
        var collider = result["collider"].AsGodotObject();
        if (collider is Interactable interactable)
        {
            return interactable;
        }

        return null;
    }
}
