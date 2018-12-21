using System;
using System.Linq;

namespace Events.Core
{
    public abstract class EventBase {
        private event Action _event;
#if UNITY_EDITOR
        public Action Event { get { return _event; } }
#endif
        public void Invoke() {
            if (_event != null) {
                _event.Invoke();
            }
        }

        public void Add(Action action) {
            if (_event == null || (_event != null && !_event.GetInvocationList().Contains(action))) {
                _event += action;
            }
        }

        public void Remove(Action action) {
            if (_event != null && _event.GetInvocationList().Contains(action)) {
                _event -= action;
            }
        }

        public void Clear() {
            _event = null;
        }
    }
}
