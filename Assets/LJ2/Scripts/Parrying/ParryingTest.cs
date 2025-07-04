using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryingTest : MonoBehaviour
{
    [SerializeField] int shield;
    [SerializeField] float invincibleTime;
    [SerializeField] float parryingRadius;
    [SerializeField] float destroyRadius;
    [SerializeField] LayerMask enemyBullet;
    [SerializeField] LayerMask destroyLayer;
    [SerializeField] SphereCollider characterCollider;
    private Coroutine parryCoroutine;

    public bool isReflect = true;

    public YieldInstruction coroutineDelay;

    private void Awake()
    {
        coroutineDelay = new WaitForSeconds(invincibleTime);
        characterCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Parrying();
        }
    }

    public void Parrying()
    {
        Collider[] canParrying = Physics.OverlapSphere(transform.position, parryingRadius, enemyBullet);
        Transform[] trasnforms = new Transform[canParrying.Length];
        Debug.Log("패링 진입");
        if (canParrying.Length > 0)
        {
            Debug.Log("조건문 진입");

            foreach (Collider c in canParrying)
            {
                c.gameObject.SetActive(false);
                Debug.Log("반복문 진입");

            }
            parryCoroutine = StartCoroutine(EraseCoroutine());

        }
        else return;

        if (isReflect)
        {
            foreach(Transform t in trasnforms)
            {
                Rigidbody rb = t.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 reflectDirection = (t.position - transform.position).normalized;
                    rb.AddForce(reflectDirection * 10f, ForceMode.Impulse);
                    Debug.Log("반사 시도");
                }
            }
        }
    }
    
    private IEnumerator InvincibleCoroutine()
    {
        Debug.Log("코루틴 진입");

        characterCollider.enabled = false;
        yield return coroutineDelay;
        characterCollider.enabled = true;
        parryCoroutine = null;
        yield break;

    }

    private IEnumerator EraseCoroutine()
    {
        yield return null;

        Collider[] hits = Physics.OverlapSphere(transform.position, destroyRadius, destroyLayer);

        Debug.Log("Erase 코루틴 진입");
        foreach (Collider c in hits)
        {
            Debug.Log("반복문 진입");

            c.gameObject.SetActive(false); 

        }
        
        hits = null;
        parryCoroutine = null;
        Debug.Log("코루틴 종료");

        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, destroyRadius);
    }
}
