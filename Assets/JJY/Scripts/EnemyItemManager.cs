using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public int itemCount = 40; // 아이템 개수
    public float dropRadius = 2f; // 아이템이 떨어지는 반경 제한
    public float dropItemSpeed = 1f; // 아이템이 퍼져나가는 속도

    void OnEnable()
    {
        Enemy.OnEnemyDied += DropItems;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDied -= DropItems;
    }

    void DropItems(Vector3 position)
    {
        for (int i = 0; i < itemCount; i++)
        {
            // 아이템이 죽은 적의 위치 중심에서 원 모양으로 퍼져나간다.
            // 아이템이 퍼지는 방향 = i + 1/360 <= direction
            GameObject item = Instantiate(itemPrefab, position, Quaternion.identity);
            GetComponent<Rigidbody>();
            // item.transform = Vector3.MoveTowards(item.transform.position, )
            // 여긴 제가 어제까지 하고있었어요. 넵 이것저것 해보는중입니다.
            // 여기 막혔음
            
        }
    }
}
