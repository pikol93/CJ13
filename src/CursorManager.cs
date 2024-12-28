using Godot;

namespace Pikol93.CJ13;

public partial class CursorManager : Node
{
    private static Input.CursorShape current = Input.CursorShape.Arrow;

    public static void SetArrow()
    {
        ChangeCursor(Input.CursorShape.Arrow);
    }

    public static void SetHand()
    {
        ChangeCursor(Input.CursorShape.PointingHand);
    }

    public static void SetDrag()
    {
        ChangeCursor(Input.CursorShape.Drag);
    }

    public static void ChangeCursor(Input.CursorShape shape)
    {
        if (current == shape)
        {
            return;
        }
        current = shape;
        Input.SetDefaultCursorShape(shape);
    }
}
