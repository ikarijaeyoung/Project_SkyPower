using System;
using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public static event Action<Vector3> OnEnemyDied;

    [Header("Enemy State")]
    public EnemyData enemyData;
    [SerializeField] private int currentHP; // TODO : Player의 공격력 * 1.5배
    public bool isFiring;

    [Header("Enemy Shot Info")]
    public Transform[] firePoints;
    public BulletPatternData[] BulletPattern;
    private Coroutine curFireCoroutine;
    public ObjectPool objectPool;
    public float bulletSpeed = 1f; //이거 왜 있지
    public float fireDelay = 1.5f;

    [Header("Hit Animation")]
    private Renderer modelRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    [SerializeField] private Color flashColor = Color.white; // 피격 시 변경될 색상
    [SerializeField] private float flashDuration = 0.1f;
    
    [Header("Animator")]
    private Animator animator;

    void Awake()
    {
        Init();
    }
    void Init()
    {
        currentHP = enemyData.maxHP; // Player의 공격력 * 1.5배
        isFiring = true;
        StartCoroutine(ChangeFireMode());
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            originalColor = modelRenderer.material.color;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) TakeDamage(1);
    }
    public void TakeDamage(int damage)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashEffectCoroutine());

        currentHP -= damage;
        if (currentHP <= 0) Die();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(1);
        }
    }
    private void Die()
    {
        SpawnManager.enemyCount--;

        OnEnemyDied?.Invoke(transform.position);

        StopCoroutine(curFireCoroutine);
        // TODO : 죽는 애니메이션 실행.
        Destroy(gameObject);
    }
    IEnumerator ChangeFireMode()
    {
        while (isFiring)
        {
            int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
            curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, enemyData.bulletPrefab, bulletSpeed, objectPool));
            yield return new WaitForSeconds(fireDelay);
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
    }
    private IEnumerator FlashEffectCoroutine()
    {
        modelRenderer.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        modelRenderer.material.color = originalColor;

        flashCoroutine = null;
    }
}
