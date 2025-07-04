using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using System.Diagnostics.Tracing;

[CreateAssetMenu(fileName = "SingleShot", menuName = "ScriptableObject/BulletPattern/SingleShot")]
public class SingleShot : BulletPatternData
{
    [Header("Single Shot Settings")]
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;
        bullet.objectPool = pool;
        //
        // 왜냐하면 
        // a enemy(SingleShot) 소환 - a ObjectPool로 연결됨. =>
        // b enemy(SingleShot) 소환 - b ObjectPool로 연결됨. =>
        // a enemy의 ObjectPool은 b ObjectPool로 덮어 씌우게 됨 =>
        // Elite Enemy와 Normal Enemy의 BulletPattern이 같다면 문제발생.
        //
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


