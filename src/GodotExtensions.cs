using System;
using System.Collections.Generic;
using Godot;

namespace Pikol93.CJ13;

public static class GodotExtensions
{
    private static readonly Random Random = new();  

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

    public static void Shuffle<T>(this IList<T> list)
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
