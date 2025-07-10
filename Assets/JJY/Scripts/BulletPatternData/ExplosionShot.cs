using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "ExplosionShot", menuName = "ScriptableObject/BulletPattern/ExplosionShot")]
public class ExplosionShot : BulletPatternData
{
    [Header("Explosion Shot Settings")]
    public float bulletSpeed = 1f;
    public float returnToPoolTimer = 5f;
    public float explosionDelay = 1f;
    public int explosionBullets = 4;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool, int attackPower)
    {
        BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

        if (bulletPrefab != null)
        {
            bulletPrefab.objectPool = pool;
            bulletPrefab.transform.position = firePoints[0].position;
            bulletPrefab.transform.rotation = firePoints[0].rotation;

            foreach (BulletInfo info in bulletPrefab.bulletInfo)
            {
                if (info.rig != null)
                {
                    info.trans.localPosition = info.originPos;
                    info.trans.gameObject.SetActive(true);
                    // info.trans.position = firePoints[0].position;
                    // info.trans.rotation = firePoints[0].rotation;
                    info.rig.velocity = Vector3.zero;
                    info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                }
            }
            yield return new WaitForSeconds(explosionDelay);
            Vector3 explosionPos = bulletPrefab.bulletInfo[0].trans.position;
            bulletPrefab.ReturnToPool();

            for (int i = 0; i < explosionBullets; i++)
            {
                float angle = i * (360f / explosionBullets);
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                BulletPrefabController explosionBullet = pool.ObjectOut() as BulletPrefabController;

                if (explosionBullet != null)
                {
                    explosionBullet.objectPool = pool;
                    explosionBullet.transform.position = explosionPos;
                    explosionBullet.transform.rotation = Quaternion.LookRotation(dir);

                    explosionBullet.ReturnToPool(returnToPoolTimer);

                    foreach (BulletInfo info in explosionBullet.bulletInfo)
                    {
                        if (info.rig != null)
                        {
                            // info.trans.localPosition = info.originPos;
                            info.trans.gameObject.SetActive(true);
                            info.trans.position = explosionPos;
                            info.rig.velocity = Vector3.zero;
                            info.rig.AddForce(dir * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
            }
        }
    }
}
