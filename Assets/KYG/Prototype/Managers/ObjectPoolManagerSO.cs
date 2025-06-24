using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    [CreateAssetMenu(fileName = "ObjectPoolManager", menuName = "Managers/ObjectPoolManager")]
    public class ObjectPoolManagerSO : ScriptableObject
    {
        private Dictionary<string, Queue<GameObject>> pool = new();
        private Dictionary<string, GameObject> prefabDictionary = new(); // 원본 프리팹 저장

        // 오브젝트 풀 생성
        public void CreatePool(string key, GameObject prefab, int count)
        {
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            if (!prefabDictionary.ContainsKey(key))
                prefabDictionary[key] = prefab;

            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool[key].Enqueue(obj);
            }
        }

        // 풀에서 꺼내기 (부족하면 자동 생성)
        public GameObject Spawn(string key, Vector3 pos, Quaternion rot)
        {
            if (!pool.ContainsKey(key))
            {
                Debug.LogWarning($"[Pool] Key '{key}' not found!");
                return null;
            }

            if (pool[key].Count == 0)
            {
                // 자동 확장
                if (prefabDictionary.ContainsKey(key))
                {
                    Debug.LogWarning($"[Pool] Pool '{key}' empty. Instantiating additional object.");
                    GameObject extra = Instantiate(prefabDictionary[key]);
                    extra.SetActive(false);
                    pool[key].Enqueue(extra);
                }
                else
                {
                    Debug.LogError($"[Pool] No prefab registered for key: {key}");
                    return null;
                }
            }

            GameObject obj = pool[key].Dequeue();
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.SetActive(true);

            return obj;
        }

        // 풀에 반환
        public void Despawn(string key, GameObject obj)
        {
            obj.SetActive(false);
            if (!pool.ContainsKey(key))
                pool[key] = new Queue<GameObject>();

            pool[key].Enqueue(obj);
        }

        // 남은 개수 확인용 (디버깅 또는 인게임 상태 체크)
        public int GetRemainingCount(string key)
        {
            return pool.ContainsKey(key) ? pool[key].Count : 0;
        }
    }
}