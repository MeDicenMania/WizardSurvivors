using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Recursively traversable spell progression tree. Each node references one spell asset.</summary>
[CreateAssetMenu(fileName = "SpellSkillTree", menuName = "ScriptableObjects/Spell Skill Tree")]
public class SpellSkillTree : ScriptableObject
{
    [Serializable]
    public class Node
    {
        public SpellsScriptableObjects spell;
        public List<Node> children = new List<Node>();
    }

    public List<Node> roots = new List<Node>();

    public bool Contains(SpellsScriptableObjects spell) => FindRecursive(roots, spell);

    public List<SpellsScriptableObjects> GetUnlockableSpells(HashSet<SpellsScriptableObjects> owned)
    {
        List<SpellsScriptableObjects> result = new List<SpellsScriptableObjects>();
        CollectUnlockableRecursive(roots, owned, result, true);
        return result;
    }

    private static bool FindRecursive(List<Node> nodes, SpellsScriptableObjects target)
    {
        if (nodes == null) return false;
        foreach (Node node in nodes)
        {
            if (node == null) continue;
            if (node.spell == target || FindRecursive(node.children, target)) return true;
        }
        return false;
    }

    private static void CollectUnlockableRecursive(List<Node> nodes, HashSet<SpellsScriptableObjects> owned, List<SpellsScriptableObjects> result, bool parentOwned)
    {
        if (nodes == null) return;
        foreach (Node node in nodes)
        {
            if (node == null) continue;
            bool available = parentOwned && node.spell != null && !owned.Contains(node.spell);
            if (available && !result.Contains(node.spell)) result.Add(node.spell);
            CollectUnlockableRecursive(node.children, owned, result, node.spell != null && owned.Contains(node.spell));
        }
    }
}
