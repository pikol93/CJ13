using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public partial class Ui : Control
{
    private readonly Queue<(string, double?, double?)> speechQueue = new();

    private Label speakLabel;
    private Label descriptionLabel;

    private string currentSpeech = null;
    private string currentItemName = "";
    private string currentItemDescription = "";
    private double timeSinceCurrentSpeech = 0.0;
    private double expectedSpeechTime = 0.01;
    private double expectedSpeechTimeWithLinger = 0.01;

    public static Ui Instance { get; private set; }

    [Export] public double TextSpeed { get; set; } = 8.0;
    [Export] public double TextEndLingerTime { get; set; } = 1.5;

    public override void _Ready()
    {
        Instance = this;
        speakLabel = GetNode<Label>("VBoxContainer/SpeakLabel");
        descriptionLabel = GetNode<Label>("VBoxContainer/DescriptionLabel");
    }

    public override void _Process(double delta)
    {
        timeSinceCurrentSpeech += delta;
        if (timeSinceCurrentSpeech > expectedSpeechTimeWithLinger)
        {
            if (speechQueue.Count > 0)
            {
                var tuple = speechQueue.Dequeue();
                currentSpeech = tuple.Item1;
                var textSpeed = tuple.Item2 ?? TextSpeed;
                var textEndLingerTime = tuple.Item3 ?? TextEndLingerTime;
                timeSinceCurrentSpeech = 0.0;
                expectedSpeechTime = currentSpeech.Length / textSpeed;
                expectedSpeechTimeWithLinger = expectedSpeechTime + textEndLingerTime;
            }
            else
            {
                currentSpeech = null;
            }
        }

        if (currentSpeech != null)
        {
            speakLabel.Uppercase = false;
            speakLabel.Text = currentSpeech;
            speakLabel.VisibleRatio = (float)(timeSinceCurrentSpeech / expectedSpeechTime);
            descriptionLabel.Text = "";
        }
        else
        {
            speakLabel.Uppercase = true;
            speakLabel.Text = currentItemName;
            speakLabel.VisibleCharacters = -1;
            descriptionLabel.Text = currentItemDescription;
        }
    }

    public void Speak(string value, double? textSpeed = null, double? textEndLingerTime = null)
    {
        speechQueue.Enqueue((value, textSpeed, textEndLingerTime));
    }

    public void ClearSpeach()
    {
        currentSpeech = null;
        speechQueue.Clear();
    }
}
