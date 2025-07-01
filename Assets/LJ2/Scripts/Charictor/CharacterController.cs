using LJ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LJ2
{
    public class CharactorController : MonoBehaviour
    {
        public CharacterData characterData;

        public SaveTester saveTester;

        public int id;
        public Grade grade;
        public string name;
        public Elemental elemental;

        public int level;
        public int step;
        public int exp;
        public int fragle; // 캐릭터 조각 : 캐릭터의 등급을 올리는데 사용됨

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

        public int upgradeUnit;
        public int currentUnit; // 현재 보유한 유닛 수, 업그레이드에 사용됨

        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                id = characterData.id;
                SetParameter();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LevelUp(5000);  // 5000은 예시로, 실제 게임에서는 플레이어가 가진 유닛 수에 따라 다르게 설정해야 함
                SetParameter();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                StepUp();
                SetParameter();
            }
        }
        private void SetParameter()
        {
            // Data의 값을 그대로 가져옴
            // bulletPrefab = characterData.bulletPrefab;
            // ultPrefab = characterData.ultVisual;
            // image = charictorData.image;

            grade = characterData.grade;
            name = characterData.name;
            elemental = characterData.elemental;
            attackSpeed = characterData.attackSpeed;
            moveSpeed = characterData.moveSpeed;
            defense = characterData.defense;


            // Save의 값을 그대로 가져옴  

            CharacterSave characterSave = saveTester.gameData.characterInventory.characters.Find(c => c.id == id);

            if (characterSave.id == 0)
            {
                Debug.LogWarning($"id {id}에 해당하는 CharacterSave를 찾을 수 없습니다.");
                return;
            }

            Debug.Log($"Character ID: {characterSave.id}, Step: {characterSave.step}, Level : {characterSave.level}");
            level = characterSave.level;
            step = characterSave.step;
            fragle = characterSave.fragle;

            // Save의 값에 따라 Data의 값을 변경
            Hp = characterData.hp + (characterData.hpPlus * (level - 1));
            attackDamage = (int)(characterData.attackDamage + (characterData.damagePlus * (level - 1)));
            ultLevel = step + 1;
            ultCool = characterData.ultCoolDefault - (characterData.ultCoolReduce * step);
            upgradeUnit = characterData.upgradeUnitDefault + (characterData.upgradeUnitPlus * level);

            switch (id)
            {
                case 10001:
                    ultDamage = characterData.attackDamage * ((150 + 25 * Mathf.Pow(step, 2)) / 100);
                    break;
                case 10002:
                    ultDamage = characterData.attackDamage * ((120 + 20 * step) / 100);
                    break;
                case 10003:
                    ultDamage = characterData.attackDamage * ((150 + 50 * step) / 100);
                    break;
                case 10004:
                    ultDamage = characterData.attackDamage * ((130 + 30 * step) / 100);
                    break;
                case 10005:
                    ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step)) / 100);
                    break;
                case 10006:
                    ultDamage = characterData.attackDamage * ((150 + (12.5f * Mathf.Pow(step, 2)) + (37.5f * step)) / 100);
                    break;
                default:
                    ultDamage = characterData.attackDamage * ((150 + 50 * step) / 100);
                    break;
            }

        }

        // 업그레이드 가능할 때만 실행
        public void LevelUp(int unit)
        {
            if (level < characterData.maxLevel)
            {
                
                if (unit > upgradeUnit)
                {
                    unit -= upgradeUnit;
                    level++;

                    int index = saveTester.gameData.characterInventory.characters.FindIndex(c => c.id == id);
                    CharacterSave characterSave = saveTester.gameData.characterInventory.characters[index];
                    characterSave.level = level;
                    saveTester.gameData.characterInventory.characters[index] = characterSave;
                }
                else
                {
                    Debug.Log("업그레이드 유닛이 부족합니다.");
                }

            }
            else
            {
                Debug.Log("최대 레벨에 도달했습니다.");
            }
        }

        public void GetUpgradeUnit(int unit)
        {
            currentUnit += unit;
            if (currentUnit > upgradeUnit)
            {
                Debug.Log("업그레이드 가능 유닛이 있습니다.");
            }
            else
            {
                Debug.Log("업그레이드 가능 유닛이 부족합니다.");
            }
        }
        public void StepUp()
        {
            if (step < 4)
            {
                step++;

                int index = saveTester.gameData.characterInventory.characters.FindIndex(c => c.id == id);
                CharacterSave characterSave = saveTester.gameData.characterInventory.characters[index];
                characterSave.step = step;
                saveTester.gameData.characterInventory.characters[index] = characterSave;
            }
            else
            {
                Debug.Log("최대 단계에 도달했습니다.");
            }
        }
    }
}
