using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Directed spell shop graph. Dijkstra minimizes spell prices along unlock routes.</summary>
public class SpellPurchaseGraph : MonoBehaviour
{
    [Serializable]
    public class Edge
    {
        public string destinationId;
    }

    [Serializable]
    public class SpellNode
    {
        public string id;
        public GameObject spellPrefab;
        [Min(0)] public int purchaseCost;
        public List<Edge> outgoing = new List<Edge>();
    }

    [SerializeField] private List<SpellNode> spells = new List<SpellNode>();
    [Tooltip("IDs of the wizard's starting spells, as configured in its character data.")]
    [SerializeField] private List<string> initialOwnedSpellIds = new List<string>();
    public IReadOnlyList<SpellNode> Spells => spells;

    public bool TryPurchase(string spellId, PlayerStats player, out List<string> purchasedRoute)
    {
        purchasedRoute = new List<string>();
        if (player == null || string.IsNullOrWhiteSpace(spellId) || IsOwned(spellId, player))
            return false;

        if (!TryGetCheapestRoute(spellId, player, out List<SpellNode> route, out int totalCost))
            return false;
        for (int i = 0; i < route.Count; i++)
            if (route[i].spellPrefab == null) return false;
        if (!player.TrySpendSpellCurrency(totalCost))
            return false;

        for (int i = 0; i < route.Count; i++)
        {
            SpellNode node = route[i];
            if (node.spellPrefab != null && !IsOwned(node.id, player))
            {
                player.SpawnSpell(node.spellPrefab);
                player.RegisterSpellInInventory(node.id);
                purchasedRoute.Add(node.id);
            }
        }
        return purchasedRoute.Count > 0;
    }

    public bool TryGetCheapestRoute(string spellId, PlayerStats player,
        out List<SpellNode> route, out int totalCost)
    {
        route = new List<SpellNode>();
        totalCost = 0;
        if (player == null || string.IsNullOrWhiteSpace(spellId) || IsOwned(spellId, player))
            return false;
        Dictionary<string, SpellNode> byId = BuildIndex();
        if (!byId.TryGetValue(spellId, out SpellNode target) || target.spellPrefab == null)
            return false;
        return FindCheapestRoute(spellId, player, byId, out route, out totalCost);
    }

    private Dictionary<string, SpellNode> BuildIndex()
    {
        Dictionary<string, SpellNode> result = new Dictionary<string, SpellNode>();
        for (int i = 0; i < spells.Count; i++)
        {
            SpellNode node = spells[i];
            if (node != null && !string.IsNullOrWhiteSpace(node.id) && !result.ContainsKey(node.id))
                result.Add(node.id, node);
        }
        return result;
    }

    private bool FindCheapestRoute(string targetId, PlayerStats player,
        Dictionary<string, SpellNode> byId, out List<SpellNode> route, out int totalCost)
    {
        route = new List<SpellNode>();
        totalCost = 0;
        Dictionary<string, long> distance = new Dictionary<string, long>();
        Dictionary<string, string> previous = new Dictionary<string, string>();
        HashSet<string> visited = new HashSet<string>();

        foreach (SpellNode node in spells)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.id)) continue;
            distance[node.id] = long.MaxValue;
        }
        foreach (SpellNode node in spells)
        {
            if (node == null || !IsOwned(node.id, player)) continue;
            distance[node.id] = 0;
        }

        while (true)
        {
            string currentId = null;
            long bestDistance = long.MaxValue;
            foreach (KeyValuePair<string, long> pair in distance)
                if (!visited.Contains(pair.Key) && pair.Value < bestDistance)
                { currentId = pair.Key; bestDistance = pair.Value; }

            if (currentId == null) break;
            if (currentId == targetId) break;
            visited.Add(currentId);
            if (!byId.TryGetValue(currentId, out SpellNode current)) continue;

            if (current.outgoing == null) continue;
            for (int i = 0; i < current.outgoing.Count; i++)
            {
                Edge edge = current.outgoing[i];
                if (edge == null || !byId.TryGetValue(edge.destinationId, out SpellNode next)) continue;
                long candidate = bestDistance + (IsOwned(next.id, player) ? 0 : Math.Max(0, next.purchaseCost));
                if (candidate < distance[next.id])
                {
                    distance[next.id] = candidate;
                    previous[next.id] = currentId;
                }
            }
        }

        if (!distance.TryGetValue(targetId, out long targetDistance) || targetDistance == long.MaxValue || targetDistance > int.MaxValue)
            return false;

        List<string> pathIds = new List<string>();
        string pathCursor = targetId;
        pathIds.Add(pathCursor);
        while (previous.TryGetValue(pathCursor, out string parentId))
        {
            pathCursor = parentId;
            pathIds.Add(pathCursor);
        }
        pathIds.Reverse();

        for (int i = 0; i < pathIds.Count; i++)
        {
            if (byId.TryGetValue(pathIds[i], out SpellNode node) && !IsOwned(node.id, player))
                route.Add(node);
        }
        totalCost = (int)targetDistance;
        return route.Count > 0;
    }

    private bool IsOwned(string spellId, PlayerStats player)
    {
        return player.HasSpellInInventory(spellId) || initialOwnedSpellIds.Contains(spellId);
    }
}
