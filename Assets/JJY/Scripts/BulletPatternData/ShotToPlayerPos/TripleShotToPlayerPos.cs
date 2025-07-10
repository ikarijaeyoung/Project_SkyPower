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
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool, int attackPower)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Quaternion[] originRots = new Quaternion[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            originRots[i] = firePoints[i].rotation;
        }
        firePoints[0].LookAt(playerPos);

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
                        // �Ѿ��� forward�� Muzzlepoint�� forward�� ����
                        info.trans.rotation = firePoints[0].rotation;
                        info.rig.velocity = Vector3.zero;
                        info.bulletController.attackPower = attackPower;
                        info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(delayBetweenshots);
        }
        for (int i = 0; i < firePoints.Length; i++)
        {
            firePoints[i].rotation = originRots[i];
        }
    }
}

