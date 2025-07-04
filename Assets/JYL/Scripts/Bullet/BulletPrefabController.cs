using UnityEngine;

namespace JYL
{
    public struct BulletInfo
    {
        public Transform trans;
        public Rigidbody rig;
        public Vector3 originPos;
    }
    public class BulletPrefabController : PooledObject
    {
        public ObjectPool objectPool; // 여러 종류의 Enemy에서 같은 BulletPattern을 사용할 때, 서로 다른 ObjetPool을 사용할 때 구분하기 위해 필요함.
        public BulletInfo[] bullet;
        private Transform[] transforms;
        private void Awake()
        {
            transforms = GetComponentsInChildren<Transform>();
            bullet = new BulletInfo[transforms.Length];
            InitBulletPrefab();

        }
        public void InitBulletPrefab()
        {
            for (int i = 0; i < bullet.Length; i++)
            {
                bullet[i].trans = transforms[i];
                bullet[i].rig = transforms[i].GetComponent<Rigidbody>();
                bullet[i].originPos = transforms[i].localPosition;
            }
        }
    }
}

