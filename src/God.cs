using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public partial class God : Node3D
{
    [Export] public Vector3 expectedLookTarget = Vector3.Zero;

    private Node3D mask;
    private AudioStreamPlayer3D speechPlayer;

    public static God Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        mask = GetNode<Node3D>("god_mask_5");
        speechPlayer = mask.GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
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

    public void LookAtPlayer()
    {
        expectedLookTarget = Vector3.Zero;
    }

    public void LookAtStuff()
    {
        expectedLookTarget = new Vector3(-5.0f, 1.0f, 0.0f);
    }

    public void ClearSpeech()
    {
        Ui.Instance.ClearSpeech();
    }

    public void Speak(string value)
    {
        Ui.Instance.Speak(value);
    }

    public void SpeakMod(string value, double textSpeed, double textEndLingerTime)
    {
        Ui.Instance.Speak(value, textSpeed, textEndLingerTime);
    }

    public void SpeakLoss()
    {
        string text = GameManager.GodLosses switch
        {
            1 => "Jedna przegrana o niczym nie świadczy.",
            2 => "Do trzech razy sztuka. Jeszcze raz.",
            3 => "To na pewno kwestia szczęścia.",
            _ => "Zagrajmy jeszcze raz.",
        };

        Speak(text);
    }

    public void PlaySpeechSound()
    {
        speechPlayer.Play();
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

    public Vector3 GetMaskPosition()
    {
        return mask.IsInsideTree() ? mask.GlobalPosition : Vector3.Zero;
    }
}
