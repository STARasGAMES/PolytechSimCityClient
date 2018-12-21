using System;
using System.Collections;
using UnityEngine;

namespace GameControllers.Core
{
    public abstract class ViewControllerBase : MonoBehaviour
    {
        private Transform _transform;
        private RectTransform _rectTransform;

        public new Transform transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }
                return _transform;
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }

        //<summary>Init Component ones when scene is loaded</summary>
        public abstract void Init();
        
        public virtual void SetVisibility(bool isVisivle)
        {
            gameObject.SetActive(isVisivle);
        }

        public virtual IEnumerator Dispose()
        {
            yield break;
        }
    }
}
