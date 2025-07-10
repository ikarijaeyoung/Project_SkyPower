using KYG_skyPower;
using LJ2;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace JYL
{
    public class InvenPopUp : BaseUI
    {
        // 게임매니저에서 캐릭터 정보를 불러와야 함
        private string iconPrefabPath = "JYL/UI/ItemIconPrefab";
        private GameObject iconPrefab;
        private GameObject invenScroll => GetUI("InvenScroll");
        private TMP_Text invenCharName => GetUI<TMP_Text>("InvenCharNameText");
        private TMP_Text level => GetUI<TMP_Text>("InvenCharLevelText");
        private TMP_Text hp => GetUI<TMP_Text>("InvenCharHPText");
        private TMP_Text ap => GetUI<TMP_Text>("InvenCharAPText");
        private TMP_Text gold => GetUI<TMP_Text>("InvenUnitText");
        private RawImage apIcon => GetUI<RawImage>("WeaponBtn");
        private RawImage amIcon => GetUI<RawImage>("ArmorBtn");
        private RawImage acIcon => GetUI<RawImage>("AccessoryBtn");
        private RectTransform parent => GetUI<RectTransform>("Content");
        private Button wpEnhBtn => GetUI<Button>("WPEnhanceBtn1");
        private Button amEnhBtn => GetUI<Button>("AMEnhanceBtn2");
        private Image charImage;
        private CharacterSaveLoader characterLoader;
        private CharactorController mainController => characterLoader.mainController;
        private EquipController equipController;
        public static bool isInvenOpened = false;

        private List<GameObject> iconList;
        private Dictionary<GameObject, int> equipIdDict;
        private new void Awake()
        {
            base.Awake();
            characterLoader = GetComponent<CharacterSaveLoader>();
            equipController = GetComponent<EquipController>();

            Init();

        }
        private void OnEnable()
        {
            Init();
        }
        void Start()
        {

            // 장비 클릭시 활성화

            invenScroll.SetActive(false);
            GetEvent("CharEnhanceBtn").Click += OpenCharEnhance;
            GetEvent("WeaponBtn").Click += OpenWPInven;
            GetEvent("ArmorBtn").Click += OpenAMInven;
            GetEvent("AccessoryBtn").Click += OpenACInven;
            GetEvent("WPEnhanceBtn1").Click += OpenWPEnhance;
            GetEvent("AMEnhanceBtn2").Click += OpenAMEnhance;

            // 현재 캐릭터의 정보가 표시된다
            // index는 UIManager가 관리
            // GameManager.Instance.character[index]
            gameObject.GetComponent<PointerHandler>().Click += CloseInvenScroll;

            iconPrefab = Resources.Load<GameObject>(iconPrefabPath);

        }
        private void LateUpdate()
        {
            if (isInvenOpened)
            {
                Init();
                isInvenOpened = false;
            }
        }
        private void OnDisable()
        {
            Manager.Game.SaveGameProgress();
        }
        private void Init()
        {
            invenScroll.SetActive(false);
            if (iconList != null && iconList.Count > 0)
            {
                // 기존 리스트 클리어 및 다시 담기
                if (iconList.Count > 0)
                {
                    foreach (GameObject icon in iconList)
                    {
                        GameObject outIcon = DeleteFromDictionary(icon.gameObject.name, icon.gameObject);
                        Destroy(icon);
                        Debug.Log("아이콘 파괴중");
                    }
                    iconList.Clear();
                }
            }
            iconList = new List<GameObject>();
            equipIdDict = new Dictionary<GameObject, int>();
            equipController.Init();
            characterLoader.GetCharPrefab();
            charImage = GetUI<Image>("InvenCharImage");
            charImage.sprite = mainController.image;
            if (mainController.step == 0)
            {
                invenCharName.text = $"{mainController.charName}";
            }
            else
            {
                invenCharName.text = $"{mainController.charName} + {mainController.step}";
            }
            UpdateCharacterInfo();
            CreateEquipedIcons();
            CheckBtnEvent();
        }
        private void UpdateCharacterInfo()
        {
            level.text = $"{mainController.level}";
            hp.text = $"{mainController.Hp}";
            ap.text = $"{mainController.attackDamage}";
            gold.text = $"{Manager.Game.CurrentSave.gold}";
        }
        private void CreateInvenIcons(EquipType type)
        {

            // 기존 리스트 클리어 및 다시 담기
            if (iconList.Count > 0)
            {
                foreach (GameObject icon in iconList)
                {
                    GameObject outIcon = DeleteFromDictionary(icon.gameObject.name, icon.gameObject);
                    Destroy(icon);
                    Debug.Log("아이콘 파괴중");
                }
                iconList.Clear();
            }
            // 기존 딕셔너리 삭제 및 다시 만듦
            if (equipIdDict.Count > 0) { equipIdDict.Clear(); }

            List<EquipInfo> equipList = new List<EquipInfo>();
            equipList = equipController.GetEquipListByType(type);
            int index = 0;
            foreach (EquipInfo tmp in equipList)
            {
                if (tmp.level > 0)
                {
                    GameObject iconObj = Instantiate(iconPrefab, parent);
                    iconObj.name = $"ItemIconPrefab{index + 1}";
                    AddUIToDictionary(iconObj.gameObject);
                    Image iconImg = iconObj.GetComponentInChildren<Image>();
                    iconImg.sprite = tmp.icon;

                    if (tmp.id == Manager.Game.CurrentSave.wearingId[(int)type] && tmp.id != 0) // 현재 이 장비는 장착중!
                    {
                        Color color = iconImg.color;
                        color = Color.Lerp(color, Color.black, 0.3f);
                        iconImg.color = color;
                        Debug.Log($"이거 활성화 되는중{tmp.name}");
                        iconObj.GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(true);
                        iconList.Insert(0, iconObj); // 장착중인 것은 맨 앞에 오게 한다
                        iconObj.GetOrAddComponent<PointerHandler>().Click += ClickEquipIcon;
                        if (!equipIdDict.TryAdd(iconObj, tmp.id))
                        {
                            Debug.LogWarning($"이미 장비 딕셔너리에 있음 : GameObject({iconObj.name}) ID({tmp.id}) ");
                        }
                    }
                    else//장착중이지 않은 장비들
                    {
                        Debug.Log("장착중이 아님!");
                        iconObj.GetOrAddComponent<PointerHandler>().Click += ClickEquipIcon;
                        iconList.Add(iconObj);
                        if (!equipIdDict.TryAdd(iconObj, tmp.id))
                        {
                            Debug.LogWarning($"이미 장비 딕셔너리에 있음 : GameObject({iconObj.name}) ID({tmp.id}) ");
                        }
                    }
                }
                index++;
            }
        }

        private void CreateEquipedIcons()
        {
            Color tmpColor = Color.white;
            Color tmpColorOrigin = Color.white;
            tmpColorOrigin.a = 1f;
            tmpColor.a = 0f;
            if (equipController.weapon.id == 0) // 무기 장착 안되어 있음
            {
                apIcon.gameObject.GetComponentInChildren<Image>().color = tmpColor;
            }
            else
            {
                Image tmpImg = apIcon.gameObject.GetComponentInChildren<Image>();
                tmpImg.color = tmpColorOrigin;
                tmpImg.sprite = equipController.weapon.icon;
            }

            if (equipController.armor.id == 0) // 방어구 장착 안되어 있음
            {
                amIcon.gameObject.GetComponentInChildren<Image>().color = tmpColor;
            }
            else
            {
                Image tmpImg = amIcon.gameObject.GetComponentInChildren<Image>();
                tmpImg.color = tmpColorOrigin;
                tmpImg.sprite = equipController.armor.icon;
            }

            if (equipController.accessory.id == 0) // 악세사리 장착 안되어 있음
            {
                acIcon.gameObject.GetComponentInChildren<Image>().color = tmpColor;
            }
            else
            {
                Image tmpImg = acIcon.gameObject.GetComponentInChildren<Image>();
                tmpImg.color = tmpColorOrigin;
                tmpImg.sprite = equipController.accessory.icon;
            }

        }
        private void OpenWPInven(PointerEventData eventData)
        {
            if (invenScroll.activeSelf) invenScroll.SetActive(false);
            invenScroll.SetActive(true);
            CreateInvenIcons(EquipType.Weapon);
        }
        private void OpenAMInven(PointerEventData eventData)
        {
            if (invenScroll.activeSelf) invenScroll.SetActive(false);
            invenScroll.SetActive(true);
            CreateInvenIcons(EquipType.Armor);
        }
        private void OpenACInven(PointerEventData eventData)
        {
            if (invenScroll.activeSelf) invenScroll.SetActive(false);
            invenScroll.SetActive(true);
            CreateInvenIcons(EquipType.Accessory);
        }

        private void CloseInvenScroll(PointerEventData eventData)
        {
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, result);

            foreach (RaycastResult rs in result)
            {
                if (rs.gameObject == null) continue;

                if (rs.gameObject.transform.IsChildOf(invenScroll.transform))
                {
                    // 팝업 내부 클릭: 무시
                    return;
                }
            }

            invenScroll.SetActive(false);
        }

        private void ClickEquipIcon(PointerEventData eventData) // 장비 변경
        {
            if (!equipIdDict.TryGetValue(eventData.pointerClick, out int equipId))
            {
                Debug.LogWarning($"장비 딕셔너리에 해당 아이콘이 없음{eventData.pointerClick.name}");
            }
            Debug.Log("클릭됨");
            EquipInfo equipInfoTemp = equipController.FindEquip(equipId);
            ReplaceEquipment(equipInfoTemp.type, equipInfoTemp);
            Debug.Log($"지금 장착중인 장비{equipInfoTemp.type}_{equipController.armor.id}level{equipController.armor.level}");
            equipController.UpdateWearing();
            characterLoader.GetCharPrefab();
            CreateEquipedIcons();
            CreateInvenIcons(equipInfoTemp.type);
            UpdateCharacterInfo();
            CheckBtnEvent();
        }
        private void CheckBtnEvent()
        {
            if (equipController.weapon.level <= 0)
            {
                wpEnhBtn.gameObject.SetActive(false);
            }
            else
            {
                wpEnhBtn.gameObject.SetActive(true);
            }

            if (equipController.armor.level <= 0)
            {
                amEnhBtn.gameObject.SetActive(false);
            }
            else
            {
                amEnhBtn.gameObject.SetActive(true);
            }
        }
        private void ReplaceEquipment(EquipType type, EquipInfo equipInfo)
        {
            switch (type)
            {
                case EquipType.Weapon:
                    if (Manager.Game.CurrentSave.wearingId[0] != equipInfo.id)
                    {
                        Manager.Game.CurrentSave.wearingId[0] = equipInfo.id;
                    }
                    else
                    {
                        Manager.Game.CurrentSave.wearingId[0] = 0;
                    }
                    break;
                case EquipType.Armor:
                    if (Manager.Game.CurrentSave.wearingId[1] != equipInfo.id)
                    {
                        Manager.Game.CurrentSave.wearingId[1] = equipInfo.id;
                    }
                    else
                    {
                        Manager.Game.CurrentSave.wearingId[1] = 0;
                    }
                    break;
                case EquipType.Accessory:
                    if (Manager.Game.CurrentSave.wearingId[2] != equipInfo.id)
                    {
                        Manager.Game.CurrentSave.wearingId[2] = equipInfo.id;
                    }
                    else
                    {
                        Manager.Game.CurrentSave.wearingId[2] = 0;
                    }
                    break;
            }
        }

        private void OpenCharEnhance(PointerEventData eventData)
        {
            UIManager.selectIndexUI = 1;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

        private void OpenWPEnhance(PointerEventData eventData)
        {
            UIManager.selectIndexUI = 2;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

        private void OpenAMEnhance(PointerEventData eventData)
        {
            UIManager.selectIndexUI = 3;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

    }
}


