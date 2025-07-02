using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/StageEnemyData")]
public class StageEnemyData : ScriptableObject
{
    public List<EnemySpawnInfo> sequence;
    public GameObject bossPrefab;
    public Vector3 bossSpawnPos;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject[] enemyPrefab;
    public Vector3[] spawnPos;
    public float spawnDelay;
}