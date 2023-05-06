using UnityEngine;

namespace _00___Game_Data.Scripts.Core.Utils.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (_instance == null)
                    {
                        SetupInstance();
                    }
                }

                return _instance;
            }
        }

        public void Awake()
        {
            RemoveDuplicates();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static void SetupInstance()
        {
            _instance = (T)FindObjectOfType(typeof(T));
            if (_instance == null)
            {
                GameObject gameObj = new GameObject();
                gameObj.name = typeof(T).Name;
                _instance = gameObj.AddComponent<T>();
                //DontDestroyOnLoad(gameObj);
            }
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}