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

        private int poolIndex = 0;
        private ObjectPool curBulletPool => bulletPools[poolIndex];

        // 스킬
        // private Parry 1 - 출전하는 서브캐릭 1의 패리스킬
        // private Parry 2 - 출전하는 서브캐릭 2의 패리스킬

        private void Awake()
        {
        }
        private void Update()
        {
            PlayerHandler();
            Debug.Log($"{curBulletPool}");
        }

        private void PlayerHandler()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire1();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                poolIndex = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                poolIndex = 1;
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                poolIndex = 2;
            }
            PlayerMove();
            //UseUlt
            //Parry
        }

        private void PlayerMove()
        {

        }

        private void Fire1()
        {
            BulletPrefabController bullet = curBulletPool.ObjectOut() as BulletPrefabController;
            bullet.transform.position = muzzlePoint.position;
            bullet.ReturnToPool(bulletReturnTimer);
            foreach(BulletInfo info in bullet.bullet)
            {
                if(info.rig == null)
                {
                    continue;
                }
                info.trans.gameObject.SetActive(true);
                info.trans.position = info.originPos;
                info.rig.velocity = Vector3.zero;
                info.rig.AddForce(playerModel.fireSpeed * info.trans.forward, ForceMode.Impulse);
            }
        }
        private void UseUlt()
        {

        }
        private void Parry()
        {

        }
    }
}