using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameControllers.Core.Controller
{
    public abstract class SceneControllerBase : ViewControllerBase
    {
        [SerializeField] private string _sceneName;

        protected bool _isError;
        protected string _error;

        public bool IsInitError
        {
            get { return _isError; }
        }

        public string SceneName
        {
            get { return _sceneName; }
        }

        public override void Init() { }

        public virtual IEnumerator InitSceneLogic()
        {
            _isError = false;
            yield return null;
        }

        public virtual void RunSceneLogic()
        {

        }

        public override IEnumerator Dispose()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
            yield return base.Dispose();
        }
#if OCULUS
        protected virtual void OnApplicationPause(bool pause)
        {
            Debug.Log("OnApplicationPause: " + pause.ToString());
            
        }
#endif
    }
}
