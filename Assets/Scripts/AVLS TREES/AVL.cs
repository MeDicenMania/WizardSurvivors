using System;

public class TreeNode<T> where T : IComparable<T>
{
    public T Value;
    public TreeNode<T> Left;
    public TreeNode<T> Right;
    public int Height = 1;

    public TreeNode(T value)
    {
        Value = value;
    }
}

public class BST<T> where T : IComparable<T>
{
    protected TreeNode<T> root;

    public virtual void Insert(T value)
    {
        root = InsertRecursive(root, value);
    }

    protected virtual TreeNode<T> InsertRecursive(
        TreeNode<T> current, T value)
    {
        if (current == null)
            return new TreeNode<T>(value);

        int comparison = value.CompareTo(current.Value);

        if (comparison < 0)
            current.Left = InsertRecursive(current.Left, value);
        else if (comparison > 0)
            current.Right = InsertRecursive(current.Right, value);

        // No inserta duplicados.
        return current;
    }

    public bool Contains(T value)
    {
        TreeNode<T> current = root;

        while (current != null)
        {
            int comparison = value.CompareTo(current.Value);

            if (comparison == 0)
                return true;

            current = comparison < 0
                ? current.Left
                : current.Right;
        }

        return false;
    }

    public virtual void Remove(T value)
    {
        root = RemoveRecursive(root, value);
    }

    protected virtual TreeNode<T> RemoveRecursive(
        TreeNode<T> current, T value)
    {
        if (current == null)
            return null;

        int comparison = value.CompareTo(current.Value);

        if (comparison < 0)
        {
            current.Left = RemoveRecursive(current.Left, value);
        }
        else if (comparison > 0)
        {
            current.Right = RemoveRecursive(current.Right, value);
        }
        else
        {
            if (current.Left == null)
                return current.Right;

            if (current.Right == null)
                return current.Left;

            TreeNode<T> successor = current.Right;

            while (successor.Left != null)
                successor = successor.Left;

            current.Value = successor.Value;

            current.Right = RemoveRecursive(
                current.Right, successor.Value);
        }

        return current;
    }
}

public class AVL<T> : BST<T> where T : IComparable<T>
{
    private int Height(TreeNode<T> node)
    {
        return node == null ? 0 : node.Height;
    }

    private int BalanceFactor(TreeNode<T> node)
    {
        if (node == null)
            return 0;

        return Height(node.Left) - Height(node.Right);
    }

    private void UpdateHeight(TreeNode<T> node)
    {
        node.Height = 1 + Math.Max(
            Height(node.Left),
            Height(node.Right));
    }

    // Rotación simple a la derecha.
    private TreeNode<T> RotateRight(TreeNode<T> y)
    {
        TreeNode<T> x = y.Left;
        TreeNode<T> subtree = x.Right;

        x.Right = y;
        y.Left = subtree;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    // Rotación simple a la izquierda.
    private TreeNode<T> RotateLeft(TreeNode<T> x)
    {
        TreeNode<T> y = x.Right;
        TreeNode<T> subtree = y.Left;

        y.Left = x;
        x.Right = subtree;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
    }

    private TreeNode<T> Balance(TreeNode<T> node)
    {
        if (node == null)
            return null;

        UpdateHeight(node);

        int balance = BalanceFactor(node);

        // Caso izquierda-izquierda o izquierda-derecha.
        if (balance > 1)
        {
            if (BalanceFactor(node.Left) < 0)
                node.Left = RotateLeft(node.Left);

            return RotateRight(node);
        }

        // Caso derecha-derecha o derecha-izquierda.
        if (balance < -1)
        {
            if (BalanceFactor(node.Right) > 0)
                node.Right = RotateRight(node.Right);

            return RotateLeft(node);
        }

        return node;
    }

    public override void Insert(T value)
    {
        root = InsertAVL(root, value);
    }

    private TreeNode<T> InsertAVL(
        TreeNode<T> current, T value)
    {
        if (current == null)
            return new TreeNode<T>(value);

        int comparison = value.CompareTo(current.Value);

        if (comparison < 0)
            current.Left = InsertAVL(current.Left, value);
        else if (comparison > 0)
            current.Right = InsertAVL(current.Right, value);
        else
            return current; // No admite duplicados.

        return Balance(current);
    }

    public override void Remove(T value)
    {
        root = RemoveAVL(root, value);
    }

    private TreeNode<T> RemoveAVL(
        TreeNode<T> current, T value)
    {
        if (current == null)
            return null;

        int comparison = value.CompareTo(current.Value);

        if (comparison < 0)
        {
            current.Left = RemoveAVL(current.Left, value);
        }
        else if (comparison > 0)
        {
            current.Right = RemoveAVL(current.Right, value);
        }
        else
        {
            // Caso sin hijo izquierdo.
            if (current.Left == null)
                return current.Right;

            // Caso sin hijo derecho.
            if (current.Right == null)
                return current.Left;

            // Caso con dos hijos: sucesor mínimo derecho.
            TreeNode<T> successor = current.Right;

            while (successor.Left != null)
                successor = successor.Left;

            current.Value = successor.Value;

            current.Right = RemoveAVL(
                current.Right, successor.Value);
        }

        return Balance(current);
    }
}