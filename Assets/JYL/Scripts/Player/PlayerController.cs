using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Set Scriptable Object")]
        [SerializeField] PlayerModel playerModel;

        [Header("Set References")]
        [SerializeField] Transform muzzlePoint;

        [Header("Set Value")]
        [SerializeField] float bulletReturnTimer = 2f;
        [SerializeField] List<ObjectPool> bulletPools;

        private ObjectPool curBulletPool;

        private void Awake()
        {
            curBulletPool = bulletPools[0];
        }
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireBullet();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                curBulletPool = bulletPools[0];
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                curBulletPool = bulletPools[1];
            }
        }
        private void PlayerMove()
        {

        }
        private void FireBullet()
        {
            BulletController bullet = curBulletPool.ObjectOut() as BulletController;
            bullet.transform.position = muzzlePoint.position;
            bullet.transform.forward = muzzlePoint.forward;
            bullet.ReturnToPool(bulletReturnTimer);
            foreach(Rigidbody rig in bullet.rigs)
            {
                rig.velocity = Vector3.zero;
                rig.AddForce(playerModel.fireSpeed * muzzlePoint.forward, ForceMode.Impulse);
            }
        }
    }
}

