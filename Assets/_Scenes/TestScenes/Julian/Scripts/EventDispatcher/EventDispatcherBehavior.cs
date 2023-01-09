using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventDispatcherBehavior : MonoBehaviour, IEventDispatcher
{
    private NotificationDispatcher _dispatcher;

    public void AddListener(string eventKey, Action<GeneralEvent> callback)
    {
        dispatcher.AddListener(eventKey, callback);
    }
    public void RemoveListener(string eventKey, Action<GeneralEvent> callback)
    {
        dispatcher.RemoveListener(eventKey, callback);
    }
    public bool HasListener(string eventKey)
    {
        return dispatcher.HasListener(eventKey);
    }
    public void RemoveAllListenersOfEvent(string eventKey)
    {
        dispatcher.RemoveAllListenersOfEvent(eventKey);
    }
    public void RemoveAllListeners()
    {
        dispatcher.RemoveAllListeners();
    }
    public bool DispatchEvent(string eventKey, bool bubbles = false, object eventData = null)
    {
        return dispatcher.DispatchEvent(eventKey, bubbles, eventData);
    }
    public bool DispatchEvent(GeneralEvent e)
    {
       return dispatcher.DispatchEvent(e);
    }

    private NotificationDispatcher dispatcher
    {
        get
        {
            if(_dispatcher == null)
            {
                _dispatcher = new NotificationDispatcher(transform);
            }
            return _dispatcher;
        }
    }
}

