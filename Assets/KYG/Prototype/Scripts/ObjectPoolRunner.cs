using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KYG
{
    public class ObjectPoolRunner : MonoBehaviour // 오브젝트 풀 매니저에 초기 풀 생성
    {
        public ObjectPoolManagerSO poolManager;
        public GameObject prefab;
        public string key = "Bullet";

        void Awake()
        {
            poolManager.CreatePool(key, prefab, 20);
        }
    }
}