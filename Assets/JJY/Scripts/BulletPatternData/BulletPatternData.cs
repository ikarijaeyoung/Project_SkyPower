using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

public abstract class BulletPatternData : ScriptableObject
{
    protected ObjectPool objectPool;
    public void SetPool(ObjectPool pool)
    {
        objectPool = pool;
    }
    public abstract IEnumerator Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed);
}
