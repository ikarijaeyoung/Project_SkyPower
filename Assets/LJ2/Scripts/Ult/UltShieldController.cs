using JYL;
using KYG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltShieldController : MonoBehaviour
{
    [SerializeField] public bool isReflect = true;
    private Vector3 reflect;
    public void Reflect(BulletController target)  // 이상하게 돌아가는 중
    {
        reflect.x = -target.rig.velocity.x * 2;
        reflect.z = -target.rig.velocity.z * 2;
        target.rig.velocity = reflect;

        target.gameObject.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        BulletController targetController = collision.gameObject.GetComponent<BulletController>();
        Reflect(targetController);
        
    }
}
