using System.Collections;
using System.Collections.Generic;
using JYL;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyData;

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

    public IEnumerator SpawnSequence(SequenceData sequence)
    {
        foreach (var info in sequence.enemiesInfo)
        {
            for (int i = 0; i < info.enemyPrefab.Length; i++)
            {
                GameObject enemyObj = Instantiate(info.enemyPrefab[i], info.spawnPos[i], Quaternion.identity);
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                EnemyType type = enemy.enemyData.enemyType;
                if (poolDic.TryGetValue(type, out ObjectPool pool))
                {
                    Debug.Log($"{type}, {pool}");
                    enemy.objectPool = pool;
                    for (int j = 0; j < enemy.BulletPattern.Length; j++)
                    {
                        enemy.BulletPattern[j].SetPool(pool);
                    }
                }

                SpawnManager.enemyCount++;
                Debug.Log($"Total Enemies: {SpawnManager.enemyCount}");

                yield return new WaitForSeconds(info.spawnDelay);
            }
        }
    }
}
// public class EnemySpawner : MonoBehaviour
// {
//     [Header("Spawner Settings")]
//     [SerializeField] private Vector3[] spawnPoint; // 스폰은 이 스포너의 위치에서 스폰되지만, 이 스포너의 위치를 옮겨줄 필요가 있음.
//     [SerializeField] private GameObject[] enemyPrefabs;
//     [SerializeField] private float spawnDelay = 2f; // 적 소환 간격
//     public ObjectPool objectPool;
//     private void Start()
//     {
//         StartCoroutine(SpawnEnemy());
//     }
//     public IEnumerator SpawnEnemy()
//     {
//         for (int i = 0; i < enemyPrefabs.Length; i++)
//         {
//             MoveSpawnPoint(spawnPoint[i]);
//             GameObject enemyobj = Instantiate(enemyPrefabs[i], transform.position, transform.rotation);

//             SpawnManager.enemyCount++;
//             Debug.Log($"Enemy Spawner Enemy Count : {SpawnManager.enemyCount}");

//             enemyobj.transform.position = transform.position;
//             Enemy enemy = enemyobj.GetComponent<Enemy>();
//             enemy.objectPool = objectPool;

//             yield return new WaitForSeconds(spawnDelay);
//         }

//         // 다 소환하면 그 이후엔 어떻게 할 것인가?
//         // 앞 스테이지 몬스터가 다 죽지 않았으면, 스폰 정지. => 몬스터가 다 죽으면 스폰 이어하기.
//         //
//         // if (enemyCount < enemyPrefabs.Length)
//         // {
//         //     for (int i = 0; i < enemyPrefabs.Length; i++)
//         //     {
//         //         GameObject enemy = Instantiate(enemyPrefabs[i], transform.position, Quaternion.identity);
//         //         enemy.transform.SetParent(transform);
//         //         yield return new WaitForSeconds(spawnDelay);
//         //
//         //         enemyCount++; => Game Clear 조건과 연결
//         //     }
//         // }
//         // 다 소환하고, 현재 맵에 적이 없으면(또는 처치 시 enemyCount--;) enemyCount = 0;으로 초기화
//         //
//         // or
//         //
//         // 이 스포너를 여러개 만들기.

//         void MoveSpawnPoint(Vector3 spawnPosition)
//         {
//             transform.position = spawnPosition;
//         }
//     }
// }
