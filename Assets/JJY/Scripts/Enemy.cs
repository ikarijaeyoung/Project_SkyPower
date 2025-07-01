using System;
using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Random = System.Random;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public EnemyDropItemData dropItem;
    [SerializeField] private int currentHP;
    public bool isFiring; // 몬스터는 맵 밖에서 소환되어, 특정 위치로 애니메이터를 통해 이동된다. 이동중에는 공격을 하면 안되기 때문에 공격은 isMoving이 false일 때만 기능한다.
    public Transform[] firePoints;
    public static event Action<Vector3> OnEnemyDied; // 죽었을 때 사용되는 이벤트
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
        currentHP = enemyData.maxHP;
        for (int i = 0; i < BulletPattern.Length; i++)
        {
            BulletPattern[i].SetPool(objectPool);
        }
        isFiring = true; // 테스트용
        StartCoroutine(ChangeFireMode());
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
        // TODO : Sprite 색 변경
    }
    private void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space)) // 테스트용
        // {
        //     TakeDamage(1);
        // }
    }
    // private void Fire()
    // {
    //     curFireCoroutine = StartCoroutine(BulletPattern[0].Shoot(firePoints, enemyData.bulletPrefab, bulletSpeed));
    // }
    private void Die()
    {
        // 여기도 GameManager에서 이벤트
        OnEnemyDied?.Invoke(transform.position);
        // GameManager에서는 죽었다는 이벤트를 받아서 => 아이템 드롭 => 아이템 먹으면 점수 증가
        StopCoroutine(curFireCoroutine);
        // 죽는 애니메이션 실행.
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
