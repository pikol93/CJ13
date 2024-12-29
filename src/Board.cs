using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Pikol93.CJ13;

public partial class Board : Node3D
{
    public Node3D myStack;
    public Node3D enemyStack;
    public List<Slot> handSlots = new();
    public List<List<Slot>> boardSlots = new();

    public Queue<Card> stackCards = new();
    public Queue<Card> enemyStackCards = new();
    public List<Card> handCards = new();
    public Dictionary<(int, int), Card> boardCards = new();

    public override void _Ready()
    {
        myStack = GetNode<Node3D>("MyStack");
        enemyStack = GetNode<Node3D>("EnemyStack");

        var handChildren = GetNode("MyHand").GetChildren();
        for (int i = 0; i < handChildren.Count; i++)
        {
            var child = handChildren[i];
            if (child is Slot slot)
            {
                handSlots.Add(slot);
                slot.handIndex = i;
            }
        }

        var rowParent = GetNode("Sprite3D");
        var rows = rowParent.GetChildren();
        for (int y = 0; y < rows.Count; y++)
        {
            var row = rows[y];
            var slots = row.GetChildren();
            var list = new List<Slot>();
            for (int x = 0; x < slots.Count; x++)
            {
                if (slots[x] is Slot slot)
                {
                    slot.row = y;
                    slot.column = x;
                    list.Add(slot);
                }
            }

            boardSlots.Add(list);
        }

        ClearBoard();
        DrawCards();
    }

    public void OnBoardSlotSelected(int row, int column)
    {
        GD.Print($"Slot selected: {row} {column}");
    }

    public void OnHandSlotSelected(int handIndex)
    {
        GD.Print($"Hand slot selected: {handIndex}");
    }

    public void ClearBoard()
    {
        stackCards = new Queue<Card>(GameManager.MyDeck);
        enemyStackCards = new Queue<Card>(Deck.GenerateDeck(3));
        foreach (var card in enemyStackCards)
        {
            card.IsEnemy = true;
        }

        foreach (var card in stackCards.Concat(enemyStackCards))
        {
            if (card.IsInsideTree())
            {
                card.Reparent(this);
            }
            else
            {
                AddChild(card);
            }
        }
    }

    public void DrawCards()
    {
        var amountToDraw = handSlots.Count - handCards.Count;
        for (var i = 0; i < amountToDraw; i++)
        {
            var success = stackCards.TryDequeue(out var card);
            if (!success)
            {
                GD.Print("Could not draw card from stack.");
                break;
            }

            handCards.Add(card);
        }

        Card c = stackCards.Dequeue();
        boardCards.Add((1, 1), c);

        Card d = enemyStackCards.Dequeue();
        boardCards.Add((3, 3), d);

        UpdateCardPositions();
    }

    public void UpdateCardPositions()
    {
        foreach (var card in stackCards)
        {
            card.Target = myStack;
            card.Hidden = true;
        }

        foreach (var card in enemyStackCards)
        {
            card.Target = enemyStack;
            card.Hidden = true;
        }

        for (var i = 0; i < handCards.Count; i++)
        {
            var card = handCards[i];
            card.Target = handSlots[i];
            card.Hidden = false;
        }

        foreach (var ((row, column), card) in boardCards)
        {
            card.Target = boardSlots[row][column];
            card.Hidden = false;
        }
    }

	private IEnumerable<Card> IterCards()
	{
		return stackCards
			.Concat(enemyStackCards)
			.Concat(handCards)
			.Concat(boardCards.Select((a, b) => a.Value));
	}
}
