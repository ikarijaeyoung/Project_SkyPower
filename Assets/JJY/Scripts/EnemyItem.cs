using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItem : MonoBehaviour
{
    public EnemyDropItemData dropItem;
    private Transform player;
    public float magneticRange = 3f;
    public float magneticSpeed = 5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
        }
    }
    void Update()
    {
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= magneticRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, magneticSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        Debug.Log("Enemy item collected.");
        // TODO : 점수 증가 로직 추가 가능
        Destroy(gameObject);
    }
}
