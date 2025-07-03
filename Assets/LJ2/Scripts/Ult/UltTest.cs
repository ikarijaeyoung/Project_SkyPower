using KYG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltTest : MonoBehaviour
{
    public GameObject shield;
    public UltShieldController ultShieldController;

    public GameObject ultLaser;
    public UltLaserController ultLaserController;

    public GameObject ultAll;
    public UltMapAttack ultAllController;
    public LayerMask enemyBullet;

    public GameObject ultBullet;
    public UltBulletController ultBulletController;

    public Coroutine ultRoutine;
    public float setUltDelay;
    public YieldInstruction ultDelay;
    public bool canUseUlt;

    public float ultDamage = 10f;

    public void Awake()
    {
        ultDelay = new WaitForSeconds(setUltDelay);
        enemyBullet = LayerMask.GetMask("EnemyBullet");
        ultLaserController = ultLaser.GetComponentInChildren<UltLaserController>();
        ultAllController = ultAll.GetComponent<UltMapAttack>();
        ultShieldController = shield.GetComponentInChildren<UltShieldController>();
        ultBulletController = ultBullet.GetComponentInChildren<UltBulletController>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Ult(ultDamage);
        }
    }

    public void Ult(float damage)
    {
        if (ultRoutine == null)
        {
            ultLaserController.AttackDamage(damage);
            ultAllController.AttackDamage(damage);
            ultShieldController.AttackDamage(damage);
            ultBulletController.AttackDamage(damage);
            ultRoutine = StartCoroutine(UltBulletCotoutine());
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

    private IEnumerator UltBulletCotoutine()
    {
        ultBullet.SetActive(true);
        Debug.Log("UltBullet Active");
        yield return ultDelay;

        ultBullet.SetActive(false);
        Debug.Log("UltBullet Off");
        ultRoutine = null;
        yield break;

    }

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
        ultAll.SetActive(true);
        ultAll.SetActive(false);
        hits = null;
        ultRoutine = null;
        Debug.Log("코루틴 종료");

        yield break;
    }
}
