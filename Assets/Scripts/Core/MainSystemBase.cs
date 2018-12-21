using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Tools;
using Tools.Patterns;

namespace Core
{
    public abstract class MainSystemBase : MonoBehaviourSingleton<MainSystemBase>
    {

        private readonly Dictionary<Type, IManager> _managers = new Dictionary<Type, IManager>();
        protected virtual void Init()
        {
            var managerTypes = ToolsGetter.Assembly.GetSubclassListThroughHierarchy<IManager>();
            foreach (var managerType in managerTypes)
            {
                var manager = (IManager)Activator.CreateInstance(managerType);
                _managers.Add(managerType, manager);
            }

            _managers.ToList().ForEach(x => x.Value.Init());
        }

        public T GetManager<T>() where T : IManager
        {
            var type = typeof(T);
            if (_managers.ContainsKey(type))
            {
                return (T)_managers[type];
            }
            return default(T);
        }

        public void DisposeAllManagers()
        {
            _managers.ToList().ForEach(x => x.Value.Dispose());
        }
    }
}
