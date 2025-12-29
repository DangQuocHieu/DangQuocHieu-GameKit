namespace DangQuocHieu.GameKit
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    public class EventBus : MonoBehaviour
    {
        private static readonly Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> eventHandler) where T : IGameEvent
        {
            var type = typeof(T);
            if(!_eventHandlers.ContainsKey(type))
            {
                _eventHandlers[type] = null;
            }
            _eventHandlers[type] = Delegate.Combine(_eventHandlers[type], eventHandler);
        }

        public static void UnSubscribe<T>(Action<T> eventHandler) where T : IGameEvent
        {
            var type = typeof(T);   
            if(!_eventHandlers.ContainsKey(type))
            {
                _eventHandlers[type] = Delegate.Remove(_eventHandlers[type], eventHandler);
                if (_eventHandlers[type] == null)
                {
                    _eventHandlers.Remove(type);
                }
            }
        }

        public static void Raise<T>(T gameEvent) where T : IGameEvent
        {
            var type = typeof(T);
            if(_eventHandlers.ContainsKey(type))
            {
                (_eventHandlers[type] as Action<T>)?.Invoke(gameEvent);
            }
        }
    }

}