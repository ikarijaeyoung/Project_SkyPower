using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;
    public EnemyDropItemData enemyDropItemData;
    public BulletPatternData bulletPatternData;
    private int currentHP;
    private bool isMoving = true; // 몬스터는 맵 밖에서 소환되어, 특정 위치로 애니메이터를 통해 이동된다. 이동중에는 공격을 하면 안되기 때문에 공격은 isMoving이 false일 때만 기능한다.
    void Start()
    {
        currentHP = enemyData.maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Die();
    }

    private void Die()
    {
        // 죽는 애니메이션 실행.
        // EnemyDropItemData를 통해 아이템 드랍.
    }

    private void Fire()
    {
        // BulletPatternData에서 PatternType을 선택해서 발사.
    }
}
