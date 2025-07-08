using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

[CreateAssetMenu(fileName = "SnailShot", menuName = "ScriptableObject/BulletPattern/SnailShot")]
public class SnailShot : BulletPatternData
{
    [Header("Snail Shot Settings")]
    public int shotCount = 8;
    public float fireDelayBetweenShots = 0.5f;
    public float returnToPoolTimer = 5f;
    public override IEnumerator Shoot(Transform[] firePoints, float bulletSpeed, ObjectPool pool,int attackPower)
    {
        for (int i = 0; i < shotCount; i++)
            {
                BulletPrefabController bulletPrefab = pool.ObjectOut() as BulletPrefabController;

                //bulletPrefab.transform.position = gameObject.transform.position; 에너미 위치로 가져와야 함. 필요없을 수도 잇음

                if (bulletPrefab != null)
                {
                    // firePoints 인덱스가 배열 크기를 넘지 않도록 순환시킵니다.
                    // shotCount > firePoints == firePoints[i]에서 배열 범위 벗어남. => IndexOutOfRangeException Error.
                    Transform curFirePoint = firePoints[i % firePoints.Length];

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
    }
}
