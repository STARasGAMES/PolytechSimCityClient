using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Models.Request;
using Models.Response;

namespace Core
{
    public partial class GameSystem
    {
        public static Managers.ManagerServerCommunication ServerCommunication { get { return Instance.GetManager<Managers.ManagerServerCommunication>(); } }
    }
}

namespace Managers
{
    public class ManagerServerCommunication : IManager
    {
        //private const string SERVER_URL = "http://52.55.223.22/mobile/"; // DEBUG
        //private const string SERVER_URL = "https://vr.ceek.com/mobile/"; // RELEASE 
        private const string SERVER_URL = "http://localhost:5000/api/"; // LOCAL DEBUG

        private const int REQUEST_TIMEOUT = 30;

        public void Init() { }

        public IEnumerator Dispose()
        {
            yield return null;
        }

        public IEnumerator GetTestRoute(UnityAction<SimpleResponse> callback)
        {
            var request = new SimpleGetRequest(SERVER_URL);
            yield return SendRequestWithTimer(request);
            var res = ParseResponse<SimpleResponse>(request);
            callback(res);
        }

        #region Private Methods

        private static T ParseResponse<T>(RequestBase request) where T : SimpleResponse, new()
        {
            var json = "{}";
            if (request.UnityWebRequest.downloadHandler != null)
            {
                json = request.UnityWebRequest.downloadHandler.text;
            }

            //TODO КОСТЫЛЬ!!!!!!!!!
            if (json[0] == '[')
            {
                json = "{\"data\":" + json + "}";
            }
#if UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_ANDROID)
            string responseName = typeof(T).ToString().Split('.').Last();
            string fileName = System.DateTime.Now.ToFileTime() + "_" + responseName;
            string path = Application.persistentDataPath + "/jsons/" + fileName;
            string newjson = json.Replace(",", ",\n");

            Debug.Log(responseName + "   Response:\n" + newjson);
            try
            {
                newjson = JsonUtility.ToJson(
                    JsonUtility.FromJson<T>(json),
                    true
                );//json.Replace(",", ",\n");

                Debug.Log(responseName + "   Parsed Response:\n" + newjson);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            System.IO.File.WriteAllText(path, newjson);
#endif
            if (request.UnityWebRequest.responseCode < 300 && request.UnityWebRequest.responseCode >= 200)
            {
                try
                {

                    return JsonUtility.FromJson<T>(json);
                }
                catch (Exception e)
                {
                    return new T()
                    {
                        error = "Exception! " + e.Message + "\nJSON: " + json
                    };
                }
            }
            if (request.UnityWebRequest.responseCode == 400)
            {
                var respounse = JsonUtility.FromJson<T>(json);
                respounse.errorCode = request.UnityWebRequest.responseCode;
                respounse.responseText = request.UnityWebRequest.downloadHandler.text;
                return respounse;
            }
            return new T()
            {
                errorCode = request.UnityWebRequest.responseCode,
                error = request.UnityWebRequest.error
            };
        }

        private static string RemoveJsonArrayField(string json, string fieldName)
        {
            int startIndex = -1;
            int wtf = 0;
            while ((startIndex = json.IndexOf(fieldName)) > 0)
            {
                int endIndex = json.IndexOf("],", startIndex);
                json = json.Remove(startIndex, endIndex - startIndex + 3);
                if (++wtf > 100)
                {
                    Debug.LogError("WTF");
                    break;
                }
            }
            return json;
        }

        private static IEnumerator SendRequestWithTimer(RequestBase request)
        {
            //ToolsGetter.Cookies.TrySetCookieInRequest(request.UnityWebRequest);
            bool flag = true;
            float timeToNewRequest = 1;
            float maxTimeToNewRequest = 10;
            do
            {
                Debug.Log("Sending request...");
                var asyncOperation = request.Send();
                float timer = REQUEST_TIMEOUT;
                while (!asyncOperation.isDone)
                {
                    timer -= Time.fixedDeltaTime;
                    yield return null;
                    if (timer <= 0)
                    {
                        request.Abort();
                        break;
                    }
                }
                if (!request.UnityWebRequest.isNetworkError && !request.UnityWebRequest.isHttpError && request.UnityWebRequest.isDone)
                {
                    flag = false;
                }
                else
                {
                    if (timeToNewRequest > maxTimeToNewRequest)
                    {
                        Debug.LogError("Connection to server failed!");
                        yield break;
                    }
                    request.InitRequest();
                    Debug.LogError($"code: {request.UnityWebRequest.responseCode}, isNetworkError: {request.UnityWebRequest.isNetworkError}, isHttpError: {request.UnityWebRequest.isHttpError}, error: {request.UnityWebRequest.error}");
                    float time = timeToNewRequest;
                    timeToNewRequest *= 2;
                    while(time > 0)
                    {
                        Debug.Log($"{(int)time} seconds befor next attempt...");
                        yield return new WaitForSecondsRealtime(1f);
                        time--;
                    }
                }

            } while (flag);
        }
        #endregion
    }
}
