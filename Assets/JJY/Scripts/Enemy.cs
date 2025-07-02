using System;
using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    [SerializeField] private int currentHP; // TODO : Player의 공격력 * 1.5배
    public bool isFiring;
    public Transform[] firePoints;
    public static event Action<Vector3> OnEnemyDied;
    public BulletPatternData[] BulletPattern;
    private Coroutine curFireCoroutine;
    public ObjectPool objectPool;

    // Enemy의 특성대로 총알 속도와 발사 간격을 조절.
    public float bulletSpeed = 1f; //이거 왜 있지
    public float fireDelay = 1.5f;

    void Start()
    {
        Init();
    }
    void Init()
    {
        currentHP = enemyData.maxHP; // Player의 공격력 * 1.5배
        for (int i = 0; i < BulletPattern.Length; i++)
        {
            BulletPattern[i].SetPool(objectPool);
        }
        isFiring = true;
        StartCoroutine(ChangeFireMode());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) TakeDamage(1);
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
        // TODO : Sprite 색 변경
    }
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(1);
        }
    }
    // private void Fire()
    // {
    //     curFireCoroutine = StartCoroutine(BulletPattern[0].Shoot(firePoints, enemyData.bulletPrefab, bulletSpeed));
    // }
    private void Die()
    {
        SpawnManager.enemyCount--; // TODO : 임시로 EnemyItemManager를 사용함. StageManager로 옮길것.
        // 여기도 GameManager에서 이벤트
        
        OnEnemyDied?.Invoke(transform.position);
        // GameManager에서는 죽었다는 이벤트를 받아서 => 아이템 드롭 => 아이템 먹으면 점수 증가

        StopCoroutine(curFireCoroutine);
        // TODO : 죽는 애니메이션 실행.
        Destroy(gameObject);
    }
    IEnumerator ChangeFireMode()
    {
        while (isFiring)
        {
            Random random = new Random();
            int ranNum = random.Next(0, BulletPattern.Length);
            curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, enemyData.bulletPrefab, bulletSpeed));
            yield return new WaitForSeconds(fireDelay);
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
    }
}
