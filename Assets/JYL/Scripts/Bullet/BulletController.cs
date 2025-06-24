using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class BulletController : PooledObject
    {
        public Rigidbody[] rigs;
        public Transform[] childTransforms;
        public Vector3[] childPosOrigin;
        void Awake()
        {
            rigs = GetComponentsInChildren<Rigidbody>();
        }

        void Update()
        {

        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 1<<7)
            {
                ReturnToPool();
            }
        }

    }
}