using System;
using UnityEngine;

/// <summary>Balanced AVL index from player level to cumulative XP required for that level.</summary>
[CreateAssetMenu(fileName = "ExperienceCurve", menuName = "ScriptableObjects/Experience Curve AVL")]
public class ExperienceCurveAVL : ScriptableObject
{
    [Serializable] public class Entry { [Min(1)] public int level = 1; [Min(1)] public int experienceRequired = 5; }
    [SerializeField] private Entry[] entries;
    private Node root;
    private bool built;
    private class Node { public Entry value; public int height = 1; public Node left, right; public Node(Entry e) { value = e; } }

    public int GetRequiredExperience(int level, int fallback)
    {
        EnsureBuilt();
        Node n = root; Entry best = null;
        while (n != null) { if (level == n.value.level) return n.value.experienceRequired; if (level < n.value.level) n = n.left; else { best = n.value; n = n.right; } }
        return best != null ? best.experienceRequired : fallback;
    }
    private void EnsureBuilt() { if (built) return; root = null; if (entries != null) foreach (Entry e in entries) if (e != null && e.level > 0) root = Insert(root, e); built = true; }
    private static int H(Node n) => n == null ? 0 : n.height;
    private static void Update(Node n) { n.height = 1 + Math.Max(H(n.left), H(n.right)); }
    private static int Balance(Node n) => H(n.left) - H(n.right);
    private static Node Right(Node y) { Node x = y.left, t = x.right; x.right = y; y.left = t; Update(y); Update(x); return x; }
    private static Node Left(Node x) { Node y = x.right, t = y.left; y.left = x; x.right = t; Update(x); Update(y); return y; }
    private static Node Insert(Node n, Entry e)
    {
        if (n == null) return new Node(e);
        if (e.level < n.value.level) n.left = Insert(n.left, e); else if (e.level > n.value.level) n.right = Insert(n.right, e); else { n.value = e; return n; }
        Update(n); int b = Balance(n);
        if (b > 1) { if (e.level > n.left.value.level) n.left = Left(n.left); return Right(n); }
        if (b < -1) { if (e.level < n.right.value.level) n.right = Right(n.right); return Left(n); }
        return n;
    }
    private void OnValidate() { built = false; }
}
