using System;
using UnityEngine;

public class SimpleArrayPriorityQueue<T> : ISimplePriorityQueue<T>
{
    private T[] items;
    private int[] priorities;
    private int count;

    public int Count => count;

    public bool IsEmpty => count == 0;

    public SimpleArrayPriorityQueue(int capacity = 10)
    {
        items = new T[capacity];
        priorities = new int[capacity];
        count = 0;
    }

    public void Enqueue(T item, int priority)
    {
        if (count == items.Length)
        {
            throw new InvalidOperationException("Priority Queue llena.");
        }

        int insertIndex = count;

        // Buscamos dónde insertar según la prioridad.
        // Menor número = mayor prioridad.
        for (int i = count; i > 0; i--)
        {
            if (priority < priorities[i - 1])
            {
                items[i] = items[i - 1];
                priorities[i] = priorities[i - 1];

                insertIndex = i - 1;
            }
            else
            {
                break;
            }
        }

        items[insertIndex] = item;
        priorities[insertIndex] = priority;

        count++;
    }

    public T Dequeue()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Priority Queue vacía.");
        }

        T item = items[0];

        ShiftLeft();

        count--;

        return item;
    }

    public T Peek()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Priority Queue vacía.");
        }

        return items[0];
    }

    public int GetHighestPriority()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("Priority Queue vacía.");
        }

        return priorities[0];
    }

    public void Clear()
    {
        count = 0;
    }

    public T[] ToArray()
    {
        T[] result = new T[count];

        for (int i = 0; i < count; i++)
        {
            result[i] = items[i];
        }

        return result;
    }

    private void ShiftLeft()
    {
        for (int i = 1; i < count; i++)
        {
            items[i - 1] = items[i];
            priorities[i - 1] = priorities[i];
        }
    }
}

