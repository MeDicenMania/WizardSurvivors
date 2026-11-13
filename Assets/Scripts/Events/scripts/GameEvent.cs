using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise(Component sender, object data)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            if (listeners[i] == null)
            {
                listeners.RemoveAt(i);
                continue;
            }

            listeners[i].OnEventRaised(sender, data);
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }

    }


    public void Unregister(GameEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);

        }
    }

}