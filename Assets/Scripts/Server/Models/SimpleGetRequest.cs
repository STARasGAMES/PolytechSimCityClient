using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Models.Request
{

    public class SimpleGetRequest : RequestBase
    {

        public SimpleGetRequest(string url) : base(url + "values") { }

        public override void InitRequest()
        {
            _unityWebRequest = UnityWebRequest.Get(URL);
        }
    }
}
