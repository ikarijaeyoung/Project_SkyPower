using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 10f;
        public float lifeTime = 3f;

        private float timer;

        void OnEnable()
        {
            timer = 0f;
        }

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime); // Z+ 방향 이동

            timer += Time.deltaTime;
            if (timer >= lifeTime)
                gameObject.SetActive(false); // 또는 poolManager.Despawn 호출
        }
    }
}