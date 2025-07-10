using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "SingleShotToPlayerPos", menuName = "ScriptableObject/BulletPattern/SingleShotToPlayerPos")]
public class SingleShotToPlayerPos : BulletPatternData
{
    [Header("Single Shot To Player Pos Settings")]
    public float bulletSpeed = 1f;
    Vector3 playerPos;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool,int attackPower)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Quaternion[] originRots = new Quaternion[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            originRots[i] = firePoints[i].rotation;
        }
        firePoints[0].LookAt(playerPos);

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
                    // TODO : 총구쪽으로 모든 총알이 모이니, 여러 총알일때는 수정해야함.
                    info.trans.position = firePoints[0].position;
                    info.trans.rotation = firePoints[0].rotation;
                    info.rig.velocity = Vector3.zero;
                    info.bulletController.attackPower = attackPower;
                    info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                }
            }
        }
        for (int i = 0; i < firePoints.Length; i++)
        {
            firePoints[i].rotation = originRots[i];
        }
        yield return null;
    }
}

