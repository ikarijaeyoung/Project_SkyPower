using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "ShotFirePoints", menuName = "ScriptableObject/BulletPattern/ShotFirePoints")]

public class ShotFirePoints : BulletPatternData
{
    [Header("ShotFirePoints Shot Settings")]
    public float bulletSpeed = 1f;
    public int shotCount = 8;
    public float fireDelayBetweenShots = 0.5f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool, int attackPower)
    {
        for (int j = 0; j < shotCount; j++)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

                //bulletPrefab.transform.position = gameObject.transform.position; 에너미 위치로 가져와야 함. 필요없을 수도 잇음

                if (bulletPrefab != null)
                {
                    bulletPrefab.objectPool = pool;
                    bulletPrefab.ReturnToPool(returnToPoolTimer);

                    foreach (BulletInfo info in bulletPrefab.bulletInfo)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.localPosition = info.originPos;
                            info.trans.position = firePoints[i].position;
                            info.trans.rotation = Quaternion.LookRotation(firePoints[i].forward);
                            info.rig.velocity = Vector3.zero;
                            info.bulletController.attackPower = attackPower;
                            info.rig.AddForce(firePoints[i].forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(fireDelayBetweenShots);
        }
    }
}
