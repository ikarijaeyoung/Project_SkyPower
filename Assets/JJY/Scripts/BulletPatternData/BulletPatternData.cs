using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

public abstract class BulletPatternData : MonoBehaviour
{
    protected ObjectPool objectPool => FindObjectOfType<ObjectPool>();
    public abstract IEnumerator Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed);
}
