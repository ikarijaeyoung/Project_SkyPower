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
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool,int attackPower)
    {
        for (int i = 0; i < shotCount; i++)
        {
            BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

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
                        info.trans.position = firePoints[0].position;
                        info.trans.rotation = firePoints[0].rotation;
                        info.rig.velocity = Vector3.zero;
                        info.bulletController.attackPower = attackPower;
                        info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(delayBetweenshots);
        }
    }
}

