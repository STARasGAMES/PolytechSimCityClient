using UnityEngine;
using UnityEngine.Networking;

namespace Models.Request
{
    public abstract class RequestBase
    {
        protected readonly string URL;
        protected UnityWebRequest _unityWebRequest;

        public UnityWebRequest UnityWebRequest
        {
            get
            {
                if (_unityWebRequest == null)
                {
                    InitRequest();
                }
                return _unityWebRequest;
            }
        }

        protected RequestBase(string url)
        {
            URL = url;
        }

        public UnityWebRequestAsyncOperation Send()
        {
			return UnityWebRequest.SendWebRequest();
        }

        public void Abort()
        {
            if (_unityWebRequest != null)
            {
                _unityWebRequest.Abort();
            }
        }

        public abstract void InitRequest();
    }
}
