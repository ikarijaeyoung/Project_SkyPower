using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltMapAttack : MonoBehaviour
{
    [SerializeField] int attackDamage;

    private void OnEnable()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2f, Quaternion.identity);

        foreach (Collider c in hits)
        {
            Debug.Log("TakeDamage ½Ãµµ");
            if (c.gameObject.GetComponent<Enemy>()) 
            { 
                Enemy enemy = c.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(attackDamage);
            }
        }
    }
    public void AttackDamage(float damage)
    {
        attackDamage = (int)damage;
    }
}
