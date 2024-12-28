using System.Runtime.Versioning;
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
            return;
        }

        var normalized = GlobalPosition.MoveToward(ExpectedLookTarget, 1.0f);

        var targetRotationHorizontal = Mathf.Atan2(normalized.X, normalized.Z);
        var targetRotationVertical = -Mathf.Atan2(normalized.Z, normalized.Y);

        var newRotationHorizontal = Mathf.LerpAngle(Rotation.Y, targetRotationHorizontal, 0.1f);
        var newRotationVertical = Mathf.LerpAngle(Camera.Rotation.X, targetRotationVertical, 0.1f);

        Rotation = new Vector3(0.0f, newRotationHorizontal, 0.0f);
        Camera.Rotation = new Vector3(newRotationVertical, 0.0f, 0.0f);
    }

    public override void _Input(InputEvent ev)
    {
        if (ev is not InputEventMouseMotion mouseMotion)
        {
            return;
        }

        if (!Input.IsActionPressed("manual_look"))
        {
            return;
        }

        var diff = -(mouseMotion.Relative * mouseSensitivity);

        var newRotationHorizontal = Rotation.Y + diff.X;
        var newRotationVertical = Mathf.Clamp(Camera.Rotation.X + diff.Y, minVerticalAngle, maxVerticalAngle);

        Rotation = new Vector3(0.0f, newRotationHorizontal, 0.0f);
        Camera.Rotation = new Vector3(newRotationVertical, 0.0f, 0.0f);
    }
}
