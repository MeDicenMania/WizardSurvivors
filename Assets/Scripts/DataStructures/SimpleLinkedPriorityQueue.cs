using System;
using UnityEngine;

public class SimpleLinkedPriorityQueue<T> : ISimplePriorityQueue<T>
{
    private PriorityLinkedNode<T> first;
    private PriorityLinkedNode<T> last;
    private int count;

    public int Count => count;

    public bool IsEmpty => count == 0;

    public void Enqueue(T item, int priority)
    {
        PriorityLinkedNode<T> newNode =
            new PriorityLinkedNode<T>(item, priority);

        // Si la cola está vacía
        if (IsEmpty)
        {
            first = newNode;
            last = newNode;
            count++;
            return;
        }

        // Si tiene mayor prioridad que el primero
        if (priority < first.Priority)
        {
            newNode.Next = first;
            first = newNode;
            count++;
            return;
        }

        // Buscamos dónde insertar
        PriorityLinkedNode<T> current = first;

        while (current.Next != null &&
               current.Next.Priority <= priority)
        {
            current = current.Next;
        }

        // Insertamos después de current
        newNode.Next = current.Next;
        current.Next = newNode;

        // Si se insertó al final, actualizamos last
        if (newNode.Next == null)
        {
            last = newNode;
        }

        count++;
    }

    public T Dequeue()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "Priority Queue vacía.");
        }

        T item = first.Item;

        first = first.Next;

        count--;

        if (count == 0)
        {
            last = null;
        }

        return item;
    }

    public T Peek()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "Priority Queue vacía.");
        }

        return first.Item;
    }

    public int GetHighestPriority()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "Priority Queue vacía.");
        }

        return first.Priority;
    }

    public void Clear()
    {
        first = null;
        last = null;
        count = 0;
    }

    public T[] ToArray()
    {
        T[] result = new T[count];

        PriorityLinkedNode<T> current = first;

        int index = 0;

        while (current != null)
        {
            result[index] = current.Item;

            current = current.Next;
            index++;
        }

        return result;
    }
}

public class PriorityLinkedNode<T>
{
    public T Item;
    public int Priority;
    public PriorityLinkedNode<T> Next;

    public PriorityLinkedNode(T item, int priority)
    {
        Item = item;
        Priority = priority;
        Next = null;
    }
}
