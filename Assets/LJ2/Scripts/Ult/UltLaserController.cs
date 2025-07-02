using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltLaserController : MonoBehaviour
{
    [SerializeField] private float increase = 0.2f;
    [SerializeField] private float maxSize = 5f;

    private Coroutine attackCoroutine;
    [SerializeField] private float setDelay;
    private YieldInstruction attackDelay;

    [SerializeField] private LayerMask enemy;

    private float size;

    private void Awake()
    {
        increase = Mathf.Clamp(increase, 0.1f, maxSize);
        attackDelay = new WaitForSeconds(setDelay);
        enemy = LayerMask.GetMask("Enemy");
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        size = 0;
    }

    private void LateUpdate()
    {
        if (size < maxSize)
        {
            size += increase;
        }
        transform.localScale = Vector3.one * size;
    }

    private void OnDisable()
    {
        attackCoroutine = null;
    }

    public void Attack(float damage)
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine(damage));
        }
        else
        {
            return;
        }
    }


    private IEnumerator AttackCoroutine(float damage)
    {
        Debug.Log("AttackCoroutine 시작");
        while (true)
        {
            Collider[] hits = Physics.OverlapCapsule(Vector3.zero, Vector3.one * size, size, enemy);
            foreach (Collider c in hits)
            {
                Debug.Log($"TakeDamage {damage}시도");
                if (c.gameObject.GetComponent<Enemy>())
                {
                    Enemy enemy = c.gameObject.GetComponent<Enemy>();
                    enemy.TakeDamage((int)damage);
                }
            }
            yield return attackDelay;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(transform.GetComponent<MeshFilter>().sharedMesh, transform.position, transform.rotation, Vector3.one * size);
    }
}
