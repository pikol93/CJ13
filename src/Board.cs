using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public partial class Board : Node3D
{
    private List<List<Slot>> slots;

    public override void _Ready()
    {
        this.slots = new List<List<Slot>>();
        var rowParent = GetNode("Sprite3D");
        var rows = rowParent.GetChildren();
        for (int y = 0; y < rows.Count; y++)
        {
            var row = rows[y];
            var slots = row.GetChildren();
			var list = new List<Slot>();
            for (int x = 0; x < slots.Count; x++)
            {
                var slot = (Slot)slots[x];
                slot.row = y;
                slot.column = x;
                list.Add(slot);
            }

			this.slots.Add(list);
        }
    }

    public void OnSlotSelected(int row, int column)
    {
        GD.Print($"Slot selected: {row} {column}");
    }
}
