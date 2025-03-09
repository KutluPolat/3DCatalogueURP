using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace KPFramework
{
    public class Events
    {
        [System.Serializable]
        public class KPEvent : UnityEvent<object> { }
        private static Dictionary<int, KPEvent> _eventList;
        private static bool _isInitialized;
        private static void Initialize()
        {
            if (_isInitialized)
                return;

            _eventList = new Dictionary<int, KPEvent>();

            foreach (EventName eventName in Enum.GetValues(typeof(EventName)))
            {
                _eventList.Add((int)eventName, new KPEvent());
            }

            _isInitialized = true;
        }

        public static void AddEvent(EventName name, UnityAction<object> action)
        {
            Initialize();
            _eventList[(int)name].AddListener(action);
        }

        public static void RemoveEvent(EventName name, UnityAction<object> action)
        {
            Initialize();
            _eventList[(int)name].RemoveListener(action);
        }

        public static void InvokeEvent(EventName name, object param = null)
        {
            Initialize();
            _eventList[(int)name]?.Invoke(param);
        }

        public static KPEvent GetEvent(EventName name)
        {
            Initialize();
            return _eventList[(int)name];
        }
    }
}