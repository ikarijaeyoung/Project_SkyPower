using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPoolManager", menuName = "Managers/ObjectPoolManager")]
public class ObjectPoolManagerSO : ScriptableObject
{
    // 풀 저장소 (key: 프리팹 이름)
    private Dictionary<string, Queue<GameObject>> pool = new();

    // 오브젝트 풀 생성
    public void CreatePool(string key, GameObject prefab, int count)
    {
        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool[key].Enqueue(obj);
        }
    }

    // 풀에서 꺼내기
    public GameObject Spawn(string key, Vector3 pos, Quaternion rot)
    {
        if (!pool.ContainsKey(key) || pool[key].Count == 0) return null;

        GameObject obj = pool[key].Dequeue();
        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    // 풀에 반환
    public void Despawn(string key, GameObject obj)
    {
        obj.SetActive(false);
        pool[key].Enqueue(obj);
    }
}
