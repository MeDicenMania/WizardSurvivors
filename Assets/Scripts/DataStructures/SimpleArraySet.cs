using System;

public class SimpleArraySet<T> : ISimpleSet<T>
{
    private T[] items;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public SimpleArraySet(int capacity = 10)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        items = new T[capacity];
    }

    public bool Add(T item)
    {
        if (Contains(item))
            return false;

        EnsureCapacity();
        items[count++] = item;
        return true;
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0)
            return false;

        count--;
        items[index] = items[count];
        items[count] = default;
        return true;
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void Clear()
    {
        Array.Clear(items, 0, count);
        count = 0;
    }

    public T[] ToArray()
    {
        T[] copy = new T[count];
        Array.Copy(items, copy, count);
        return copy;
    }

    public ISimpleSet<T> UnionWith(ISimpleSet<T> other)
    {
        ValidateOther(other);
        SimpleArraySet<T> result = new SimpleArraySet<T>(count + other.Count);
        foreach (T item in ToArray()) result.Add(item);
        foreach (T item in other.ToArray()) result.Add(item);
        return result;
    }

    public ISimpleSet<T> IntersectWith(ISimpleSet<T> other)
    {
        ValidateOther(other);
        SimpleArraySet<T> result = new SimpleArraySet<T>();
        foreach (T item in ToArray())
            if (other.Contains(item)) result.Add(item);
        return result;
    }

    public ISimpleSet<T> DifferenceWith(ISimpleSet<T> other)
    {
        ValidateOther(other);
        SimpleArraySet<T> result = new SimpleArraySet<T>();
        foreach (T item in ToArray())
            if (!other.Contains(item)) result.Add(item);
        return result;
    }

    private int IndexOf(T item)
    {
        for (int i = 0; i < count; i++)
            if (Equals(items[i], item)) return i;
        return -1;
    }

    private void EnsureCapacity()
    {
        if (count < items.Length) return;
        int newCapacity = items.Length == 0 ? 4 : items.Length * 2;
        Array.Resize(ref items, newCapacity);
    }

    private static void ValidateOther(ISimpleSet<T> other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
    }
}
