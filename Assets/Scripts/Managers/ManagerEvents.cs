using System;
using System.Collections;
using System.Collections.Generic;
using Events.Core;

namespace Core
{
    public partial class GameSystem 
    {
        public static Managers.ManagerEvents Events { get { return Instance.GetManager<Managers.ManagerEvents>(); } }
    }
}

namespace Managers
{
    public class ManagerEvents : IManager {
        private readonly Dictionary<Type, EventBase> _eventDictionary = new Dictionary<Type, EventBase>();

#if UNITY_EDITOR
        public Dictionary<Type, EventBase> EventDictionary { get { return _eventDictionary; } }
#endif
        public void Init()
        {
            var eventTypeArray = Tools.Assembly.Instance.GetSubclassListThroughHierarchy<EventBase>(false);
            foreach (var type in eventTypeArray)
            {
                var eventObj = (EventBase)Activator.CreateInstance(type);
                _eventDictionary.Add(type, eventObj);
            }
        }

        public void Invoke<T>() where T : EventBase
        {
            _eventDictionary[typeof (T)].Invoke();
        }
        public void AddListener<T>(Action action) where T : EventBase
        {
            _eventDictionary[typeof (T)].Add(action);
        }

        public void RemoveListener<T>(Action action) where T : EventBase
        {
            _eventDictionary[typeof (T)].Remove(action);
        }

        public void ClearAllListeners<T>() where T : EventBase
        {
            _eventDictionary[typeof (T)].Clear();
        }

        public IEnumerator Dispose()
        {
            foreach (var dictItem in _eventDictionary)
            {
                dictItem.Value.Clear();
            }
            yield return null;
        }
    }
}
