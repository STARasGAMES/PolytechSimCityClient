using System.Collections;
using UnityEngine;

namespace Core
{
    public partial class GameSystem : MainSystemBase
    {        
        #region Unity methods

        void Awake()
        {
            Application.runInBackground = false;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            Init();
            LoadNextScenes();
        }

        public static Coroutine StartMainCoroutine(IEnumerator enumerator)
        {
            return Instance.StartCoroutine(enumerator);
        }
        
        private void LoadNextScenes()
        {
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                //TODO

            }
            catch
            {
                Debug.LogError("You don't set second scene! Please add scene in Build Setings or Project/Scenes");
            }
            
        }    

        void Update()
        {
            Events.Invoke<OnUpdate>();
        }

        void LateUpdate()
        {
        }

        public void OnApplicationQuit()
        {
            DisposeAllManagers();
        }

#endregion
    }
}
