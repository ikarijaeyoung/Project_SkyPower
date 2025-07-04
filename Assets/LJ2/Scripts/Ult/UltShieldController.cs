using JYL;
using KYG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltShieldController : MonoBehaviour
{
    [SerializeField] public bool isReflect = true;
    private Vector3 reflect;

    [SerializeField] private float attackDelay = 0.5f;
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
                Debug.Log($"TakeDamage {enemyComponent.name} ½Ãµµ");
                enemyComponent.TakeDamage(attackDamage);
            }
        }
    }
}
