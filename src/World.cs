using System;
using Godot;

namespace Pikol93.CJ13;

public partial class World : Node3D
{
    public static void EnablePlayerInteraction()
    {
        GameManager.PlayerInteractionBlockTime = null;
    }

    public static void DisablePlayerInteraction(double seconds)
    {
        GameManager.PlayerInteractionBlockTime = DateTime.Now;
        GameManager.PlayerInteractionBlockageOverride = TimeSpan.FromSeconds(seconds);
    }

    public void Quit()
    {
        GetTree().Quit();
    }
}
