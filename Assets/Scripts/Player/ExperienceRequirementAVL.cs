using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceRequirements", menuName = "Progression/Experience Requirements AVL")]
public class ExperienceRequirementAVL : ScriptableObject
{
    [Serializable]
    public class LevelRequirement : IComparable<LevelRequirement>
    {
        [Min(2)] public int level;
        [Min(1)] public int experienceRequired;

        public int CompareTo(LevelRequirement other)
        {
            return other == null ? 1 : level.CompareTo(other.level);
        }
    }

    [SerializeField] private List<LevelRequirement> requirements = new List<LevelRequirement>();

    [NonSerialized] private AVL<LevelRequirement> index;

    private void OnEnable() { Rebuild(); }

    public bool TryGetRequirement(int level, out int experienceRequired)
    {
        if (index == null) Rebuild();
        bool found = index.TryGetValue(new LevelRequirement { level = level }, out LevelRequirement requirement);
        experienceRequired = found ? requirement.experienceRequired : 0;
        return found;
    }

    private void OnValidate() { Rebuild(); }

    private void Rebuild()
    {
        index = new AVL<LevelRequirement>();
        if (requirements == null) return;
        for (int i = 0; i < requirements.Count; i++)
            if (requirements[i] != null && requirements[i].level >= 2 && requirements[i].experienceRequired > 0)
                index.Insert(requirements[i]);
    }
}
