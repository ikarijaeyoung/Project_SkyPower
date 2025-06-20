using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    public string EnemyName;
    public int maxHP; // 몬스터 종류에 따라 최대 체력이 다른가?

    // Binary Tree Pattern을 여기서 사용하나?

    // 움직임은 Animator로 만들것.
}
public class EnemyDropItemData : ScriptableObject
{
    [Header("Enemy Drop Item")]
    public string itemName;
    public Sprite itemIcon; // 인게임 내에서 보일 모습.
    public float dropRate;

    // 플레이어는 주변 아이템을 자력으로 흡수한다. => 아이템은 플레이어에게 어떻게 다가갈 것인가?
}
public class BulletPatternData : ScriptableObject
{
    [Header("Pattern Basic Info")]
    public string patternName = "Test SingleShot Pattern"; // 테스트용으로 싱글샷.
    public GameObject bulletPrefab; // Bullet은 상위 클래스에서 상속 받을것.
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;
    public int bulletDamage = 1; // 탄환이 주는 피해량 (플레이어의 체력은 몇 단위로 감소될 것인가?)

    [Header("Pattern Type")]
    public PatternType patternType = PatternType.SingleShot; // 테스트용으로 싱글샷.
    public enum PatternType
    {
        SingleShot, // 단일 발사
        // 3점사
        // 부채꼴 (특정 각도사이로 흩뿌리기)
        // 유도탄 (Player Lock on)
    }
}
