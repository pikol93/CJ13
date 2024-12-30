using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Godot;

namespace Pikol93.CJ13;

public partial class Board : Node3D
{
    private static readonly Random random = new();

    public Node3D myStack;
    public Node3D enemyStack;
    public List<Slot> handSlots = new();
    public List<List<Slot>> boardSlots = new();

    public Queue<Card> stackCards = new();
    public Queue<Card> enemyStackCards = new();
    public List<Card> handCards = new();
    public Dictionary<(int, int), Card> boardCards = new();

    private Card selectedCard;
    private bool lastTurnPlayer = true;
    private double moveAnimationTimer;
    private double attackAnimationTimer;
    private double postAttackAnimationTimer;
    private double postPlayerTurnTimer;
    private double postEnemyTurnTimer;

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
        BeginPlayerTurn();
    }

    public override void _Process(double delta)
    {
        if (moveAnimationTimer > 0.0)
        {
            moveAnimationTimer -= delta;
            if (moveAnimationTimer <= 0.0)
            {
                attackAnimationTimer = 0.1;
            }
            return;
        }

        if (attackAnimationTimer > 0.0)
        {
            attackAnimationTimer -= delta;
            if (attackAnimationTimer <= 0.0)
            {
                PerformAttacks();
                GD.Print($"Count: {boardCards.Count}");
                postAttackAnimationTimer = 0.1;
            }
            return;
        }

        if (postAttackAnimationTimer > 0.0)
        {
            postAttackAnimationTimer -= delta;
            return;
        }

        if (postPlayerTurnTimer > 0.0)
        {
            postPlayerTurnTimer -= delta;
            if (postPlayerTurnTimer <= 0)
            {
                PerformEnemyTurn();
            }
        }
        if (postEnemyTurnTimer > 0.0)
        {
            postEnemyTurnTimer -= delta;
            if (postEnemyTurnTimer <= 0)
            {
                BeginPlayerTurn();
            }
        }
    }

    public void OnBoardSlotSelected(int row, int column)
    {
        GD.Print($"Slot selected: {row} {column}");
        var card = boardCards.Get((row, column));
        if (card != null)
        {
            SetSelectedCard(card, false, row, column);
        }
        else
        {
            if (selectedCard != null)
            {
                MoveCard(selectedCard, row, column);
                EndMyTurn();
            }
        }
    }

    public void OnHandSlotSelected(int handIndex)
    {
        GD.Print($"Hand slot selected: {handIndex}");
        var card = handCards[handIndex];
        if (card != null)
        {
            SetSelectedCard(card, true, -1, -1);
        }
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

    public void BeginPlayerTurn()
    {
        lastTurnPlayer = true;
        MarkOnlySlotsWithCardsAsSelectable();
    }

    public void EndMyTurn()
    {
        PostTurn();
        postPlayerTurnTimer = 0.2f;
    }

    public void PostTurn()
    {
        ClearSelectedCard();
        MarkAllSlotsAsUnpickable();
        moveAnimationTimer = 0.2;
        UpdateCardPositions();
    }

    private void MoveCard(Card card, int row, int column)
    {
        if (card == null)
        {
            throw new ArgumentException(null, nameof(card));
        }

        handCards.Remove(card);
        foreach (var item in boardCards.Where(kvp => kvp.Value == card).ToList())
        {
            boardCards.Remove(item.Key);
        }

        boardCards.Add((row, column), card);
    }

    private void MarkAllSlotsAsUnpickable()
    {
        foreach (var slot in IterSlots())
        {
            slot.MakeInvisibleUnpickable();
        }
    }

    private void MarkOnlySlotsWithCardsAsSelectable()
    {
        MarkAllSlotsAsUnpickable();

        foreach (var card in handCards.Concat(boardCards.Select((a, b) => a.Value)).Where((card) => !card.IsEnemy))
        {
            var obj = card.Target;
            if (obj is not Slot slot)
            {
                GD.PrintErr($"This should not happen. {card.Name} {card.Target}");
                continue;
            }

            slot.MakeAvailable();
        }
    }

    private void MarkPossibleMovesAndCardsAsSelectable(List<Vector2I> list)
    {
        MarkOnlySlotsWithCardsAsSelectable();

        foreach (var item in list)
        {
            var row = item.Y;
            var column = item.X;
            var slot = boardSlots[row][column];
            slot.MakeAvailable();
        }
    }

    private void SetSelectedCard(Card card, bool isInHand, int row, int column)
    {
        ClearSelectedCard();

        selectedCard = card;
        card.Select();

        List<Vector2I> possibleSlots;
        if (isInHand)
        {
            possibleSlots = new()
            {
                new Vector2I(0, 0),
                new Vector2I(1, 0),
                new Vector2I(2, 0),
                new Vector2I(3, 0),
                new Vector2I(4, 0),
            };
        }
        else
        {
            possibleSlots = card
                .AvailableMoves
                .Select((item) => new Vector2I(item.X + column, item.Y + row))
                .ToList();
        }

        DiscardOutOfBoundsMoves(possibleSlots);
        DiscardUnavailableSlots(possibleSlots);
        MarkPossibleMovesAndCardsAsSelectable(possibleSlots);
    }

    private void ClearSelectedCard()
    {
        selectedCard?.Unselect();
        selectedCard = null;
    }

    private static void DiscardOutOfBoundsEnemyMoves(List<Vector2I> list)
    {
        var discardCount = list.RemoveAll((item) => item.X < 0 || item.X > 4 || item.Y < 1 || item.Y > 3);
        GD.Print($"Discarded {discardCount} enemy out of bounds moves. Remaining {list.Count} slots");
    }

    private static void DiscardOutOfBoundsMoves(List<Vector2I> list)
    {
        var discardCount = list.RemoveAll((item) => item.X < 0 || item.X > 4 || item.Y < 0 || item.Y > 2);
        GD.Print($"Discarded {discardCount} player out of bounds moves. Remaining {list.Count} slots");
    }

    private void DiscardUnavailableSlots(List<Vector2I> list)
    {
        var discardCount = list.RemoveAll((item) => boardCards.ContainsKey((item.Y, item.X)));
        GD.Print($"Discarded {discardCount} unavailable slots. Remaining {list.Count} slots");
    }

    private IEnumerable<Slot> IterSlots()
    {
        return handSlots.Concat(boardSlots.SelectMany(list => list));
    }

    private IEnumerable<Card> IterCards()
    {
        return stackCards
            .Concat(enemyStackCards)
            .Concat(handCards)
            .Concat(boardCards.Select((a, b) => a.Value));
    }

    private void PerformAttacks()
    {
        List<(Card, Card, int)> attacks = new();
        foreach (var ((row, column), card) in boardCards.Where(card => card.Value.IsEnemy != lastTurnPlayer))
        {
            if (card.AttackVertical > 0)
            {
                var newRow = row + (card.IsEnemy ? -1 : 1);
                var receiver = boardCards.Get((newRow, column));
                if (receiver != null && card.IsEnemy != receiver.IsEnemy)
                {
                    attacks.Add((card, receiver, card.AttackVertical));
                }
            }

            if (card.AttackWest > 0)
            {
                var receiver = boardCards.Get((row, column - 1));
                if (receiver != null && card.IsEnemy != receiver.IsEnemy)
                {
                    attacks.Add((card, receiver, card.AttackVertical));
                }
            }

            if (card.AttackEast > 0)
            {
                var receiver = boardCards.Get((row, column + 1));
                if (receiver != null && card.IsEnemy != receiver.IsEnemy)
                {
                    attacks.Add((card, receiver, card.AttackVertical));
                }
            }
        }

        foreach (var (_, rx, attack) in attacks)
        {
            rx.Health -= attack;
            if (rx.Health <= 0)
            {
                rx.QueueFree();
                GameManager.ValidateDeck();
                ValidateDeck();
            }
        }
    }

    private void ValidateDeck()
    {
        var keys = new List<(int, int)>(boardCards.Keys);
        foreach (var key in keys)
        {
            var card = boardCards[key];
            if (!IsInstanceValid(card) || card.IsQueuedForDeletion())
            {
                GD.Print($"Invalid card instance. Removing key {key}");
                boardCards.Remove(key);
            }
        }
    }

    private void PerformEnemyTurn()
    {
        lastTurnPlayer = false;
        List<Action> possibleActions = new();
        var enemyDrawCardOnBoard = EnemyDrawCardOnBoard();
        if (enemyDrawCardOnBoard != null)
        {
            possibleActions.Add(enemyDrawCardOnBoard);
        }

        var actionMoveCard = EnemyMoveCard();
        if (actionMoveCard != null)
        {
            possibleActions.Add(actionMoveCard);
        }

        if (possibleActions.Count == 0)
        {
            EnemyForfeit();
            return;
        }

        var index = random.Next(possibleActions.Count);
        possibleActions[index].Invoke();

        PostTurn();
        postEnemyTurnTimer = 0.2;
    }

    private Action EnemyDrawCardOnBoard()
    {
        if (enemyStackCards.Count == 0)
        {
            return null;
        }

        List<int> possibleColumns = new() { 0, 1, 2, 3, 4 };
        foreach (var ((row, column), _) in boardCards)
        {
            if (row != 3)
            {
                continue;
            }

            possibleColumns.Remove(column);
        }

        if (possibleColumns.Count == 0)
        {
            return null;
        }

        var index = random.Next(possibleColumns.Count);
        var newColumn = possibleColumns[index];
        return () =>
        {
            var card = enemyStackCards.Dequeue();
            boardCards.Add((3, newColumn), card);
        };
    }

    private Action EnemyMoveCard()
    {
        var a = boardCards
        .Where((item) => item.Value.IsEnemy)
        .SelectMany((item) =>
            {
                var (row, column) = item.Key;
                var card = item.Value;
                var movesList = card.AvailableMoves.Select((move) => new Vector2I(column, row) - move).ToList();
                DiscardOutOfBoundsEnemyMoves(movesList);
                DiscardUnavailableSlots(movesList);

                var result = movesList.Select((item) => (card, item)).ToList();
                return result;
            })
        .ToList();

        GD.Print($"{a.Count} moves available");

        if (a.Count == 0)
        {
            return null;
        }

        var index = random.Next(a.Count);
        var (card, move) = a[index];

        return () =>
        {
            MoveCard(card, move.Y, move.X);
        };
    }

    private static void EnemyForfeit()
    {
        GD.Print("Enemy forfeit!");
    }
}
