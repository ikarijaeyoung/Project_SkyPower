using KYG_skyPower;
using LJ2;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

namespace JYL
{
    public class EnhancePopUp : BaseUI
    {
        [SerializeField] GameObject lessUnitImg;
        [SerializeField] GameObject maxLevImg;
        private CharacterSaveLoader characterLoader;
        private EquipController equipController;
        private GameObject enhanceBtn => GetUI("EnhanceBtn");
        private Image enhanceTypeImg => GetUI<Image>("EnhanceMenuBack");
        private TMP_Text nowLevel => GetUI<TMP_Text>("NowLevelText");
        private TMP_Text afterLevel => GetUI<TMP_Text>("AfterLevelText");
        private TMP_Text nowHp => GetUI<TMP_Text>("NowHPText");
        private TMP_Text afterHp => GetUI<TMP_Text>("AfterHPText");
        private TMP_Text nowAp => GetUI<TMP_Text>("NowAPText");
        private TMP_Text afterAp => GetUI<TMP_Text>("AfterAPText");

        [SerializeField] private Sprite charEnhanceImg;
        [SerializeField] private Sprite wpEnhanceImg;
        [SerializeField] private Sprite amEnhanceImg;
        private void OnEnable()
        {
            Init();
        }
        private void OnDisable()
        {
            InvenPopUp.isInvenOpened = true;
            Manager.Game.SaveGameProgress();
        }
        private void Init()
        {
            lessUnitImg.SetActive(false);
            maxLevImg.SetActive(false);
            switch (UIManager.selectIndexUI)
            {
                case 1:
                    OnSelectChar();
                    break;
                case 2:
                case 3:
                    OnSelectEquip();
                    break;
            }
        }
        private void OnSelectChar() // 캐릭강화로 들어왓을 때
        {
            characterLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
            characterLoader.GetCharPrefab();
            enhanceTypeImg.sprite = charEnhanceImg;
            

            nowLevel.text = $"{characterLoader.mainController.level}";
            nowHp.text = $"{(int)(characterLoader.mainController.characterData.hp + (characterLoader.mainController.characterData.hpPlus * (characterLoader.mainController.level - 1)))}";
            nowAp.text = $"{(int)(characterLoader.mainController.characterData.attackDamage + (characterLoader.mainController.characterData.damagePlus * (characterLoader.mainController.level - 1)))}";
            
            // 최대레벨이 아니면 강화 가능
            if(characterLoader.mainController.level < characterLoader.mainController.characterData.maxLevel)
            {
                afterLevel.text = $"{characterLoader.mainController.level+1}";
                afterHp.text = $"{(int)(characterLoader.mainController.characterData.hp + (characterLoader.mainController.characterData.hpPlus * (characterLoader.mainController.level)))}";
                afterAp.text = $"{(int)(characterLoader.mainController.characterData.attackDamage + (characterLoader.mainController.characterData.damagePlus * (characterLoader.mainController.level)))}";
                // 재화 충분
                if(Manager.Game.CurrentSave.gold>= characterLoader.mainController.level * characterLoader.mainController.characterData.upgradeUnitPlus)
                {
                    enhanceBtn.SetActive(true);
                    GetEvent("EnhanceBtn").Click += CharacterEnhance;
                }
                // 재화 불충분
                else
                {
                    lessUnitImg.SetActive(true);
                    enhanceBtn.SetActive(false);
                }
            }
            // 최대레벨일 경우
            else
            {
                afterLevel.text = $"";
                afterHp.text = $"";
                afterAp.text = $"";
                enhanceBtn.SetActive(false);
                maxLevImg.SetActive(true);
            }
        }
        private void OnSelectEquip() // 장비 강화로 들어 왔을 때
        {
            equipController = gameObject.GetOrAddComponent<EquipController>();
            equipController.Init();
            equipController.UpdateEquipInfoBySave();
            
            switch(UIManager.selectIndexUI)
            {
                case 2: // 무기 강화의 경우
                    enhanceTypeImg.sprite = wpEnhanceImg;
                    nowLevel.text = $"{equipController.weapon.level}";
                    nowHp.text = $"";
                    nowAp.text = $"{equipController.weapon.equipValue}";
                    
                    // 레벨이 최대레벨보다 낮을 시
                    if (equipController.weapon.level < equipController.weapon.maxLevel)
                    {
                        afterLevel.text = $"{equipController.weapon.level + 1}";
                        afterHp.text = $"";
                        afterAp.text = $"{equipController.weapon.equipValue+ equipController.weapon.equipValuePlus}";
                        
                        // 재화가 충분할 시
                        if (Manager.Game.CurrentSave.gold >=equipController.weapon.level * equipController.weapon.upgradeGoldPlus)
                        {
                            enhanceBtn.SetActive(true);
                            GetEvent("EnhanceBtn").Click += EquipEnhance;
                        }
                        // 재화가 부족할 경우
                        else
                        {
                            enhanceBtn.SetActive(false);
                            lessUnitImg.SetActive(true);
                        }
                    }
                    // 최대레벨 이상일 때
                    else
                    {
                        afterLevel.text = $"";
                        afterHp.text = $"";
                        afterAp.text = $"";
                        enhanceBtn.SetActive(false);
                        maxLevImg.SetActive(true);
                    }
                    break;
                case 3: // 방어구
                    enhanceTypeImg.sprite = amEnhanceImg;
                    nowLevel.text = $"{equipController.armor.level}";
                    nowHp.text = $"{equipController.armor.equipValue}";
                    nowAp.text = $"";
                    
                    // 레벨이 최대레벨보다 낮을 경우
                    if (equipController.armor.level < equipController.armor.maxLevel)
                    {
                        afterLevel.text = $"{equipController.armor.level + 1}";
                        afterHp.text = $"{equipController.armor.equipValue + equipController.armor.equipValuePlus}";
                        afterAp.text = $"";
                        
                        // 골드가 충분할 경우
                        if (Manager.Game.CurrentSave.gold >= equipController.armor.level * equipController.armor.upgradeGoldPlus)
                        {
                            enhanceBtn.SetActive(true);
                            GetEvent("EnhanceBtn").Click += EquipEnhance;
                        }
                        else
                        {
                            enhanceBtn.SetActive(false);
                            lessUnitImg.SetActive(true);
                        }
                    }
                    // 최대 레벨 이상일 경우
                    else
                    {
                        afterLevel.text = $"";
                        afterHp.text = $"";
                        afterAp.text = $"";
                        enhanceBtn.SetActive(false);
                        maxLevImg.SetActive(true);
                    }
                    break;
            }
        }

        private void CharacterEnhance(PointerEventData eventData)
        {
            Manager.Game.CurrentSave.gold -= characterLoader.mainController.level * characterLoader.mainController.characterData.upgradeUnitPlus;
            characterLoader.mainController.LevelUp();
            Manager.Game.SaveGameProgress();
            UIManager.Instance.ClosePopUp();
        }
        private void EquipEnhance(PointerEventData eventData)
        {
            switch(UIManager.selectIndexUI)
            {
                case 2:
                    Manager.Game.CurrentSave.gold -= equipController.weapon.level * equipController.weapon.upgradeGoldPlus;
                    equipController.UpdateOneEquipInfo(equipController.weapon.id,true);
                    break;
                case 3:
                    Manager.Game.CurrentSave.gold -= equipController.armor.level * equipController.armor.upgradeGoldPlus;
                    equipController.UpdateOneEquipInfo(equipController.armor.id, true);
                     break;
            }
            Manager.Game.SaveGameProgress();
            UIManager.Instance.ClosePopUp();
        }
    }
}

