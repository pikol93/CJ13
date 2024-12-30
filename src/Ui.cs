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
    private int lastCharactersVisible = 0;
    private double timeSinceItem = 0.0;

    public static Ui Instance { get; private set; }

    [Export] public double TextSpeed { get; set; } = 8.0;
    [Export] public double TextEndLingerTime { get; set; } = 1.5;
    [Export] public int SoundPeriod { get; set; } = 2;

    public override void _Ready()
    {
        Instance = this;
        speakLabel = GetNode<Label>("VBoxContainer/SpeakLabel");
        descriptionLabel = GetNode<Label>("VBoxContainer/DescriptionLabel");
    }

    public override void _Process(double delta)
    {
        timeSinceItem -= delta;
        if (timeSinceItem < 0.0)
        {
            currentItemName = "";
            currentItemDescription = "";
        }

        timeSinceCurrentSpeech += delta;
        if (timeSinceCurrentSpeech > expectedSpeechTimeWithLinger)
        {
            lastCharactersVisible = 0;

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
            if (speakLabel.VisibleCharacters - lastCharactersVisible >= SoundPeriod)
            {
                God.Instance.PlaySpeechSound();
                lastCharactersVisible = speakLabel.VisibleCharacters;
            }
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

    public void ClearSpeech()
    {
        currentSpeech = null;
        speechQueue.Clear();
    }

    public void Item(string name, string description)
    {
        currentItemName = name;
        currentItemDescription = description;
        timeSinceItem = 0.1;
    }
}
