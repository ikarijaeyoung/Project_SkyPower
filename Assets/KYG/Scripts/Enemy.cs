using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.KYG.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float speed;

        public GameManagerSO gameManager;
        public ObjectPoolManagerSO poolManager;
        public string poolKey = "Enemy"; // 풀 키

        void Update()
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y < -6f)
            {
                poolManager.Despawn(poolKey, gameObject);
            }
        }

        private void OnTriggerEnter(Collider collision) // 3D용 콜라이더 이벤트
        {
            if (collision.CompareTag("Bullet"))
            {
                // 총알 비활성화
                poolManager.Despawn("Bullet", collision.gameObject);

                gameManager.AddScore(100);

                // 적 비활성화 (풀 반환)
                poolManager.Despawn(poolKey, gameObject);
            }
        }
    }
}