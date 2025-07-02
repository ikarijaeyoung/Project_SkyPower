using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parrying : MonoBehaviour
{
    [SerializeField] bool isParrying = false;
    [SerializeField] float parryRadius = 1.1f;
    [SerializeField] LayerMask enemyBullet;
    [SerializeField] Collider characterCollider;

    private Coroutine parryCoroutine;
    public YieldInstruction coroutineDelay;
    [SerializeField] float invincibleTime = 1;

    [SerializeField] int shield = 3;

    public void Awake()
    {
        characterCollider = GetComponent<SphereCollider>();
        coroutineDelay = new WaitForSeconds(invincibleTime);
        enemyBullet = LayerMask.GetMask("EnemyBullet");
    }

    public void Parry()
    {
        Collider[] canParry = Physics.OverlapSphere(transform.position, parryRadius, enemyBullet);
        if (canParry.Length > 0)
        {
            isParrying = true;
            
            foreach (Collider c in canParry)
            {
                c.gameObject.SetActive(false); // Deactivate enemy bullets
            }
        }
        else
        {
            isParrying = false; // No bullets to parry
        }
    }

    public void Invicible()
    {   
        if (isParrying)
        {
           parryCoroutine = StartCoroutine(InvincibleCoroutine());
        }
        isParrying = false; // Reset parrying state
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

    public int Shield()
    {
        int getShield = 0; // Default shield value
        if (isParrying)
        {
            getShield = shield;
        }
        isParrying = false; // Reset parrying state after getting shield
        return getShield;
    }

    // 반사된 총알을 적에게 발사하는 기능은 현재 구현되어 있지 않습니다.    
}
