using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using JetBrains.Annotations;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;

    public CustomGameEvent response;


    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        gameEvent.Unregister(this);
    }

    public void OnEventRaised(Component sender,object data)
    {
        response.Invoke(sender,data);
    }
}
