using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System;
using JYL;

public class TripleShot : BulletPatternData
{
    private int shotCount = 3;
    private float delayBetweenshots = 0.1f;
    private float fireDelay = 2f;
    public float bulletReturnTimer = 1.5f;
    public override IEnumerator Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed)
    {
        BulletPrefabController bullets = objectPool.ObjectOut() as BulletPrefabController;

        while (true)
        {
            // bullets.transform.position = firePoint.position;
            for (int i = 0; i < shotCount; i++)
            {
                // bullet.ReturnToPool(bulletReturnTimer);
                foreach (BulletInfo info in bullets.bullet)
                {
                    if (info.rig != null)
                    {
                        info.trans.gameObject.SetActive(true);
                        info.trans.position = firePoint.position;
                        info.rig.velocity = Vector3.zero;
                        info.rig.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
                        yield return new WaitForSeconds(delayBetweenshots);
                    }
                }
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}

