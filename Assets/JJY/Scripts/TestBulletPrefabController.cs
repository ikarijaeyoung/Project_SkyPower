using UnityEngine;
using JYL;

// public struct BulletInfo
// {
//     public Transform trans;
//     public Rigidbody rig;
//     public Vector3 originPos;
// }

// public class TestBulletPrefabController : PooledObject
// {
//     public BulletInfo[] bulletInfo;
//     private Transform[] transforms;
//     
//     public GameObject owner; // 자신이 쏜 총알에 자신이 맞을 때 무시하기 위해.
//
//     private void Awake()
//     {
//         transforms = GetComponentsInChildren<Transform>();
//         bulletInfo = new BulletInfo[transforms.Length];
//         InitBulletPrefab();

//     }
//
//     public void InitBulletPrefab()
//     {
//         for (int i = 0; i < bulletInfo.Length; i++)
//         {
//             bulletInfo[i].trans = transforms[i];
//             bulletInfo[i].rig = transforms[i].GetComponent<Rigidbody>();
//             bulletInfo[i].originPos = transforms[i].localPosition;
//         }
//     }
//     
//     public void OnTriggerEnter(Collider other)
//     {
//          if (other.gameObject == owner) return; // 발사할 때, bulletInfo.owner = bulletOwner; 설정
//          // 플레이어 충돌 시 ReturnToPool(); 
//          // 에너미 충돌 시 ReturnToPool();
//          // 위 두 조건 동시에 카메라 바깥쪽 투명 벽 Collider에 충돌 시 ReturnToPool();
//     }
// }

