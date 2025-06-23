using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.KYG.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] public float speed;

        public GameManagerSO gameManager;

        public ObjectPoolManagerSO poolManager;

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);

            if (transform.position.y < -6f)

                Destroy(gameObject);


        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Bullet"))
            {
                collision.gameObject.SetActive(false);
                gameManager.AddScore(100);
                Destroy(gameObject);
            }
        }
    }
}