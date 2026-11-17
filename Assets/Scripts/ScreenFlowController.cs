using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFlowController : MonoBehaviour
{
    private static ScreenFlowController instance;

    [SerializeField] private string[] screenOrder;

    private ScreenPriorityQueue<string> screenQueue;
    private readonly Stack<string> screenHistory = new Stack<string>();
    private int nextPriority;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        screenQueue = new ScreenPriorityQueue<string>();

        if (screenOrder == null)
        {
            return;
        }

        foreach (string screen in screenOrder)
        {
            if (!string.IsNullOrWhiteSpace(screen))
            {
                screenQueue.Enqueue(screen, nextPriority++);
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void EnqueueScreen(string screenName)
    {
        if (!string.IsNullOrWhiteSpace(screenName))
        {
            screenQueue.Enqueue(screenName, nextPriority++);
        }
    }

    public void ShowNextScreen()
    {
        if (screenQueue.IsEmpty)
        {
            Debug.LogWarning("No hay más pantallas en la cola.", this);
            return;
        }

        string nextScreen = screenQueue.Dequeue();
        string currentScreen = SceneManager.GetActiveScene().name;

        if (currentScreen != nextScreen)
        {
            screenHistory.Push(currentScreen);
            SceneManager.LoadScene(nextScreen);
        }
    }

    public void GoBack()
    {
        if (screenHistory.Count == 0)
        {
            Debug.LogWarning("No hay pantallas anteriores en el historial.", this);
            return;
        }

        SceneManager.LoadScene(screenHistory.Pop());
    }
}

// Cola enlazada con prioridad, basada en SimpleLinkedPriorityQueue<T>.
// Un valor de prioridad menor se atiende primero.
internal sealed class ScreenPriorityQueue<T>
{
    private ScreenPriorityNode<T> first;
    private ScreenPriorityNode<T> last;
    private int count;

    public int Count => count;
    public bool IsEmpty => count == 0;

    public void Enqueue(T item, int priority)
    {
        ScreenPriorityNode<T> newNode = new ScreenPriorityNode<T>(item, priority);

        if (IsEmpty)
        {
            first = newNode;
            last = newNode;
            count++;
            return;
        }

        if (priority < first.Priority)
        {
            newNode.Next = first;
            first = newNode;
            count++;
            return;
        }

        ScreenPriorityNode<T> current = first;

        while (current.Next != null && current.Next.Priority <= priority)
        {
            current = current.Next;
        }

        newNode.Next = current.Next;
        current.Next = newNode;

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
            throw new InvalidOperationException("La cola de pantallas está vacía.");
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
}

internal sealed class ScreenPriorityNode<T>
{
    public T Item;
    public int Priority;
    public ScreenPriorityNode<T> Next;

    public ScreenPriorityNode(T item, int priority)
    {
        Item = item;
        Priority = priority;
        Next = null;
    }
}
