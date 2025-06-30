using LJ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorController : MonoBehaviour
{
    public CharacterData characterData;

    public CharacterInventory charictorInventory;

    public int id;
    public Grade grade;
    public string name;
    public Elemental elemental;

    public int level;
    public int step;
    public int exp;

    public int Hp;
    public int attackDamage;
    public float attackSpeed;
    public float moveSpeed;
    public int defense;

    public int ultLevel;
    public float ultDamage;
    public int ultCool;

    public GameObject bulletPrefab;
    public GameObject ultPrefab;

    public Parry parry;
    public int parryCool;
    public Sprite image;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            id = characterData.id;
            // 저장위치에 따른 index 변화 구현 필요
            //SaveManager.Instance.PlayerLoad(charictorSave, 0);
            SetParameter();
        }
    }
    private void SetParameter()
    {
        // Data의 값을 그대로 가져옴
        //bulletPrefab = characterData.bulletPrefab;
        // ultPrefab = characterData.ultVisual;
        // image = charictorData.image;
        
        grade = characterData.grade;
        name = characterData.name;
        elemental = characterData.elemental;
        attackSpeed = characterData.attackSpeed;
        moveSpeed = characterData.moveSpeed;
        defense = characterData.defense;

        // Save의 값을 그대로 가져옴  
        CharacterSave characterSave = charictorInventory.characters.Find(c => c.id == id);
        Debug.Log($"Character ID: {characterSave.id}, Step: {characterSave.step}, Level : {characterSave.level}");
        level = characterSave.level;

        // Save의 값에 따라 Data의 값을 변경
        Hp = characterData.hp + (characterData.hpPlus * (level - 1));
        attackDamage = (int)(characterData.attackDamage + (characterData.damagePlus * (level - 1)));
        ultLevel = step + 1;
        ultCool = characterData.ultCoolDefault - (characterData.ultCoolReduce * step);
        switch (id)
        {
            case 10001:
                ultDamage = characterData.attackDamage * ((150 + Mathf.Pow(25, step)) / 100);
                break;
            case 10002:
                ultDamage = characterData.attackDamage * ((120 + Mathf.Pow(20, step)) / 100);
                break;
            case 10003:
                ultDamage = characterData.attackDamage * ((150 + Mathf.Pow(50, step)) / 100);
                break;
            case 10004:
                ultDamage = characterData.attackDamage * ((130 + Mathf.Pow(30, step) ) / 100);
                break;
            case 10005:
                ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step) ) / 100);
                break;
            case 10006:
                ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step)) / 100);
                break;
            default:
                ultDamage = characterData.attackDamage * ((150 + Mathf.Pow(50,step)) / 100);    
                break;
        }
        
    }

}
