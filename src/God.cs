using Godot;

namespace Pikol93.CJ13;

public partial class God : Node3D
{
    [Export] public Vector3 expectedLookTarget = Vector3.Zero;

    private Node3D mask;

    public override void _Ready()
    {
        mask = GetNode<Node3D>("god_mask_5");
    }

    public override void _Process(double delta)
    {
        var lookTarget = expectedLookTarget.IsZeroApprox() ? GetPlayerPosition() : expectedLookTarget;
        var difference = lookTarget - mask.GlobalPosition;
        var distance = new Vector2(difference.X, difference.Z).Length();

        var normalizedOnHorizontalPlane = new Vector2(difference.Z, difference.X).Angle();
        var normalizedOnVerticalPlane = new Vector2(distance, difference.Y).Angle();

        var newRotationHorizontal = Mathf.LerpAngle(Rotation.Y, normalizedOnHorizontalPlane, 0.1f);
        var newRotationVertical = Mathf.LerpAngle(mask.Rotation.X, -normalizedOnVerticalPlane, 0.1f);

        Rotation = new Vector3(0.0f, newRotationHorizontal, 0.0f);
        mask.Rotation = new Vector3(newRotationVertical, 0.0f, 0.0f);
    }

    private static Vector3 GetPlayerPosition()
    {
        var player = Player.Instance;
        if (player == null || !player.IsInsideTree())
        {
            return Vector3.Zero;
        }

        return player.GetCameraPosition();
    }
}
