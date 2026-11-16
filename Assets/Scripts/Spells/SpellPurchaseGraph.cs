using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Directed, non-negative weighted spell graph. Edge weights are XP purchase costs.</summary>
[CreateAssetMenu(fileName = "SpellPurchaseGraph", menuName = "ScriptableObjects/Spell Purchase Graph")]
public class SpellPurchaseGraph : ScriptableObject
{
    [Serializable] public class Edge { public SpellsScriptableObjects destination; [Min(0)] public int cost = 1; }
    [Serializable] public class Vertex { public SpellsScriptableObjects spell; public List<Edge> outgoing = new List<Edge>(); }
    [Serializable] public class PurchasePath { public List<SpellsScriptableObjects> spells = new List<SpellsScriptableObjects>(); public int totalCost; }
    public List<Vertex> vertices = new List<Vertex>();

    public bool TryGetCheapestPath(SpellsScriptableObjects start, SpellsScriptableObjects target, out PurchasePath path)
    {
        path = null;
        if (start == null || target == null) return false;
        Dictionary<SpellsScriptableObjects, int> distances = new Dictionary<SpellsScriptableObjects, int>();
        Dictionary<SpellsScriptableObjects, SpellsScriptableObjects> previous = new Dictionary<SpellsScriptableObjects, SpellsScriptableObjects>();
        HashSet<SpellsScriptableObjects> visited = new HashSet<SpellsScriptableObjects>();
        foreach (Vertex v in vertices) if (v != null && v.spell != null) distances[v.spell] = int.MaxValue;
        if (!distances.ContainsKey(start) || !distances.ContainsKey(target)) return false;
        distances[start] = 0;

        while (true)
        {
            SpellsScriptableObjects current = null;
            int best = int.MaxValue;
            foreach (KeyValuePair<SpellsScriptableObjects, int> pair in distances)
                if (!visited.Contains(pair.Key) && pair.Value < best) { current = pair.Key; best = pair.Value; }
            if (current == null) break;
            if (current == target) break;
            visited.Add(current);
            Vertex vertex = FindVertex(current);
            if (vertex == null || vertex.outgoing == null) continue;
            foreach (Edge edge in vertex.outgoing)
            {
                if (edge == null || edge.destination == null || edge.cost < 0 || !distances.ContainsKey(edge.destination) || visited.Contains(edge.destination)) continue;
                long candidate = (long)best + edge.cost;
                if (candidate < distances[edge.destination]) { distances[edge.destination] = (int)candidate; previous[edge.destination] = current; }
            }
        }
        if (distances[target] == int.MaxValue) return false;
        List<SpellsScriptableObjects> reversed = new List<SpellsScriptableObjects>();
        SpellsScriptableObjects step = target;
        reversed.Add(step);
        while (step != start) { if (!previous.TryGetValue(step, out step)) return false; reversed.Add(step); }
        reversed.Reverse();
        path = new PurchasePath { spells = reversed, totalCost = distances[target] };
        return true;
    }

    private Vertex FindVertex(SpellsScriptableObjects spell)
    {
        foreach (Vertex vertex in vertices) if (vertex != null && vertex.spell == spell) return vertex;
        return null;
    }
}
