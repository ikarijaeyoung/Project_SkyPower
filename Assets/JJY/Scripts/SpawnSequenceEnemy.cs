using System.Collections;
using System.Collections.Generic;
using JYL;
using Unity.VisualScripting;
using UnityEngine;
public class SpawnSequenceEnemy : MonoBehaviour
{
    [SerializeField] private ObjectPool[] objectPools;
    private Dictionary<EnemyType, ObjectPool> poolDic;
    void Awake()
    {
        poolDic = new Dictionary<EnemyType, ObjectPool>();

        foreach (var pool in objectPools)
        {
            if (poolDic.ContainsKey(pool.enemyType))
            {
                // Debug.LogError($"중복된 EnemyType이 ObjectPool에 있습니다: {pool.enemyType}");
                continue;
            }
            Debug.Log($"집어넣으려는 풀 : {pool}, {pool.enemyType}");
            poolDic.TryAdd(pool.enemyType, pool); // pool 스크립트에 public EnemyType enemyType; 정의필요.
        }
    }

    public IEnumerator SpawnSequence(EnemySpawnInfo info)
    {
        for (int i = 0; i < info.enemyPrefab.Length; i++)
        {
            GameObject enemyObj = Instantiate(info.enemyPrefab[i], this.transform.position, Quaternion.Euler(0, 180f, 0));
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            EnemyType type = enemy.enemyData.enemyType;
            if (poolDic.TryGetValue(type, out ObjectPool pool))
            {
                enemy.Init(pool);
            }

            SpawnManager.enemyCount++;
            Debug.Log($"Total Enemies: {SpawnManager.enemyCount}");

            yield return new WaitForSeconds(info.spawnDelay);
        }
    }
}
