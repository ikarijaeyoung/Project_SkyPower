using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerModel playerModel;
        [SerializeField] Transform muzzlePoint;
        ObjectPool objectPool;

        private void Awake()
        {
            objectPool = GetComponent<ObjectPool>();
        }
        private void Update()
        {
            
        }

        private void FireBullet()
        {
            // 여기서 오브젝트 풀에서 꺼낸 다음, 발사
        }
    }
}

