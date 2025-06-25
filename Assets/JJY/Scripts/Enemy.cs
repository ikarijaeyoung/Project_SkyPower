using JYL;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    // TODO : 스크립트, 프리팹으로 변경
    public BulletPatternData bulletPatternData;
    private int currentHP;
    public bool isMoving = false; // 몬스터는 맵 밖에서 소환되어, 특정 위치로 애니메이터를 통해 이동된다. 이동중에는 공격을 하면 안되기 때문에 공격은 isMoving이 false일 때만 기능한다.
    private float fireTimer;
    public Transform firePoint;
    public GameObject bulletPrefab;

    // 오브젝트 풀
    // TODO : 외부 오브젝트 풀 연결 방법 찾기
    public ObjectPool objectPool => FindObjectOfType<ObjectPool>();
    private float bulletReturnTimer = 1.5f;

    // Enemy의 특성대로 총알 속도와 발사 간격을 조절.
    public float bulletSpeed = 10f;
    public float fireDelay = 0.5f;

    void Start()
    {
        currentHP = enemyData.maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        // 죽는 애니메이션 실행.
        // EnemyDropItemData를 통해 아이템 드랍.
    }

    private void Update()
    {
        // BulletPatternData에서 PatternType을 선택해서 발사.
        if (isMoving) return; // 이동중에는 발사하지 않음.

        Fire();
    }
    private void Fire()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireDelay)
        {
            //bulletPatternData.Shoot(firePoint, bulletPrefab, bulletSpeed);
            fireTimer = 0f;

            BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;
            bullet.transform.position = firePoint.position;
            bullet.ReturnToPool(bulletReturnTimer);
            foreach (BulletInfo info in bullet.bullet)
            {
                if (info.rig == null)
                {
                    continue;
                }
                info.trans.gameObject.SetActive(true);
                info.trans.position = info.originPos;
                info.rig.velocity = Vector3.zero;
                info.rig.AddForce(info.trans.forward * bulletSpeed, ForceMode.Impulse);
            }
        }

    }
}
