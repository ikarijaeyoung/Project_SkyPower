using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "DrillShot", menuName = "ScriptableObject/BulletPattern/DrillShot")]

public class DrillShot : BulletPatternData
{
    [Header("Drill Shot Settings")]
    public float bulletSpeed = 1f;
    public int shotCount = 8;
    public int drillCount = 3;
    public float fireDelayBetweenBullets = 0.1f;
    public float fireDelayCycle = 0.2f;
    public float returnToPoolTimer = 5f;
    private Vector3 playerPos;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool, int attackPower)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        Quaternion[] originRots = new Quaternion[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            originRots[i] = firePoints[i].rotation;
        }
        
        for (int j = 0; j < drillCount; j++)
        {
            for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

                if (bulletPrefab != null)
                {
                    // firePoints 인덱스가 배열 크기를 넘지 않도록 순환시킵니다.
                    // shotCount > firePoints == firePoints[i]에서 배열 범위 벗어남. => IndexOutOfRangeException Error.
                    Transform curFirePoint = firePoints[i % firePoints.Length];

                    bulletPrefab.objectPool = pool;
                    bulletPrefab.ReturnToPool(returnToPoolTimer);

                    firePoints[i].LookAt(playerPos);

                    foreach (BulletInfo info in bulletPrefab.bulletInfo)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.localPosition = info.originPos;
                            info.trans.position = firePoints[i].position;
                            info.trans.rotation = Quaternion.LookRotation(curFirePoint.forward);
                            info.rig.velocity = Vector3.zero;
                            info.bulletController.attackPower = attackPower;
                            info.rig.AddForce(curFirePoint.forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(fireDelayBetweenBullets);
            }
            yield return new WaitForSeconds(fireDelayCycle);
        }
        for (int i = 0; i < firePoints.Length; i++)
        {
            firePoints[i].rotation = originRots[i];
        }
    }
}
