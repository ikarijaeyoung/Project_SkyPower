using LJ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharictorController : MonoBehaviour
{
    public CharictorDataTest charictorData;

    public CharictorSave charictorSave = new();

    public int level;
    public int Hp;
    public int exp;
    public int attackPower;
    public float attackSpeed;
    public float moveSpeed;
    public GameObject bulletPrefab;
    public GameObject model;
    public Sprite image;

    private void Start()
    {
        // 저장위치에 따른 index 변화 구현 필요
        SaveManager.Instance.PlayerLoad(charictorSave, 0);
    }
    private void SetParameter()
    {
        // Data의 값을 그대로 가져옴
        // bulletPrefab = charictorData.bulletPrefab;
        // model = charictorData.model;
        // image = charictorData.image;

        // Save의 값을 그대로 가져옴
        level = charictorSave.level;
        exp = charictorSave.exp;
    }

}
