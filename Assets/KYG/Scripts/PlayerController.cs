using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    public ObjectPoolManagerSO poolManager;
    public string bulletKey = "Bullet";

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(h, v, 0) * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPos = transform.position + transform.forward * 1f; // 총구 위치
            Quaternion spawnRot = transform.rotation; // 방향 유지
            GameObject bullet = poolManager.Spawn(bulletKey, spawnPos, spawnRot);
        }
    }
}
