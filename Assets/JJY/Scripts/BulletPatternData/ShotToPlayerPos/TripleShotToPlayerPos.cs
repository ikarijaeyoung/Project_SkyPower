using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System;
using JYL;

[CreateAssetMenu(fileName = "TripleShotToPlayerPos", menuName = "ScriptableObject/BulletPattern/TripleShotToPlayerPos")]
public class TripleShotToPlayerPos : BulletPatternData
{
    [Header("Triple Shot To Player Pos Settings")]
    public int shotCount = 3;
    public float delayBetweenshots = 0.1f;
    Vector3 playerPos;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        firePoints[0].LookAt(playerPos);

        for (int i = 0; i < shotCount; i++)
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
                        info.trans.localPosition = info.originPos;
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

