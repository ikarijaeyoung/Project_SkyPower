using JJY_Test;
using JYL;
using KYG_skyPower;
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
    private bool isDead =false;

    [Header("Enemy Shot Info")]
    public Transform[] firePoints;
    public BulletPatternData[] BulletPattern;
    private Coroutine curFireCoroutine;
    public ObjectPool curObjectPool;
    public float bulletSpeed = 1f;
    // public float fireDelay = 1.5f;

    [Header("Hit Animation")]
    private Renderer modelRenderer;
    private MaterialPropertyBlock mpb;
    private Color originalColor;
    private Coroutine flashCoroutine;
    [SerializeField] private Color flashColor = Color.white; // 피격 시 변경될 색상
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Animator")]
    private Animator animator;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; //이거 게임 오브젝트 맞음?
    public float destroyDelay;
    void Awake()
    {
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            mpb = new MaterialPropertyBlock();
            modelRenderer.GetPropertyBlock(mpb);

            if (modelRenderer.sharedMaterial.HasProperty("_BaseColor"))
                originalColor = modelRenderer.sharedMaterial.GetColor("_BaseColor");
            else
                originalColor = Color.white;
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
        AudioManager.Instance.PlaySFX("Hit_Enemy");
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashEffectCoroutine());

        currentHP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHP}");
        if (currentHP <= 0&&!isDead)
        {
            isDead = true;
            AudioManager.Instance.PlaySFX("Death_Enemy");
            Die();
            
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(599);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            AnimationFire();
        }
    }
    public void DestoryAfterPathing()
    {
        // 여긴 죽음 이후에 폭발 이팩트, 아이템을 소환하지 않음. => animatoin에서 Path이후에 사라질 용도로 사용.
        Destroy(gameObject);
        SpawnManager.enemyCount--;

        // Stage만드시는 분들을 위한 TestSpawner전용.
        //TestSpawneManager.enemyCount--;
        //Debug.Log($"Total Enemies : {TestSpawneManager.enemyCount}");
    }
    private void Die()
    {
        SpawnManager.enemyCount--;
        AudioManager.Instance.PlaySFX("Death_Enemy");

        // Stage만드시는 분들을 위한 TestSpawner전용.
        //TestSpawneManager.enemyCount--;
        //Debug.Log($"Total Enemies : {TestSpawneManager.enemyCount}");
        OnEnemyDied?.Invoke(transform.position);

        if (curFireCoroutine != null)
        {
            StopCoroutine(curFireCoroutine);
            curFireCoroutine = null;
        }
        if (deathEffectPrefab != null)
        {
            // deathEffectPrefab은 프리팹이므로, Instantiate할 때 위치와 회전을 명시적으로 지정해야 함
            GameObject instance = Instantiate(
                deathEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            instance.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //// instance가 null이 아니고 자식에 파티클 시스템이 있을 경우
            //if (instance == null && instance.transform.childCount > 0)
            //{
            //    instance = instance.transform.GetComponent<ParticleSystem>();
            //}
            if (instance != null)
            {
                Destroy(instance.gameObject, 1.5f);
                //destroyDelay = instance.main.duration;
            }
        }
        Destroy(gameObject);
        
        //if (modelRenderer != null)
        //{
        //    modelRenderer.enabled = false;
        //}
        //Collider enemyCollider = GetComponentInChildren<Collider>();
        //if (enemyCollider != null)
        //{
        //    enemyCollider.enabled = false;
        //}
        //Rigidbody enemyRb = GetComponent<Rigidbody>();
        //if (enemyRb != null)
        //{
        //    enemyRb.velocity = Vector3.zero;
        //    enemyRb.isKinematic = true;
        //}
        //StartCoroutine(DestroyAfterDelay(destroyDelay));
    }
    // private IEnumerator DestroyAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     Destroy(gameObject);
    // }
    // IEnumerator ChangeFireMode()
    // {
    //     while (autoFire)
    //     {
    //         int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
    //         curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool, enemyData.attackPower));
    //         yield return new WaitForSeconds(fireDelay);
    //         StopCoroutine(curFireCoroutine);
    //         curFireCoroutine = null;
    //     }
    // }
    private IEnumerator FlashEffectCoroutine()
    {
        if (mpb == null) yield break;

        mpb.SetColor("_BaseColor", flashColor);
        modelRenderer.SetPropertyBlock(mpb);
        Debug.Log("반짝");
        yield return new WaitForSeconds(flashDuration);
        Debug.Log("반짝");
        mpb.SetColor("_BaseColor", originalColor);
        modelRenderer.SetPropertyBlock(mpb);

        flashCoroutine = null;
    }
    public void AnimationFire()
    {
        Debug.Log("지금 공격함");
        int ranNum = UnityEngine.Random.Range(0, BulletPattern.Length);
        curFireCoroutine = StartCoroutine(BulletPattern[ranNum].Shoot(firePoints, bulletSpeed, curObjectPool, enemyData.attackPower));
    }
}
