using System;
using System.Collections.Generic;
using UnityEngine;

// Runtime progression tree for one spell. Attach this component to the same
// GameObject as its SpellSystem and author the branches in the Inspector.
public class SpellSkillTree : MonoBehaviour
{
    [Serializable]
    public class SkillNode
    {
        [Tooltip("Stable identifier used to record this upgrade (for example: ice_damage_1).")]
        public string id;
        public string displayName;
        [Min(1)] public int requiredPlayerLevel = 1;
        public float damageBonus;
        public float speedBonus;
        public float cooldownReduction;
        public float pierceBonus;

        [SerializeReference]
        public List<SkillNode> children = new List<SkillNode>();
    }

    [SerializeReference]
    private List<SkillNode> roots = new List<SkillNode>();

    private readonly HashSet<string> acquiredUpgradeIds = new HashSet<string>();

    // Returns unacquired upgrades whose parent path has been acquired and
    // whose required level has been reached.
    
    public List<SkillNode> GetAvailableUpgrades(int playerLevel)
    {
        List<SkillNode> available = new List<SkillNode>();
        CollectAvailable(roots, playerLevel, available);
        return available;
    }

    /// <summary>Acquires an upgrade only if it is currently available.</summary>
    public bool TryAcquireUpgrade(string upgradeId, int playerLevel)
    {
        if (string.IsNullOrWhiteSpace(upgradeId))
            return false;

        List<SkillNode> available = GetAvailableUpgrades(playerLevel);
        for (int i = 0; i < available.Count; i++)
        {
            if (available[i].id == upgradeId)
                return acquiredUpgradeIds.Add(upgradeId);
        }

        return false;
    }

    //Returns the sum of all acquired modifiers in this spell tree
    public void GetBonuses(out float damage, out float speed, out float cooldownReduction, out float pierce)
    {
        damage = 0f;
        speed = 0f;
        cooldownReduction = 0f;
        pierce = 0f;
        AccumulateBonuses(roots, ref damage, ref speed, ref cooldownReduction, ref pierce);
    }

    private void CollectAvailable(List<SkillNode> nodes, int playerLevel, List<SkillNode> available)
    {
        if (nodes == null)
            return; // Base case: there are no nodes to visit

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillNode node = nodes[i];
            if (node == null)
                continue;

            if (acquiredUpgradeIds.Contains(node.id))
            {
                // Recursive step: continue down the branch after the acquired node
                CollectAvailable(node.children, playerLevel, available);
            }
            else if (playerLevel >= node.requiredPlayerLevel)
            {
                available.Add(node);
            }
        }
    }

    private void AccumulateBonuses(List<SkillNode> nodes, ref float damage, ref float speed,
        ref float cooldownReduction, ref float pierce)
    {
        if (nodes == null)
            return; // Base case: this branch has no descendants.

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillNode node = nodes[i];
            if (node == null || !acquiredUpgradeIds.Contains(node.id))
                continue;

            damage += node.damageBonus;
            speed += node.speedBonus;
            cooldownReduction += node.cooldownReduction;
            pierce += node.pierceBonus;

            // Recursive step: include modifiers from every acquired descendant.
            AccumulateBonuses(node.children, ref damage, ref speed, ref cooldownReduction, ref pierce);
        }
    }
}
