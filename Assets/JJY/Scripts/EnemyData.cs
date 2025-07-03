using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public int maxHP;
    public Sprite enemyIcon;
    public EnemyType enemyType; // 각 타입마다 ObjectPool다름
}
public enum EnemyType
{
    Normal,
    Elite,
    Boss
    // 지상적? => 지상적 전용 ObjectPool? == 지상적은 공중유닛과 다른 BulletPrefab(외형).
}
[CreateAssetMenu(fileName = "NewItemData", menuName = "ScriptableObject/ItemData")]
public class EnemyDropItemData : ScriptableObject
{
    [Header("Enemy Drop Item")]
    public string itemName; // 필요한가?
    public Sprite itemIcon; // 인게임 내에서 보일 모습.
    public float dropRate; // == 떨어질 개수
}
