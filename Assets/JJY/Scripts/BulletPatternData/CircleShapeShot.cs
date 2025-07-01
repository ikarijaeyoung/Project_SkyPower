using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "CircleShapeShot", menuName = "ScriptableObject/BulletPattern/CircleShapeShot")]

public class CircleShapeShot : BulletPatternData
{
    [Header("Circle Shape Shot Settings")]
    public int shotCount = 8; // 한 번에 발사할 총알의 개수 : 총구 개수의 배수, 총구 개수보다 많아야할듯.
    public float fireDelayBetweenShots = 0f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        for (int i = 0; i < shotCount; i++)
        {
            BulletPrefabController bullet = objectPool.ObjectOut() as BulletPrefabController;

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
                        info.rig.AddForce(firePoints[i].forward * bulletSpeed, ForceMode.Impulse);
                    }
                }
            }
            yield return new WaitForSeconds(fireDelayBetweenShots);
        }
    }
}
