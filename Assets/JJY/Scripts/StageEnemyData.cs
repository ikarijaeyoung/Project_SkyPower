using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/StageEnemyData")]
public class StageEnemyData : ScriptableObject
{
    public List<EnemySpawnInfo> sequence;
    public GameObject[] bossPrefabs;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject[] enemyPrefab;
    public float spawnDelay;
}