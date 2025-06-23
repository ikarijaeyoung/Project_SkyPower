using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [SerializeField] public float spawnInterval;

    void SpawnEnemy()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 6f, 0f);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f , spawnInterval);
    }
}
