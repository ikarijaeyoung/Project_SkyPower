using JJY;
using JYL;
using KYG;
using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltShieldController : MonoBehaviour
{
    [SerializeField] public bool isReflect = true;
    private Vector3 reflect;
    public float size = 3f;
    [Range(0.1f, 1)][SerializeField] private float attackDelay = 0.5f;
    private float currentTime;
    private int attackDamage;

    private void Update()
    {
        if (currentTime >= attackDelay)
        {
            currentTime = 0; // Reset currentTime after attack
        }
    }

    private void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;

        Collider[] hits = Physics.OverlapSphere(transform.position, size, LayerMask.GetMask("EnemyBullet"));
        foreach (Collider c in hits)
        {
            Debug.Log($"{c.gameObject.name} 제거 시도");
            c.gameObject.SetActive(false);
        }
        hits = null;
    }
    public void AttackDamage(float damage)
    {
        attackDamage = (int)damage;
    }


    private void OnTriggerStay(Collider other)
    {
        if (currentTime >= attackDelay)
        {
            Enemy enemyComponent = other.gameObject.GetComponentInParent<Enemy>();
            if (enemyComponent != null) // Fix for CS0472
            {
                Debug.Log($"TakeDamage {enemyComponent.name} 시도");
                enemyComponent.TakeDamage(attackDamage);
            }
            

        }
    }

    private void OnEnable()
    {
        Debug.Log("UltShieldController OnEnable 호출");
        AudioManager.Instance.PlaySFX("Shield_01");

    }

    private void OnDisable()
    {
        currentTime = 0;
        Debug.Log("UltShieldController OnDisable 호출");
    }
}
