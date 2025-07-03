using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//StageData에 아래 추가 => StageData에 추가하는게 적절한가? , EnemyData : ScriptableObject ?
[CreateAssetMenu(menuName = "YSK/StageEnemyData")]
public class StageEnemyData : ScriptableObject
{
    public List<SequenceData> sequence;
    public GameObject bossPrefab;
    public Vector3 bossSpawnPos;
}

[System.Serializable]
public class SequenceData
{
    public List<EnemySpawnInfo> enemiesInfo;
}

[System.Serializable]
public class EnemySpawnInfo
{
    // public EnemyType enemyType; // 각 타입마다 ObjectPool다름
    public GameObject[] enemyPrefab;
    public Vector3[] spawnPos;
    public float spawnDelay;
}
// public enum EnemyType
// {
//     Normal,
//     Elite,
//     Boss
// }