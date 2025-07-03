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

    public void Reflect(BulletController target)  // 이상하게 돌아가는 중
    {
        reflect.x = -target.rig.velocity.x * 2;
        reflect.z = -target.rig.velocity.z * 2;
        target.rig.velocity = reflect;

        target.gameObject.SetActive(true);
    }

    //private void OnCollisionEnter(Collision collision)
    //{

    //    BulletController targetController = collision.gameObject.GetComponent<BulletController>();
    //    Reflect(targetController);

    //}

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
}
