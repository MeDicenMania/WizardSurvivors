using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the order in which game scenes are shown.
/// The queue defines the forward flow (FIFO) and the stack stores
/// visited scenes for back navigation (LIFO).
/// </summary>
public class ScreenFlowController : MonoBehaviour
{
    [SerializeField] private string[] screenOrder;

    private readonly Queue<string> screenQueue = new Queue<string>();
    private readonly Stack<string> screenHistory = new Stack<string>();

    private void Awake()
    {
        foreach (string screen in screenOrder)
        {
            if (!string.IsNullOrWhiteSpace(screen))
            {
                screenQueue.Enqueue(screen);
            }
        }
    }

    /// <summary>Adds a screen to the end of the forward flow.</summary>
    public void EnqueueScreen(string screenName)
    {
        if (!string.IsNullOrWhiteSpace(screenName))
        {
            screenQueue.Enqueue(screenName);
        }
    }

    /// <summary>Opens the next queued screen and records the current one.</summary>
    public void ShowNextScreen()
    {
        if (screenQueue.Count == 0)
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

    /// <summary>Returns to the most recently visited screen.</summary>
    public void GoBack()
    {
        if (screenHistory.Count == 0)
        {
            Debug.LogWarning("No hay pantallas anteriores en el historial.", this);
            return;
        }

        string previousScreen = screenHistory.Pop();
        SceneManager.LoadScene(previousScreen);
    }
}
