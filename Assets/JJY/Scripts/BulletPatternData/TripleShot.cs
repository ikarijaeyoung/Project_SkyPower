using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System;
using JYL;

[CreateAssetMenu(fileName = "TripleShot", menuName = "ScriptableObject/BulletPattern/TripleShot")]
public class TripleShot : BulletPatternData
{
    [Header("Triple Shot Settings")]
    public int shotCount = 3;
    public float delayBetweenshots = 0.1f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        for (int i = 0; i < shotCount; i++)
        {
            BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;
            bullet.objectPool = pool;

            if (bullet != null)
            {
                bullet.ReturnToPool(returnToPoolTimer);

                foreach (BulletInfo info in bullet.bulletInfo)
                {
                    if (info.rig != null)
                    {
                        info.trans.gameObject.SetActive(true);
                        info.trans.position = firePoints[0].position;
                        info.rig.velocity = Vector3.zero;
                        info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(delayBetweenshots);
        }
    }
}

