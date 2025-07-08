using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "ShotToPlayerPosFirePoints", menuName = "ScriptableObject/BulletPattern/ShotToPlayerPosFirePoints")]

public class ShotToPlayerPosFirePoints : BulletPatternData
{
    [Header("Shot To PlayerPos Fire Points Settings")]
    public int shotCount = 8;
    public float fireDelayBetweenShots = 0.5f;
    public float returnToPoolTimer = 5f;
    private Vector3 playerPos;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool, int attackPower)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        Quaternion[] originRots = new Quaternion[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            originRots[i] = firePoints[i].rotation;
        }

        for (int j = 0; j < shotCount; j++)
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

                if (bulletPrefab != null)
                {
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
        for (int i = 0; i < firePoints.Length; i++)
        {
            firePoints[i].rotation = originRots[i];
        }
    }
}
