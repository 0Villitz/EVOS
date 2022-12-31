using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

    public class NotificationDispatcher : IEventDispatcher
    {
        protected Transform _bubbler;
        private Dictionary<string, List<Action<GeneralEvent>>> _eventDictionary = new Dictionary<string, List<Action<GeneralEvent>>>();

        public NotificationDispatcher()
        {

        }

        public NotificationDispatcher(Transform bubbler)
        {
            _bubbler = bubbler;
        }

        public void AddListener(string eventKey, Action<GeneralEvent> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<GeneralEvent>> callbackList = null;
            if (!_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList = new List<Action<GeneralEvent>>();
                _eventDictionary.Add(eventKey, callbackList);
            }
            callbackList.Add(callback);
        }

        public void RemoveListener(string eventKey, Action<GeneralEvent> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<GeneralEvent>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                int index = callbackList.FindIndex((x) => x == callback);
                if(index >= 0 && index < callbackList.Count)
                {
                    callbackList.RemoveAt(index);
                }
            }
        }

        public bool HasListener(string eventKey)
        {
            Assert.IsNotNull(_eventDictionary);
            return _eventDictionary.ContainsKey(eventKey);
        }

        public void RemoveAllListenersOfEvent(string eventKey)
        {
            List<Action<GeneralEvent>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList.Clear();
            }
        }

        public void RemoveAllListeners()
        {
            _eventDictionary.Clear();
        }

        public bool DispatchEvent(string eventKey, bool bubbles = false, object eventData = null)
        {
            //TODO pool these event objects and reuse them
            GeneralEvent e = new GeneralEvent();
            e.type = eventKey;
            e.target = this;
            e.currentTarget = this;
            e.data = eventData;
            e.isBubbling = canBubble(bubbles);

            return _invokeDispatchEvent(e);
        }

        public bool DispatchEvent(GeneralEvent e)
        {
            return _invokeDispatchEvent(e);
        }

        private bool canBubble(bool bubble)
        {
            return bubble && _bubbler != null;
        }

        private bool _invokeDispatchEvent(GeneralEvent e)
        {
            bool result = false;

            List<Action<GeneralEvent>> callbackList = null;
            if (_eventDictionary.TryGetValue(e.type, out callbackList))
            {
                int length = callbackList.Count;
                for (int i = 0; i < length; ++i)
                {
                    if (callbackList[i] != null)
                    {
                        callbackList[i].Invoke(e);
                    }
                }

                result = true;
            }
            else if(canBubble(e.isBubbling))
            {
                e.isBubbling = false; // Don't double bubble
                Transform parent = _bubbler.parent;
                IEventDispatcher[] dispatcherList = parent.GetComponentsInParent<IEventDispatcher>();
                for(int i = 0; i < dispatcherList.Length; ++i)
                {
                    e.currentTarget = dispatcherList[i];
                    if (dispatcherList[i].DispatchEvent(e))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }

