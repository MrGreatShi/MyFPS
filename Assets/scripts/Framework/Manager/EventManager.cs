using System;
using System.Collections.Generic;

namespace Game.Foundation
{
    /// <summary>
    /// 强类型事件总线：用 struct/class 作为事件类型。
    /// 用法：
    ///   EventManager.Subscribe<PlayerDied>(OnPlayerDied);
    ///   EventManager.Publish(new PlayerDied { Id = 1 });
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<Type, Delegate> _handlers = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;

            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var del))
                _handlers[type] = (Action<T>)del + handler;
            else
                _handlers[type] = handler;
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;

            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var del)) return;

            var newDel = (Action<T>)del - handler;
            if (newDel == null) _handlers.Remove(type);
            else _handlers[type] = newDel;
        }

        public static void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var del))
                ((Action<T>)del)?.Invoke(evt);
        }

        public static void ClearAll() => _handlers.Clear();
    }
}
