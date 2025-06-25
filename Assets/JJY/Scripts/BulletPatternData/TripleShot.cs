using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System;

[System.Serializable]
[CreateAssetMenu(fileName = "TripleShotData", menuName = "BulletPattern/TripleShotData")]
public class TripleShot : BulletPatternData
{
    private int shotCount = 3;
    private float delay= 0.1f;
    private Coroutine shotRoutine;
    public override void Shoot(Transform firePoint, GameObject bulletPrefab, float bulletSpeed)
    {
        
        //shotRoutine = StartCoroutine(TripleShotDelay(firePoint,bulletPrefab,bulletSpeed));
    }

    // MonoBehaviour를 상속받지 않기 때문에 사용 불가 = 사용하려면 MonoBehaviour를 상속받을 필요가 있음.
    IEnumerator TripleShotDelay(Transform firePoint,GameObject bulletPrefab,float bulletSpeed)
    {
        for (int i = 0; i < shotCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
                // 대기
            }
            else Debug.Log("Triple Shot Error");

            Debug.Log("Triple Shot : 두두두");
            yield return new WaitForSeconds(delay);
        }
    }
}

