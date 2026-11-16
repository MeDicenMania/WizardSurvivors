using UnityEngine;

public interface ISimplePriorityQueue<T>
{
    public int Count { get; }
    public bool IsEmpty { get; }

    public void Enqueue(T item, int priority);
    public T Dequeue();
    public T Peek();
    public void Clear();
    public T[] ToArray();

    public int GetHighestPriority();
}
