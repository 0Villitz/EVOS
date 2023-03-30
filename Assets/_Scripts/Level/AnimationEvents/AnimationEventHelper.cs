using System;
using UnityEngine;

namespace Game2D
{
    public class AnimationEventHelper : MonoBehaviour
    {
        private event Action<AnimationEvent> _onAnimationEvent;

        public void AddEvent(Action<AnimationEvent> eventMethod)
        {
            _onAnimationEvent += eventMethod;
        }
        
        public void RemoveEvent(Action<AnimationEvent> eventMethod)
        {
            _onAnimationEvent -= eventMethod;
        }
        
        public void OnAnimationEvent(AnimationEvent eventData)
        {
            _onAnimationEvent?.Invoke(eventData);
        }
    }
}