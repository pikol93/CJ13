using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public static class GodotExtensions
{
    public static T GetAncestor<T>(this Node node)
    where T : class
    {
        var current = node.GetParent();
        while (current != null)
        {
            if (current is T t)
            {
                return t;
            }

            current = current.GetParent();
        }

        return null;
    }

    public static TV Get<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
    {
        TV value;
        return dict.TryGetValue(key, out value) ? value : defaultValue;
    }
}
