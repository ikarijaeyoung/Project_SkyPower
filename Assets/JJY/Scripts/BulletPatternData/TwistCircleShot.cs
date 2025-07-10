using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "TwistCircleShot", menuName = "ScriptableObject/BulletPattern/TwistCircleShot")]
public class TwistCircleShot : BulletPatternData
{
    [Header("Twist Circle Shot Settings")]
    public float bulletSpeed = 1f;
    public int shotCount = 8;
    public int CircleCount = 5;
    public float fireDelayBetweenShots = 0f;
    public float fireDelayBetweenCircle = 0.5f;
    public float twistAnglePerCircle = 10f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, ObjectPool pool,int attackPower)
    {
        Quaternion[] originalRotations = new Quaternion[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            originalRotations[i] = firePoints[i].rotation;
        }

        for (int j = 0; j < CircleCount; j++)
        {
            bool twistThisCircle = (j % 2 == 1);

            for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

                //bulletPrefab.transform.position = gameObject.transform.position; 에너미 위치로 가져와야 함. 필요없을 수도 잇음

                if (bulletPrefab != null)
                {
                    int idx = i % firePoints.Length;

                    firePoints[idx].rotation = originalRotations[idx];

                    if (twistThisCircle)
                    {
                        firePoints[idx].Rotate(0f, twistAnglePerCircle, 0f);
                    }

                    Transform curFirePoint = firePoints[idx];

                    bulletPrefab.objectPool = pool;
                    bulletPrefab.ReturnToPool(returnToPoolTimer);

                    foreach (BulletInfo info in bulletPrefab.bulletInfo)
                    {
                        if (info.rig != null)
                        {
                            info.trans.gameObject.SetActive(true);
                            info.trans.localPosition = info.originPos;
                            info.trans.position = curFirePoint.position;
                            info.trans.rotation = Quaternion.LookRotation(curFirePoint.forward);
                            info.rig.velocity = Vector3.zero;
                            info.bulletController.attackPower = attackPower;
                            info.rig.AddForce(curFirePoint.forward * bulletSpeed, ForceMode.Impulse);
                        }
                    }
                }
                yield return new WaitForSeconds(fireDelayBetweenShots);
            }
            yield return new WaitForSeconds(fireDelayBetweenCircle);
        }
    }
}
