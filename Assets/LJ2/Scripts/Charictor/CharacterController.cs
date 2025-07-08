using JYL;
using KYG_skyPower;
using UnityEngine;

namespace LJ2
{
    [System.Serializable]
    public class CharactorController : MonoBehaviour
    {
        public CharacterData characterData;

        public Parrying parrying;
        public Ultimate ultimate;

        public int id => characterData.id;
        public Grade grade;
        public string charName;
        public Elemental elemental;

        public int level;
        public int step;
        public int exp;

        public int Hp;
        public int attackDamage;
        public float attackSpeed;
        public float moveSpeed;
        public int defense;
        public PartySet partySet;

        public int ultLevel;
        public float ultDamage;
        public int ultCool;

        [SerializeField] public PooledObject bulletPrefab; // TODO : 경로지정
        public PooledObject ultBulletPrefab;
        public GameObject ultPrefab; // 리소스

        public Parry parry;
        public int parryCool;

        public Sprite icon;
        public Sprite image;

        public int upgradeUnit;

        public string attackSound;

        private void Awake()
        {
            parrying = GetComponent<Parrying>();
            ultimate = GetComponent<Ultimate>();
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    id = characterData.id;
            //    SetParameter();
            //}

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    LevelUp(5000);  // 5000은 예시로, 실제 게임에서는 플레이어가 가진 유닛 수에 따라 다르게 설정해야 함
            //    SetParameter();
            //}

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    StepUp();
            //    SetParameter();
            //}
        }

        //public void ApplyEquipmentStat()
        //{
        //    var equips = EquipmentInvenManager.Instance.GetEquippedItems(id);

        //    // 기본값 세팅
        //    attackDamage = (int)characterData.attackDamage;
        //    defense = characterData.defense;
        //    Hp = characterData.hp;

        //    // 무기
        //    if (equips.weapon != null)
        //        attackDamage += equips.weapon.Base_Value;

        //    // 방어구
        //    if (equips.armor != null)
        //        defense += equips.armor.Base_Value;

        //    // 악세서리
        //    if (equips.accessory != null)
        //    {
        //        // 예시: 체력 증가
        //        Hp += equips.accessory.Base_Value;
        //        // 효과 타입별로 추가 구현 (Effect_Type 등)
        //    }
        //}
        public void SetParameter(int weapon, int armor)
        {
            // Data의 값을 그대로 가져옴
            // ultPrefab = characterData.ultVisual;
            // image = charictorData.image;

            //EquipController weapon;
            //EquipController armor;
            //EquipController acce;
            //foreach(var item in equips)
            //{
            //    // 0번 = 무기, 1번이 방어구, 2번이 악세
            //    switch(item.id)
            //    { 
            //    case Manager.Game.CurrentSave.equip[0].id:
            //        weapon = item;
            //        break;
            //    case Manager.Game.CurrentSave.equip[1].id:
            //        armor = item;
            //        break;
            //    case Manager.Game.CurrentSave.equip[2].id:
            //        acce = item;
            //        break;
            //    }
            //}

            grade = characterData.grade;
            charName = characterData.characterName;
            elemental = characterData.elemental;
            attackSpeed = characterData.attackSpeed;
            moveSpeed = characterData.moveSpeed;
            defense = characterData.defense;
            image = characterData.image;
            icon = characterData.icon;
            image = characterData.image;

            switch (grade)
            {
                case Grade.SSR:
                    attackSound = "Atk_Normal_SSR";
                    break;
                case Grade.R:
                    attackSound = "Atk_Normal_R";
                    break;
            }

            // Save의 값을 그대로 가져옴  

            CharacterSave characterSave = Manager.Game.saveFiles[Manager.Game.currentSaveIndex].characterInventory.characters.Find(c => c.id == id);

            if (characterSave.id == 0)
            {
                Debug.LogWarning($"id {id}에 해당하는 CharacterSave를 찾을 수 없습니다.");
                return;
            }

            //Debug.Log($"Character ID: {characterSave.id}, Step: {characterSave.step}, Level : {characterSave.level}");
            level = characterSave.level;
            step = characterSave.step;
            bulletPrefab = characterData.bulletPrefabs[step].GetComponent<PooledObject>(); //TODO : 돌파 상황에 따라 다른 총알 적용 해야 함.
            //bulletPrefab = Resources.Load<PooledObject>($"Prefabs/bullet/{id}_{step}");
            partySet = characterSave.partySet;

            // Save의 값에 따라 Data의 값을 변경
            if (partySet == PartySet.Main)
            {
                Debug.Log($"{characterData.name}무기 공격력: {weapon}, 방어력:{armor}");
                attackDamage = (int)(characterData.attackDamage + (characterData.damagePlus * (level - 1))) + weapon;
                Hp = characterData.hp + (characterData.hpPlus * (level - 1)) + armor;
            }
            else
            {
                Hp = characterData.hp + (characterData.hpPlus * (level - 1));
                attackDamage = (int)(characterData.attackDamage + (characterData.damagePlus * (level - 1)));
            }


            ultLevel = step + 1;
            ultCool = characterData.ultCoolDefault - (characterData.ultCoolReduce * step);
            upgradeUnit = characterData.upgradeUnitDefault + (characterData.upgradeUnitPlus * level);

            switch (id)
            {
                case 10001:
                    ultDamage = (float)attackDamage * ((150f + 25f * Mathf.Pow((float)step, 2)) / 10f);
                    break;
                case 10002:
                    ultDamage = (float)attackDamage * ((120f + 20f * (float)step) / 10f);
                    break;
                case 10003:
                    ultDamage = (float)attackDamage * ((150f + 50f * (float)step) / 10f);
                    break;
                case 10004:
                    ultDamage = (float)attackDamage * ((130f + 30f * (float)step) / 10f);
                    break;
                case 10005:
                    ultDamage = (float)attackDamage * ((150f + (12.5f * Mathf.Pow((float)step, 2)) + (37.5f * (float)step)) / 10f);
                    break;
                case 10006:
                    ultDamage = (float)attackDamage * ((150f + (12.5f * Mathf.Pow(step, 2)) + (37.5f * (float)step)) / 10f);
                    break;
                default:
                    ultDamage = (float)attackDamage * ((150f + (50f * (float)step)) / 10f);
                    break;
            }
        }

        // 업그레이드 가능할 때만 실행
        public void LevelUp()
        {
            level++;

            int index = Manager.Game.CurrentSave.characterInventory.characters.FindIndex(c => c.id == id);
            CharacterSave characterSave = Manager.Game.CurrentSave.characterInventory.characters[index];
            characterSave.level = level;
            Manager.Game.CurrentSave.characterInventory.characters[index] = characterSave;
        }

        public void GetUpgradeUnit(int unit)
        {
            exp += unit;
            if (exp > upgradeUnit)
            {
                Debug.Log("업그레이드 가능합니다.");
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

                int index = Manager.Game.CurrentSave.characterInventory.characters.FindIndex(c => c.id == id);
                CharacterSave characterSave = Manager.Game.CurrentSave.characterInventory.characters[index];
                characterSave.step = step;
                Manager.Game.CurrentSave.characterInventory.characters[index] = characterSave;
            }
            else
            {
                Debug.Log("최대 단계에 도달했습니다.");
            }
        }

        public void UseParry(Parry subParry) // Parry subParry
        {
            // Parry 기능을 사용할 때마다 쿨타임을 체크하고 실행
            switch (subParry)
            {
                case Parry.방어막:
                    parrying.Parry();
                    parrying.Invicible();
                    break;
                case Parry.반사B:
                    parrying.Parry();
                    // 반사 기능 미구현으로 인해 무적으로 처리
                    parrying.Invicible();
                    break;
                case Parry.무적:
                    parrying.Parry();
                    parrying.Invicible();
                    break;
            }
        }

        public void UseUlt()
        {
            AudioManager.Instance.PlaySFX("Atk_Ultimate");
            switch (id)
            {
                case 10001:
                    ultimate.Laser(ultDamage);
                    break;
                case 10002:
                    // 유도탄 미구현 전탄 발사로 대체
                    ultimate.ManyBullets(ultDamage);
                    break;
                case 10003:
                    // 탄막 변경 데미지 증가
                    ultimate.BulletUpgrade();
                    break;
                case 10004:
                    // 궁극기 탄막 1회 - 다단히트
                    ultimate.BigBullet(ultDamage);
                    break;
                case 10005:
                    ultimate.Fire(ultDamage);
                    break;
                case 10006:
                    defense += ultimate.Shield(ultDamage);
                    break;
                default:
                    ultimate.AllAttack(ultDamage);
                    break;
            }
        }
    }
}
