using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using System.Diagnostics.Tracing;

[CreateAssetMenu(fileName = "SingleShot", menuName = "ScriptableObject/BulletPattern/SingleShot")]
public class SingleShot : BulletPatternData
{
    [Header("Single Shot Settings")]
    public float bulletSpeed = 1f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool, int attackPower)
    {
        BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;
        if (bulletPrefab != null)
        {
            bulletPrefab.transform.position = firePoints[0].position;
            bulletPrefab.objectPool = pool;
            bulletPrefab.ReturnToPool(returnToPoolTimer);

            foreach (BulletInfo info in bulletPrefab.bulletInfo)
            {
                if (info.rig != null)
                {
                    info.trans.gameObject.SetActive(true);
                    info.trans.localPosition = info.originPos;
                    info.trans.rotation = firePoints[0].rotation;
                    info.rig.velocity = Vector3.zero;
                    info.bulletController.attackPower = attackPower;
                    info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                }
                yield return null;
            }
        }
    }
}
//
// 왜냐하면 
// a enemy(SingleShot) 소환 - a ObjectPool로 연결됨. =>
// b enemy(SingleShot) 소환 - b ObjectPool로 연결됨. =>
// a enemy의 ObjectPool은 b ObjectPool로 덮어 씌우게 됨 =>
// Elite Enemy와 Normal Enemy의 BulletPattern이 같다면 문제발생.
//

