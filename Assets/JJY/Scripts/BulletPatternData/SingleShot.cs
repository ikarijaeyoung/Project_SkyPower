using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "SingleShot", menuName = "ScriptableObject/BulletPattern/SingleShot")]
public class SingleShot : BulletPatternData
{
    [Header("Single Shot Settings")]
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;
        bullet.ReturnToPool(returnToPoolTimer);

        foreach (BulletInfo info in bullet.bulletInfo)
        {
            if (info.rig != null)
            {
                info.trans.gameObject.SetActive(true);

                // TODO : 총구쪽으로 모든 총알이 모이니, 여러 총알일때는 수정해야함.
                info.trans.position = firePoints[0].position;
                info.rig.velocity = Vector3.zero;
                info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
            }
            yield return null;
        }
    }
}


