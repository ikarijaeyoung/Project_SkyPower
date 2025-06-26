using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class BaseUI : MonoBehaviour
    {
        private Dictionary<string, GameObject> goDict;
        private Dictionary<string, GameObject> compDict;
        private void Awake()
        {
        }
        void Start()
        {

        }

        void Update()
        {

        }

        public GameObject GetUI(in string name)
        {
            goDict.TryGetValue(name, out GameObject gameObject);
            if(gameObject == null)
            {
                Debug.LogError($"다음 UI 오브젝트가 없습니다: {name}");
            }
            return gameObject; // 없을경우 Null
        }
    }
}

