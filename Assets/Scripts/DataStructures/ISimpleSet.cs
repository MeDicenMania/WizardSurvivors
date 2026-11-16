public interface ISimpleSet<T>
{
    int Count { get; }
    bool IsEmpty { get; }

    bool Add(T item);
    bool Remove(T item);
    bool Contains(T item);
    void Clear();
    T[] ToArray();

    ISimpleSet<T> UnionWith(ISimpleSet<T> other);
    ISimpleSet<T> IntersectWith(ISimpleSet<T> other);
    ISimpleSet<T> DifferenceWith(ISimpleSet<T> other);
}
