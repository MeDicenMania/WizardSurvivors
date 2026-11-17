using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ExperienceCurve", menuName = "ScriptableObjects/Experience Curve AVL")]
public class ExperienceCurveAVL : ScriptableObject
{
    [Serializable]
    public class Entry : IComparable<Entry>
    {
        [Min(1)] public int level = 1;
        [Min(1)] public int experienceRequired = 5;

        public int CompareTo(Entry other)
        {
            return level.CompareTo(other.level);
        }
    }

    [SerializeField] private Entry[] entries;

    private AVL<Entry> experienceTree = new AVL<Entry>();

    private bool built;

    public int GetRequiredExperience(int level, int fallback)
    {
        EnsureBuilt();

        Entry search = new Entry();
        search.level = level;

        Entry result = experienceTree.Find(search);

        if (result != null)
            return result.experienceRequired;

        // Si no encuentra el nivel exacto, busca el anterior.
        Entry previous = experienceTree.FindPrevious(search);

        if (previous != null)
            return previous.experienceRequired;

        return fallback;
    }


   

    private void EnsureBuilt()
    {
        if (built)
            return;

        experienceTree = new AVL<Entry>();

        if (entries != null)
        {
            foreach (Entry entry in entries)
            {
                if (entry != null && entry.level > 0)
                    experienceTree.Insert(entry);
            }
        }

        built = true;
    }

    private void OnValidate()
    {
        built = false;
    }
}