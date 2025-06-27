using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltMapAttack : MonoBehaviour
{
    [SerializeField] LayerMask bullet;
    private Coroutine coroutine;
    [SerializeField] int damage;

    private void OnEnable()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2f, Quaternion.identity);

        foreach (Collider c in hits)
        {
            Debug.Log("TakeDamage ½Ãµµ");
            if (c.gameObject.GetComponent<Enemy>()) 
            { 
                Enemy enemy = c.gameObject.GetComponent<Enemy>();
                enemy.TakeDamage(damage);

            }
        }
    }

    //private void OnDisable()
    //{
    //    coroutine = null;
    //}

    //private IEnumerator AllAttackCoroutine(int damage)
    // {
        
        //hits = null;
    //    yield break;
    //}

    
}
