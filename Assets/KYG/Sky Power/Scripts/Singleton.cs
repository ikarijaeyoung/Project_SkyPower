using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KYG_skyPower
{

    // 추상화 및 커스텀 Init 확장성을 위해
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance != null) return instance;

                    GameObject go = new GameObject(typeof(T).Name); // 이름 동적 생성!
                    instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                OnAwake(); // 커스텀 확장 지점
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // 자식에서 커스텀 Awake
        protected virtual void OnAwake() { }

        // 매니저 일괄 Init을 위한 추상화 (Manager에서 직접 호출 가능)
        public virtual void Init() { }

        private void OnApplicationQuit()
        {
            instance = null;
        }

        public void DestroyManager()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
