using JYL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltBulletController : PooledObject
{
    [SerializeField] private float attackDelay;
    [SerializeField] public Transform muzzle;
    private float currentTime;
    private int attackDamage;
    
    private Rigidbody rb;
    [SerializeField] private float bulletSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        currentTime = 0;
        rb.velocity = transform.forward * bulletSpeed; // Set initial velocity
        transform.position = muzzle.position;
    }

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
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentTime >= attackDelay)
        {
            Enemy enemyComponent = other.gameObject.GetComponentInParent<Enemy>();
            if (enemyComponent != null)
            {
                Debug.Log($"TakeDamage {enemyComponent.name} ½Ãµµ");
                enemyComponent.TakeDamage(attackDamage);
            }
        }
    }
    private void OnDisable()
    {
        currentTime = 0;
    }

    public void AttackDamage(float damage)
    {
        attackDamage = (int)damage;
    }

}
