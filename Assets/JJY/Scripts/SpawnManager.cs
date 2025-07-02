using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using static EnemyData;

public class SpawnManager : MonoBehaviour
{
    public List<StageData> allStages;
    private int curStageLevel = 0; // 스테이지 선택 화면에서 연동.
    private int curSequenceLevel = 0;
    static public int enemyCount = 0;
    [SerializeField] private SpawnSequenceEnemy spawnSequenceEnemy;
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
            poolDic.TryAdd(pool.enemyType, pool); // pool 스크립트에 public EnemyType enemyType; 정의필요.
        }
    }

    private void Start()
    {
        StartCoroutine(PlayStage());
    }

    private IEnumerator PlayStage()
    {
        while (curStageLevel < allStages.Count)
        {
            StageData currentStage = allStages[curStageLevel];

            for (curSequenceLevel = 0; curSequenceLevel < currentStage.sequence.Count; curSequenceLevel++)
            {
                SequenceData sequence = currentStage.sequence[curSequenceLevel];

                yield return StartCoroutine(spawnSequenceEnemy.SpawnSequence(sequence));

                // 적이 다 죽을 때까지 대기
                while (enemyCount > 0)
                    yield return null;

                Debug.Log($"Sequence {curSequenceLevel} cleared!");
            }

            // 보스 1마리 스폰
            Debug.Log("Boss appears!");
            GameObject bossobj = Instantiate(currentStage.bossPrefab, currentStage.bossSpawnPos, Quaternion.identity);
            Enemy enemy = bossobj.GetComponent<Enemy>();
            EnemyType type = enemy.enemyData.enemyType;
            if (poolDic.TryGetValue(type, out ObjectPool pool))
            {
                enemy.objectPool = pool;
                for (int i = 0; i < enemy.BulletPattern.Length; i++)
                {
                    enemy.BulletPattern[i].SetPool(pool);
                }
            }
            enemyCount++;
            Debug.Log($"Total Enemies: {enemyCount}");


            while (enemyCount > 0)
                yield return null;

            Debug.Log($"Stage {curStageLevel} cleared!");
            curStageLevel++;
        }

        Debug.Log("All Stages Complete!");
    }
}
