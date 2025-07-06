using JYL;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public static event Action<Vector3> OnEnemyDied;

    [Header("Enemy State")]
    public EnemyData enemyData;
    [SerializeField] private int currentHP;
    public bool autoFire;

    [Header("Enemy Shot Info")]
    public Transform[] firePoints;
    public BulletPatternData[] BulletPattern;
    private Coroutine curFireCoroutine;
    public ObjectPool curObjectPool;
    public float bulletSpeed = 1f;
    public float fireDelay = 1.5f;

    [Header("Hit Animation")]
    private Renderer modelRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;
    [SerializeField] private Color flashColor = Color.white; // 피격 시 변경될 색상
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Animator")]
    private Animator animator;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab;
    public float destroyDelay;
    void Awake()
    {
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            originalColor = modelRenderer.material.color;
        }
        animator = GetComponent<Animator>();
    }
    public void Init(ObjectPool objectPool)
    {
        curObjectPool = objectPool;
        curObjectPool.CreatePool();
        currentHP = enemyData.maxHP;
        // autoFire = true;
        // if (autoFire) StartCoroutine(ChangeFireMode());
    }
    public void TakeDamage(int damage)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashEffectCoroutine());

        currentHP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHP}");
        if (currentHP <= 0) Die();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AnimationFire();
        }
    }
    private void Die()
    {
        SpawnManager.enemyCount--;

        OnEnemyDied?.Invoke(transform.position);

        if (curFireCoroutine != null)
        {
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
        if (deathEffectPrefab != null)
        {
            GameObject instance = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

            ParticleSystem ps = instance.GetComponent<ParticleSystem>();
            if (ps == null && instance.transform.childCount > 0) // 자식 GameObject에 파티클 시스템이 있을 경우
            {
                ps = instance.transform.GetChild(0)?.GetComponent<ParticleSystem>();
            }
            if (ps != null)
            {
                Destroy(instance, ps.main.duration);
                destroyDelay = ps.main.duration;
            }
        }
        if (modelRenderer != null)
        {
            modelRenderer.enabled = false;
        }
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        Rigidbody enemyRb = GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            enemyRb.velocity = Vector3.zero;
            enemyRb.isKinematic = true;
        }
        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    IEnumerator ChangeFireMode()
    {
        while (autoFire)
        {
            int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
            curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool, enemyData.attackPower));
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
    public void AnimationFire()
    {
        Debug.Log("지금 공격함");
        int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
        curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool, enemyData.attackPower));
    }
}
