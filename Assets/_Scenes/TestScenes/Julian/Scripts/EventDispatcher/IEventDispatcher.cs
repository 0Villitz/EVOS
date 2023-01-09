using System;
using System.Collections;
using UnityEngine;

public interface IEventDispatcher
{
    void AddListener(string eventKey, Action<GeneralEvent> callback);
    bool HasListener(string eventKey);
    void RemoveListener(string eventKey, Action<GeneralEvent> callback);
    void RemoveAllListenersOfEvent(string eventKey);
    void RemoveAllListeners();
    bool DispatchEvent(string eventKey, bool bubble = false, object eventData = null);
    bool DispatchEvent(GeneralEvent e);
}
