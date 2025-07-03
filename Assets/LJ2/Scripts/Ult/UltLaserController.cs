using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltLaserController : MonoBehaviour
{
    
    [SerializeField] private float attackDelay;
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
    private void OnDisable()
    {
        currentTime = 0;
    }

    public void AttackDamage(float damage)
    {
        attackDamage = (int)damage;
    }

}
