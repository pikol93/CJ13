using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public class Deck
{
    private static readonly Texture2D DefaultTexture = readTexture("symbol0");
    private static readonly PackedScene CardScene = GD.Load<PackedScene>("res://scenes/card.tscn");
    private static readonly List<(int, int, int, int, Texture2D, List<Vector2I>)> Templates = new()
    {
        (5, 0, 2, 0, readTexture("symbol1"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down}),
        (3, 1, 1, 1, readTexture("symbol2"), new(){Vector2I.Left, Vector2I.Left * 2, Vector2I.Right, Vector2I.Right * 2, Vector2I.Down}),
        (6, 0, 2, 0, readTexture("symbol3"), new(){Vector2I.Left, Vector2I.Right}),
        (2, 0, 2, 0, readTexture("symbol4"), new(){Vector2I.Down, Vector2I.Down * 2}),
        (3, 0, 1, 2, readTexture("symbol5"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down}),
        (8, 0, 1, 0, readTexture("symbol6"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down}),
        (4, 1, 1, 0, readTexture("symbol7"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down}),
        (5, 1, 1, 1, readTexture("symbol8"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down, Vector2I.Up, Vector2I.Up * 2}),
        (3, 2, 2, 2, readTexture("symbol14"), new(){Vector2I.Left, Vector2I.Right, Vector2I.Down}),
    };

    private static readonly List<Card> CardTemplates = new();

    static Deck()
    {
        foreach (var (health, attackWest, attackVertical, attackEast, cardIcon, availableMoves) in Templates)
        {
            Card card = CardScene.Instantiate<Card>();
            card.Health = health;
            card.AttackVertical = attackVertical;
            card.AttackWest = attackWest;
            card.AttackEast = attackEast;
            card.CardIcon = cardIcon;
            card.AvailableMoves = availableMoves;
            CardTemplates.Add(card);
        }
    }

    public static List<Card> GenerateDeck(int cardCount)
    {
        var random = new Random();
        var result = new List<Card>();
        for (var i = 0; i < cardCount; i++)
        {
            var index = random.Next(CardTemplates.Count);
            var template = (Card)CardTemplates[index];
            var card = (Card)template.Duplicate();
            card.AvailableMoves = template.AvailableMoves;
            result.Add(card);
        }

        return result;
    }

    private static Texture2D readTexture(string id)
    {
        return GD.Load<Texture2D>($"res://textures/card_icons/{id}.png") ?? DefaultTexture;
    }
}