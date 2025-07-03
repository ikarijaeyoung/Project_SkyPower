using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "ExplosionShot", menuName = "ScriptableObject/BulletPattern/ExplosionShot")]

public class ExplosionShot : BulletPatternData
{
    [Header("Explosion Shot Settings")]
    public float returnToPoolTimer = 5f;
    public float explosionDelay = 1f;
    public int explosionBullets = 4;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;

        bullet.objectPool = pool;

        if (bullet != null)
        {
            bullet.ReturnToPool(returnToPoolTimer);

            foreach (BulletInfo info in bullet.bullet)
            {
                if (info.rig != null)
                {
                    info.trans.gameObject.SetActive(true);
                    info.trans.position = firePoints[0].position;
                    info.rig.velocity = Vector3.zero;
                    info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                }
            }
            yield return new WaitForSeconds(explosionDelay);

            Debug.Log("ExplosionSHot : 펑, 마저 구현해야함.");
            Vector3 explosionPos = bullet.transform.position;
            bullet.gameObject.SetActive(false);

            for (int i = 0; i < explosionBullets; i++)
            {
                float angle = i * (360f / explosionBullets);
                Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

                BulletPrefabController explosionBullet = pool.ObjectOut() as BulletPrefabController;

                bullet.objectPool = pool;

                if (explosionBullet != null)
                {
                    explosionBullet.ReturnToPool(returnToPoolTimer);

                    foreach (BulletInfo info in explosionBullet.bullet)
                    {
                        if (info.rig != null)
                        {
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
