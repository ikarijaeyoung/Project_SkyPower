using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "CircleShapeShot", menuName = "ScriptableObject/BulletPattern/CircleShapeShot")]

public class CircleShapeShot : BulletPatternData
{
    [Header("Circle Shape Shot Settings")]
    public int shotCount = 8;
    public float fireDelayBetweenShots = 0f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        for (int i = 0; i < shotCount; i++)
        {
            BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;
            bullet.objectPool = pool;

            if (bullet != null)
            {
                // firePoints 인덱스가 배열 크기를 넘지 않도록 순환시킵니다.
                // shotCount > firePoints == firePoints[i]에서 배열 범위 벗어남. => IndexOutOfRangeException Error.
                Transform curFirePoint = firePoints[i % firePoints.Length];

                bullet.ReturnToPool(returnToPoolTimer);

                foreach (BulletInfo info in bullet.bullet)
                {
                    if (info.rig != null)
                    {
                        info.trans.gameObject.SetActive(true);
                        info.trans.position = firePoints[0].position;
                        info.rig.velocity = Vector3.zero;
                        info.rig.AddForce(curFirePoint.forward * bulletSpeed, ForceMode.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(fireDelayBetweenShots);
        }
    }
}
