using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

namespace JJY
{
    public class EnemyBulletController : MonoBehaviour
    {
        public Rigidbody rb;
        public Collider col;
        void Awake()
        {
            Init();
        }
        void Init()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;
            col.isTrigger = true;
        }
        private void OnTriggerEnter(Collider other)
        {
            // gameObject.SetActive(false);
        }
    }
}
