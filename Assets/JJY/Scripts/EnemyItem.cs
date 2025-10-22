using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KYG_skyPower;

public class EnemyItem : MonoBehaviour
{
    // public EnemyDropItemData dropItem;

    // 자력 효과
    private Transform player;
    public float magneticRange = 3f;
    public float magneticSpeed = 5f;
    [SerializeField] private int addScore = 10;
    [SerializeField] private float destoryDelay = 10f;
    [SerializeField] private float checkInterval = 0.1f;

    private float sqrPickupRadius;
    private float checkTimer;

    void Start()
    {
        sqrPickupRadius = magneticRange * magneticRange;
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
        }
        StartCoroutine(AutoDestroy());
    }
    void FixedUpdate()
    {
        if (player == null)
        {
            Debug.Log("EnemyItem : Player가 할당되지 않음.");
            return;
        }

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;

            Vector3 toPlayer = player.position - transform.position;
            float sqrDist = toPlayer.sqrMagnitude;

            if (sqrDist < sqrPickupRadius)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    player.position,
                    magneticSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
    void Collect()
    {
        ScoreManager.Instance.AddScore(addScore);
        Destroy(gameObject);
    }
    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(destoryDelay);
        Destroy(gameObject);
    }
}


