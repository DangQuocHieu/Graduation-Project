namespace DQHieu.Framework
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class EventBus : MonoBehaviour
    {
        private static readonly Dictionary<Type, Delegate> _messageHandlers = new Dictionary<Type, Delegate>();

        public static void Subcribe<T>(Action<T> messageHandler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!_messageHandlers.ContainsKey(type))
            {
                _messageHandlers[type] = null;
            }
            _messageHandlers[type] = Delegate.Combine(_messageHandlers[type], messageHandler);

        }

        public static void UnSubcribe<T>(Action<T> messageHandler) where T : IGameEvent
        {
            var type = typeof(T);
            if (_messageHandlers.ContainsKey(type))
            {
                _messageHandlers[type] = Delegate.Remove(_messageHandlers[type], messageHandler);
                if (_messageHandlers[type] == null)
                {
                    _messageHandlers.Remove(type);
                }
            }
        }

        public static void SendMessage<T>(T message) where T : IGameEvent
        {
            var type = typeof(T);
            if (_messageHandlers.ContainsKey(type))
            {
                (_messageHandlers[type] as Action<T>)?.Invoke(message);
            }
        }
    }

}