using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using Unity.VisualScripting.Antlr3.Runtime;

[CreateAssetMenu(fileName = "FanShapeShot", menuName = "ScriptableObject/BulletPattern/FanShapeShot")]
public class FanShapeShot : BulletPatternData
{
    [Header("Fan Shape Shot Settings")]
    public int shotCount = 5; // 한 번에 발사할 총알의 개수
    public float fireDelayBetweenShots = 0.1f;
    public float fanShapeangle = 90;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool)
    {
        for (int i = 0; i < shotCount; i++)
        {
            BulletPrefabController bullet = pool.ObjectOut() as BulletPrefabController;
            bullet.objectPool = pool;

            float angle = i * (fanShapeangle / (shotCount - 1)) - (fanShapeangle / 2) + 180;
            firePoints[0].rotation = Quaternion.Euler(0, angle, 0); // Y축을 기준으로 회전
            firePoints[0].forward = firePoints[0].rotation * Vector3.forward; // 회전된 방향으로 총구를 향하게 함
            if (bullet != null)
            {
                bullet.ReturnToPool(returnToPoolTimer);

                foreach (BulletInfo info in bullet.bullet)
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
            yield return new WaitForSeconds(fireDelayBetweenShots); // 여기서 간격을 두어 다른 모양으로 변경 가능.
        }
    }
}

