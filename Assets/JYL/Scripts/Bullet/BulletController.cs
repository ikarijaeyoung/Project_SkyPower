using UnityEngine;

namespace JYL
{
    public class BulletController : MonoBehaviour
    {
        public Rigidbody rig;
        private Collider col;
        void Awake()
        {
            rig = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.useGravity = false;
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            gameObject.SetActive(false);
        }
    }
}
