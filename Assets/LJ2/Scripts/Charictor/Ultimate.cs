using JYL;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ultimate : MonoBehaviour
{
    public Coroutine ultRoutine;
    [Range(0.5f, 5f)] public float setUltDelay = 4f;
    public YieldInstruction ultDelay;

    public PlayerController playerController;
    [Range(0.1f, 5)][SerializeField] float bulletReturnTimer = 5f;
    [Range(0.1f, 30)][SerializeField] float bigBulletSpeed = 15f;
    [Range(0.1f, 30)][SerializeField] float manyBulletSpeed = 30f;
    [Range(0.1f, 3)][SerializeField] float ultBulletTime = 50f;

    [SerializeField] float bulletUpgradeTime = 5f;

    public LayerMask enemyBullet;

    public GameObject ultLaser;
    public UltLaserController ultLaserController;

    public GameObject shield;
    public UltShieldController ultShieldController;

    public GameObject ultAll;
    public UltMapAttack ultAllController;

    public GameObject ultFire; // Fire 프리팹 연결 (추가된 부분)
    public UltLaserController ultFireController; // Fire 컨트롤러 (추가된 부분)

    public int defense = 1;

    private int fireCounter;

    public void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        enemyBullet = LayerMask.GetMask("EnemyBullet");

        ultLaser = transform.Find("Effect_28").gameObject;
        shield = transform.Find("Effect_07").gameObject;
        ultAll = transform.Find("Effect_31").gameObject;
        ultFire = transform.Find("Effect_19").gameObject;

        ultLaserController = ultLaser.GetComponentInChildren<UltLaserController>();
        ultShieldController = shield.GetComponentInChildren<UltShieldController>();
        ultAllController = ultAll.GetComponent<UltMapAttack>();
        ultFireController = ultFire.GetComponentInChildren<UltLaserController>();

    }

    private void OnEnable()
    {
        ultDelay = new WaitForSeconds(setUltDelay);
    }

    public void Laser(float damage)
    {
        Debug.Log($"Laser Damage: {damage}");
        if (ultRoutine == null)
        {
            ultLaserController.AttackDamage(damage);
            ultRoutine = StartCoroutine(LaserCoroutine());
        }
        else
        {
            return;
        }
    }
    private IEnumerator LaserCoroutine()
    {
        Debug.Log($"{ultLaser.name}");
        ultLaser.SetActive(true);
        PlayerController.canAttack = false; // 공격 불가 상태로 변경
        Debug.Log("Laser Active");
        Debug.Log($"{setUltDelay}");
        yield return ultDelay;

        ultLaser.SetActive(false);
        PlayerController.canAttack = true; // 공격 가능 상태로 변경
        Debug.Log("Laser Off");
        ultRoutine = null;
        yield break;
    }

    public void Fire(float damage)
    {
        if (ultRoutine == null)
        {
            ultFireController.AttackDamage(damage);
            ultRoutine = StartCoroutine(FireCoroutine());
        }
        else
        {
            return;
        }
    }
    private IEnumerator FireCoroutine()
    {
        ultFire.SetActive(true);
        PlayerController.canAttack = false; // 공격 불가 상태로 변경
        Debug.Log("Laser Active");
        yield return ultDelay;

        ultFire.SetActive(false);
        PlayerController.canAttack = true; // 공격 가능 상태로 변경
        Debug.Log("Laser Off");
        ultRoutine = null;
        yield break;
    }

    public int Shield(float damage)
    {
        if (ultRoutine == null)
        {
            ultShieldController.AttackDamage(damage);
            ultRoutine = StartCoroutine(ShieldCoroutine());
        }
        return defense;
    }
    private IEnumerator ShieldCoroutine()
    {
        shield.SetActive(true);
        Debug.Log("Shield Active");
        yield return ultDelay;

        shield.SetActive(false);
        Debug.Log("Shield Off");
        ultRoutine = null;
        yield break;
    }

    public void AllAttack(float damage)
    {
        if (ultRoutine == null)
        {
            ultAllController.AttackDamage(damage);
            ultRoutine = StartCoroutine(EraseCoroutine());
        }
        else
        {
            return;
        }
    }

    private IEnumerator EraseCoroutine()
    {
        Collider[] hits = Physics.OverlapBox(ultAll.transform.position, ultAll.transform.localScale / 2f, Quaternion.identity, enemyBullet);

        foreach (Collider c in hits)
        {
            c.gameObject.SetActive(false);
        }
        ultAll.SetActive(true);

        yield return ultDelay;
        ultAll.SetActive(false);
        hits = null;
        ultRoutine = null;
        Debug.Log("코루틴 종료");

        yield break;
    }

    // 궁극기 탄막 1회 + 다단히트
    public void BigBullet(float damage)
    {
        playerController.poolIndex = 1;
        if (ultRoutine != null)
        {
            fireCounter = 1;
            ultRoutine = StartCoroutine(UltFireCoroutine(damage, bigBulletSpeed));
        }
        else
        {
            return;
        }
        playerController.poolIndex = 0; // 다시 기본 총알로 변경
    }

    public void ManyBullets(float damage)
    {
        playerController.poolIndex = 1;
        if (ultRoutine != null)
        {
            fireCounter = 5;
            ultRoutine = StartCoroutine(UltFireCoroutine(damage, manyBulletSpeed));
        }
        else
        {
            return;
        }
        playerController.poolIndex = 0;
    }


    public IEnumerator UltFireCoroutine(float damage, float bulletSpeed)
    {
        PlayerController.canAttack = false; // 공격 불가 상태로 변경
        while (fireCounter > 0)
        {
            fireCounter--;
            BulletPrefabController bulletPrefab = playerController.curBulletPool.ObjectOut() as BulletPrefabController;
            bulletPrefab.transform.position = playerController.muzzlePoint.position;
            bulletPrefab.ReturnToPool(bulletReturnTimer);
            foreach (BulletInfo info in bulletPrefab.bulletInfo)
            {
                if (info.rig == null)
                {
                    continue;
                }
                info.trans.gameObject.SetActive(true);
                info.trans.localPosition = info.originPos;
                info.trans.rotation = Quaternion.Euler(0, 3 * fireCounter , 0);
                info.rig.velocity = Vector3.zero;
                
                info.bulletController.attackPower = (int)damage;
                info.bulletController.canDeactive = false;
                
                info.rig.AddForce(bulletSpeed * info.trans.forward, ForceMode.Impulse); // 이 부분을 커스텀하면 됨
            }
            yield return new WaitForSeconds(ultBulletTime * 0.1f);
        }
        
        StopCoroutine(ultRoutine);
        PlayerController.canAttack = true; // 공격 가능 상태로 변경
        ultRoutine = null;
    }

    // 탄막 변경 + 데미지 증가
    public void BulletUpgrade()
    {
        if(ultRoutine == null)
        {
            ultRoutine = StartCoroutine(UpgradeRoutine());
        }
        else
        {
            return;
        }
    }

    public IEnumerator UpgradeRoutine()
    {
        playerController.poolIndex = 1;
        Debug.Log("Upgrade Bullet Shot");
        yield return ultDelay;

        playerController.poolIndex = 0;
        Debug.Log("Normal Bullet Shot");
        ultRoutine = null;
        yield break;
    }
}
