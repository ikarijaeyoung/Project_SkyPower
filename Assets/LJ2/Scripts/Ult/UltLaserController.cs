using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltLaserController : MonoBehaviour
{
    
    [SerializeField] private float attackDelay;
    private float currentTime;
    private int attackDamage = 2;

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
                Debug.Log($"TakeDamage {enemyComponent.name} 시도");
                enemyComponent.TakeDamage(attackDamage);
                Debug.Log($"TakeDamage {attackDamage} 성공");
            }
        }
    }
    private void OnDisable()
    {
        currentTime = 0;
    }

    public void AttackDamage(float damage)
    {
        Debug.Log($"UltLaserController AttackDamage: {damage}");
        attackDamage = (int)damage;
        Debug.Log($"UltLaserController AttackDamage: {attackDamage}");
    }

}
