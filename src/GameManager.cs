using System;
using Godot;

namespace Pikol93.CJ13;

public partial class GameManager : Node
{
    private static readonly TimeSpan DefaultPlayerInteractionBlockage = TimeSpan.FromSeconds(3);

    public static DateTime? PlayerInteractionBlockTime { get; set; }
    public static TimeSpan? PlayerInteractionBlockageOverride { get; set; }

    public static TimeSpan PlayerInteractionBlockage
    {
        get => PlayerInteractionBlockageOverride ?? DefaultPlayerInteractionBlockage;
    }

    public static bool CanPlayerInteract
    {
        get => PlayerInteractionBlockage == null
            || PlayerInteractionBlockTime?.Add(PlayerInteractionBlockage) > DateTime.Now;
    }
}
