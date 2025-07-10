using JYL;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ultimate : MonoBehaviour
{
    public Coroutine ultRoutine;
    [Range(0.1f, 5f)] public float laserDelay = 4f; // �ּ� ������ �ð�
    [Range(0.1f, 5f)] public float fireDelay = 4f; // �ּ� ������ �ð�
    [Range(0.1f, 5f)] public float shieldDelay = 4f; // �ּ� ������ �ð�
    [Range(0.1f, 5f)] public float allDelay = 1f; // �ּ� ������ �ð�
    [Range(0.1f, 1f)] public float bigBulletDelay = 0.5f; // �ּ� ������ �ð�
    [Range(0.1f, 1f)] public float manyBulletDelay = 0.3f; // �ּ� ������ �ð�

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

    public GameObject ultFire; // Fire ������ ���� (�߰��� �κ�)
    public UltLaserController ultFireController; // Fire ��Ʈ�ѷ� (�߰��� �κ�)

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

        ultLaser.SetActive(true);
        PlayerController.canAttack = false; // ���� �Ұ� ���·� ����
        playerController.isUsingUlt = true; // �÷��̾� ���� ���·� ����
        Debug.Log("Laser Active");
        yield return new WaitForSeconds(laserDelay);

        ultLaser.SetActive(false);
        PlayerController.canAttack = true; // ���� ���� ���·� ����
        Debug.Log("Laser Off");
        playerController.isUsingUlt = false; // �÷��̾� ���� ���� ����
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
        PlayerController.canAttack = false; // ���� �Ұ� ���·� ����
        playerController.isUsingUlt = true; // �÷��̾� ���� ���·� ����
        Debug.Log("Laser Active");
        yield return new WaitForSeconds(laserDelay);

        ultFire.SetActive(false);
        PlayerController.canAttack = true; // ���� ���� ���·� ����
        playerController.isUsingUlt = false; // �÷��̾� ���� ���� ����
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
        playerController.isUsingUlt = true; // �÷��̾� ���� ���·� ����
        yield return new WaitForSeconds(shieldDelay);

        shield.SetActive(false);
        Debug.Log("Shield Off");
        playerController.isUsingUlt = false; // �÷��̾� ���� ���� ����
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
        playerController.isUsingUlt = true;
        Collider[] hits = Physics.OverlapBox(ultAll.transform.position, ultAll.transform.localScale / 2f, Quaternion.identity, enemyBullet);

        foreach (Collider c in hits)
        {
            c.gameObject.SetActive(false);
        }
        ultAll.SetActive(true);

        yield return new WaitForSeconds(allDelay);
        ultAll.SetActive(false);
        hits = null;
        ultRoutine = null;

        playerController.isUsingUlt = false;
        Debug.Log("�ڷ�ƾ ����");

        yield break;
    }

    // �ñر� ź�� 1ȸ + �ٴ���Ʈ
    public void BigBullet(float damage)
    {
        Debug.Log($"BigBullet Damage: {damage}");
        if (ultRoutine == null)
        {
            fireCounter = 1;
            ultRoutine = StartCoroutine(UltFireCoroutine(damage, bigBulletSpeed, bigBulletDelay));
        }
        else
        {
            return;
        }
    }

    public void ManyBullets(float damage)
    {
        Debug.Log($"ManyBullets Damage: {damage}");
        
        if (ultRoutine == null && fireCounter <= 0)
        {
            Debug.Log("ManyBullets Start Coroutine");
            fireCounter = 5;
            ultRoutine = StartCoroutine(UltFireCoroutine(damage, manyBulletSpeed, manyBulletDelay));
        }
        Debug.Log($"{playerController.poolIndex}");
    }


    public IEnumerator UltFireCoroutine(float damage, float bulletSpeed, float delay)
    {
        Debug.Log($"UltFireCoroutine Damage: {damage}, Speed: {bulletSpeed}");
        PlayerController.canAttack = false; // ���� �Ұ� ���·� ����
        playerController.isUsingUlt = true; // �÷��̾� ���� ���·� ����
        while (fireCounter > 0)
        {
            playerController.poolIndex = 1;
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
                info.trans.rotation = info.trans.rotation * Quaternion.Euler(0, 3 * fireCounter , 0);
                info.rig.velocity = Vector3.zero;
                
                info.bulletController.attackPower = (int)damage;
                info.bulletController.canDeactive = false;
                
                info.rig.AddForce(bulletSpeed * info.trans.forward, ForceMode.Impulse); // �� �κ��� Ŀ�����ϸ� ��
            }
            Debug.Log($"���� �߻� ��: {fireCounter} remaining");
            yield return new WaitForSeconds(delay);
        }
        playerController.poolIndex = 0;
        StopCoroutine(ultRoutine);
        Debug.Log("UltFireCoroutine Ended");
        PlayerController.canAttack = true; // ���� ���� ���·� ����
        playerController.isUsingUlt = false; // �÷��̾� ���� ���� ����
        ultRoutine = null;
    }

    // ź�� ���� + ������ ����
    public void BulletUpgrade()
    {
        Debug.Log("Bullet Upgrade Called");
        if (ultRoutine == null)
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
        Debug.Log($"{PlayerController.canAttack}");
        Debug.Log("Upgrade Routine Started");
        playerController.isUsingUlt = true; // �÷��̾� ���� ���·� ����
        playerController.poolIndex = 1;
        Debug.Log("Upgrade Bullet Shot");
        yield return new WaitForSeconds(bulletUpgradeTime);

        playerController.poolIndex = 0;
        Debug.Log("Normal Bullet Shot");
        playerController.isUsingUlt = false; // �÷��̾� ���� ���� ����
        ultRoutine = null;
        yield break;
    }
}
