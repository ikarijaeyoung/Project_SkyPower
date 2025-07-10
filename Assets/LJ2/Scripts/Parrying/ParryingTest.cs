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

    [SerializeField] GameObject parryingEffectPrefab;

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
        Debug.Log("�и� ����");
        if (canParrying.Length > 0)
        {
            Debug.Log("���ǹ� ����");

            foreach (Collider c in canParrying)
            {
                c.gameObject.SetActive(false);
                Debug.Log("�ݺ��� ����");

            }
            parryCoroutine = StartCoroutine(InvincibleCoroutine());

        }
        else return;
        // �и��� �������� ��, �� �Ѿ��� ��Ȱ��ȭ�ϰ� �ڷ�ƾ�� �����մϴ�.
        //if (isReflect)
        //{
        //    foreach(Transform t in trasnforms)
        //    {
        //        Rigidbody rb = t.GetComponent<Rigidbody>();
        //        if (rb != null)
        //        {
        //            Vector3 reflectDirection = (t.position - transform.position).normalized;
        //            rb.AddForce(reflectDirection * 10f, ForceMode.Impulse);
        //            Debug.Log("�ݻ� �õ�");
        //        }
        //    }
        //}
    }

    private IEnumerator InvincibleCoroutine()
    {
        Debug.Log("�ڷ�ƾ ����");

        parryingEffectPrefab.SetActive(true);
        yield return coroutineDelay;
        parryingEffectPrefab.SetActive(false);
        parryCoroutine = null;
        yield break;

    }

    private IEnumerator EraseCoroutine()
    {
        yield return null;

        Collider[] hits = Physics.OverlapSphere(transform.position, destroyRadius, destroyLayer);

        Debug.Log("Erase �ڷ�ƾ ����");
        foreach (Collider c in hits)
        {
            Debug.Log("�ݺ��� ����");

            c.gameObject.SetActive(false); 

        }
        
        hits = null;
        parryCoroutine = null;
        Debug.Log("�ڷ�ƾ ����");

        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, parryingRadius);
    }
}
