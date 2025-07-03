using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "SingleShotToPlayerPos", menuName = "ScriptableObject/BulletPattern/SingleShotToPlayerPos")]
public class SingleShotToPlayerPos : BulletPatternData
{
    [Header("Single Shot To Player Pos Settings")]
    Vector3 playerPos;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;
        // BulletPrefabController.cs 에 public ObjectPool objectPool; 추가
        // bullet.objectPool = pool;
        bullet.ReturnToPool(returnToPoolTimer);

        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        firePoints[0].LookAt(playerPos);

        foreach (BulletInfo info in bullet.bullet)
        {
            if (info.rig != null)
            {
                info.trans.gameObject.SetActive(true);

                // TODO : 총구쪽으로 모든 총알이 모이니, 여러 총알일때는 수정해야함.
                info.trans.position = firePoints[0].position;
                info.rig.velocity = Vector3.zero;
                info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
            }
        }
        yield return null;
    }
}

