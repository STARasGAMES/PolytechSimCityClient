using Models.Response;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tools.Patterns;
using UnityEngine;
using UnityEngine.Networking;
 

namespace Tools
{
    public class URL : Singleton<URL>
    {

        private const string ASSET_BUNDLE_FOLDER = "asset_bundles";
        private const string PLAYER_PREFS_BUNDLE_HASH = "BundleHashFor:";
#if !UNITY_IOS && USE_TEXTURE_CACHE
        private Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();
#endif

        public IEnumerator GetSpriteFromURL(string url, Action<Sprite> callback)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
			yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                callback(null);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                callback(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)));
            }
        }

        public IEnumerator GetTexture2DFromURL(string url, Action<Texture2D> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException("Texture URL is null or empty!");
            }
            else
            {
                using (var request = UnityWebRequestTexture.GetTexture(url))
                {
					yield return request.SendWebRequest();
                    if (request.isNetworkError)
                    {
                        callback(null);
                    }
                    else
                    {
                        callback(DownloadHandlerTexture.GetContent(request));
                    }
                }
            }
        }

        public IEnumerator GetTexture2DFromURLCache(string url, Action<Texture2D> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException("Texture URL is null or empty!");
            }
            else
            {
                const int nameLength = 32;
                string[] arr = url.Split('/');
                string shortUrl = arr[arr.Length - 1];
                arr = shortUrl.Split('.');
                shortUrl = arr[0];
                //Debug.Log("Short URL: "+shortUrl);
                string fileName = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(shortUrl));
                //Debug.Log("File Name: "+fileName);
                fileName = fileName.Substring(0, nameLength);
#if !UNITY_IOS && USE_TEXTURE_CACHE
                if (_loadedTextures.ContainsKey(fileName))
                {
                    callback(_loadedTextures[fileName]);
                    yield break;
                }
#endif
                //Debug.Log("File Name(Cut): "+fileName);
                string path = Application.persistentDataPath + "/" + fileName;
                //Debug.Log("Path " + path);
                Texture2D texture;
                if (System.IO.File.Exists(path))
                {
                    //Debug.Log("Exist in cache");
                    var webRequest = UnityWebRequestTexture.GetTexture("file:///" + path);
					yield return webRequest.SendWebRequest();
                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError("ERROR");
                        yield break;
                    }
                    texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                    //var ccc = new WWW("file:///" + path);
                    //while (!ccc.isDone)
                    //{
                    //    //Debug.Log("Extracting...");
                    //    yield return null;
                    //}
                    //texture = ccc.texture;
                    //Debug.Log("Extracted!");
                }
                else
                {
                    //Debug.Log("Need to download");
                    var webRequest = UnityWebRequestTexture.GetTexture(url);
					yield return webRequest.SendWebRequest();
                    if (webRequest.isNetworkError)
                    {
                        Debug.LogError("ERROR");
                    }
                    texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                    yield return null;
                    //var www = new WWW(url);
                    //while (!www.isDone)
                    //{
                    //    //Debug.Log("Downloading...");
                    //    yield return null;
                    //}
                    //texture = www.texture;
                    System.IO.File.WriteAllBytes(path, webRequest.downloadHandler.data);
                    //Debug.Log("Downloaded!");
                }
                yield return null;
                if (texture != null)
                {
#if !UNITY_IOS && USE_TEXTURE_CACHE
					if (!_loadedTextures.ContainsKey(fileName))
                    	_loadedTextures.Add(fileName, texture);
#endif
                    callback(texture);
                }
                else
                {
                    Debug.LogError("GetTexture2DFromURLCache: Error!");
                    callback(null);
                }
            }
        }

        private string GetPathToAssetBundle(string hash)
        {
            return Application.persistentDataPath + "/" + ASSET_BUNDLE_FOLDER + "/" + hash;
        }

        private string GetPlayerPrefsName(string roomId)
        {
            return PLAYER_PREFS_BUNDLE_HASH + roomId;
        }

        public IEnumerator GetAssetBundle(string roomId, string hash, string url, bool useCache, Action<AssetBundle> callback, Action<float> progress)
        {
            //url = "http://www.oxsionserver.ru/download/arena";
            //url = "file://D:/WORK/ARENA/Arena VR (5.6.1)/AssetBundles/Android/arena";
            Debug.Log("Here! " + url);
            AssetBundle assetBundle = null;
            UnityWebRequest req = null;
            if (useCache)
                req = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Parse(hash), 0);
            else
                req = UnityWebRequestAssetBundle.GetAssetBundle(url);
			var operation = req.SendWebRequest();
            while(!operation.isDone)
            {
                //Debug.Log("Downloading asset bundle. Progress: " + operation.progress);
                progress(operation.progress);
                yield return null;
            }
            assetBundle = ((DownloadHandlerAssetBundle)req.downloadHandler).assetBundle;
            callback(assetBundle);
            yield break;
            if (string.IsNullOrEmpty(roomId) || string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(url))
                throw new NullReferenceException(string.Format("roomId, hash or url empty or NULL! {0} {1} {2}", roomId, hash, url));
            
            yield return GetAssetBundleByHash(hash, ab => assetBundle = ab);
            if (assetBundle != null)
            {
                callback(assetBundle);
                yield break;
            }
            // if no such asset bundle in cache:
            if (PlayerPrefs.HasKey(GetPlayerPrefsName(roomId))) 
            {
                // delete old asset bundle for room
                string oldHash = PlayerPrefs.GetString(GetPlayerPrefsName(roomId));
                string oldPath = GetPathToAssetBundle(oldHash);
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }
            yield return GetAssetBundleFromURL(hash, url, ab => assetBundle = ab);
            if (assetBundle != null)
            {
                PlayerPrefs.SetString(GetPlayerPrefsName(roomId), hash);
                callback(assetBundle);
                yield break;
            }
            Debug.LogError("Error!");
            callback(null);
        }

        /// <summary>
        /// Returns the cached asset bundle with given hash. If no cached file has such hash, then returns NULL
        /// </summary>
        /// <param name="hash">string value, provided by server</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetAssetBundleByHash(string hash, Action<AssetBundle> callback)
        {
            if (string.IsNullOrEmpty(hash))
                throw new NullReferenceException("AssetBundle hash is null or empty!");
            string path = GetPathToAssetBundle(hash);
            if (!System.IO.File.Exists(path))
            {
                callback(null);
                yield break;
            }
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle("file://" + path);
            if (webRequest.isNetworkError)
            {
                Debug.LogError("ERROR");
                callback(null);
                yield break;
            }
            callback(((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle);
        }

        /// <summary>
        /// Returns asset bundle downloaded by given URL. Returns NULL if error.
        /// </summary>
        /// <param name="hash">string value, provided by server</param>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetAssetBundleFromURL(string hash, string url, Action<AssetBundle> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new NullReferenceException("Texture URL is null or empty!");
            }
            string path = GetPathToAssetBundle(hash);
            AssetBundle assetBundle;
            var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
            Debug.Log("Downloading asset bundle...");
			yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.LogError("ERROR");
                callback(null);
                yield break;
            }
            assetBundle = ((DownloadHandlerAssetBundle)webRequest.downloadHandler).assetBundle;
            yield return null;

            if (assetBundle != null)
            {
                System.IO.File.WriteAllBytes(path, webRequest.downloadHandler.data);
                yield return null;
                Debug.Log("Downloaded!");
                callback(assetBundle);
            }
            else
            {
                Debug.LogError("Error!");
                callback(null);
            }
        }

        public IEnumerator SaveFileToMemory(string url, string pathToFile, Action<string> finishAction, Action<float> proccessAction)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var request = UnityWebRequest.Get(url);
				var asyncOperation = request.SendWebRequest();
                while (!asyncOperation.isDone)
                {
                    yield return null;
                    proccessAction(asyncOperation.progress);
                }
                if (request.isNetworkError)
                {
                    finishAction(request.error);
                }
                else
                {
                    yield return null;
                    File.WriteAllBytes(pathToFile, request.downloadHandler.data);
                    finishAction(null);
                    //var obg = request.downloadHandler.data;
                }
            }
            else
            {
                finishAction("URL is NULL or EMPTY");
            }
        }

        public IEnumerator GetFileSizeFromURL(string url, Action<string> callback)
        {
            var request = UnityWebRequest.Head(url);
			yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                callback("0");
            }
            else
            {
                var responseHeaders = request.GetResponseHeaders();
                if (responseHeaders != null && responseHeaders.ContainsKey("Content-Length"))
                {
                    callback(responseHeaders["Content-Length"]);
                }
                else
                {
                    callback("0");
                }
            }
        }
    }
}
