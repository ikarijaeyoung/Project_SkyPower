using UnityEngine;

namespace JYL
{
    public struct BulletInfo
    {
        public Transform trans;
        public Rigidbody rig;
        public Vector3 originPos;
        public BulletController bulletController;
    }
    public class BulletPrefabController : PooledObject
    {
        public BulletInfo[] bulletInfo;
        private Transform[] transforms;
        
        private void Awake()
        {
            transforms = GetComponentsInChildren<Transform>();
            bulletInfo = new BulletInfo[transforms.Length];
            InitBulletPrefab();

        }
        public void InitBulletPrefab()
        {
            for (int i = 0; i < bulletInfo.Length; i++)
            {
                bulletInfo[i].rig = transforms[i].GetComponent<Rigidbody>();
                bulletInfo[i].bulletController = GetComponent<BulletController>();
                bulletInfo[i].trans = transforms[i];
                bulletInfo[i].originPos = transforms[i].localPosition;
            }
        }
    }
}

