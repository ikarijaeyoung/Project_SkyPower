using KYG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltTest : MonoBehaviour
{
    public GameObject shield;

    public GameObject ultBullet;
    public GameObject ultLaser;
    public GameObject ultAll;
    public LayerMask enemyBullet;

    public Coroutine ultRoutine;
    public float setUltDelay;
    public YieldInstruction ultDelay;
    public bool canUseUlt;

    public void Awake()
    {
        ultDelay = new WaitForSeconds(setUltDelay);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Ult();
        }
    }

    public void Ult()
    {
        if (ultRoutine == null)
        {
            ultRoutine = StartCoroutine(EraseCoroutine());
        }
        else
        {
            return;
        }
    }
    

    private IEnumerator ShieldCotoutine()
    { 
        shield.SetActive(true);
        Debug.Log("Shield Active");
        yield return ultDelay;

        shield.SetActive(false);
        Debug.Log("Shield Off");
        ultRoutine = null;
        yield break;
        
    }

    private IEnumerator LaserCotoutine()
    {
        ultLaser.SetActive(true);
        Debug.Log("Laser Active");
        yield return ultDelay;

        ultLaser.SetActive(false);
        Debug.Log("Laser Off");
        ultRoutine = null;
        yield break;

    }
    /*
    private IEnumerator MapAttack()
    {
        ultAll.SetActive(true);
        Debug.Log("MapAttack Active");
        yield return ultDelay;

        ultAll.SetActive(false);
        Debug.Log("MapAttack Off");
        ultRoutine = null;
        yield break;
    }
    */

    private IEnumerator EraseCoroutine()
    {
        yield return null;
        Collider[] hits = Physics.OverlapBox(ultAll.transform.position, ultAll.transform.localScale / 2f, Quaternion.identity, enemyBullet);
        Debug.Log("Erase 코루틴 진입");
        foreach (Collider c in hits)
        {
            Debug.Log("반복문 진입");

            c.gameObject.SetActive(false);


        }
        hits = null;
        ultRoutine = null;
        yield break;
    }
}
