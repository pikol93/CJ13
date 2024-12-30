using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public partial class World : Node3D
{
    private static readonly Random Random = new();
    public static void EnablePlayerInteraction()
    {
        GameManager.PlayerInteractionBlockTime = null;
    }

    public static void DisablePlayerInteraction(double seconds)
    {
        GameManager.PlayerInteractionBlockTime = DateTime.Now;
        GameManager.PlayerInteractionBlockageOverride = TimeSpan.FromSeconds(seconds);
    }

    public void ShowReward()
    {
        GD.Print("Showing reward.");
        var rewards = new List<string>
        {
            "reward_items",
            "reward_items",
            "board_game",
        };

        var index = Random.Next(rewards.Count);
        var animationName = rewards[index];
        GameManager.GameAnimationTreeStateMachine.Travel(animationName);
    }

    public void Quit()
    {
        GetTree().Quit();
    }
}
