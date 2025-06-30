using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "SnakeShot", menuName = "ScriptableObject/BulletPattern/SnakeShot")]

public class SnakeShot : BulletPatternData
{
    [Header("Snake Shot Settings")]
    public int shotCount = 10;
    public float delayBetweenshots = 0.1f;
    public float fireDelay = 0f;
    public float returnToPoolTimer = 5f;

    [Header("Fire Point Movement")]
    public float firePointsMoveRadius = 0.5f;
    public float firePointsMoveSpeed = 1f;
    public override IEnumerator Shoot(Transform[] firePoints, GameObject bulletPrefab, float bulletSpeed)
    {
        while (true)
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
                            info.rig.AddForce(firePoints[0].forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(delayBetweenshots);
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}
