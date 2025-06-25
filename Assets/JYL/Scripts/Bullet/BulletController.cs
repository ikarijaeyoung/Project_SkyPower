using UnityEngine;

namespace JYL
{
    public class BulletController : MonoBehaviour
    {
        public Rigidbody rig;
        void Awake()
        {
            rig = GetComponent<Rigidbody>();
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.useGravity = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            gameObject.SetActive(false);
        }
    }
}
