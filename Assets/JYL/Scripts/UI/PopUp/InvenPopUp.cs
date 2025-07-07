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
            GetEvent("WPEnhanceBtn1").Click += OpenWPEnhance;
            GetEvent("ArmorBtn").Click += OpenAMInven;
            GetEvent("AMEnhanceBtn2").Click += OpenAMEnhance;
            GetEvent("AccessoryBtn").Click += OpenACInven;
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
        private void Init()
        {
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
        }
        private void UpdateCharacterInfo()
        {
            level.text = $"{mainController.level}";
            hp.text = $"{mainController.Hp}";
            ap.text = $"{mainController.attackDamage}";
            gold.text = $"{Manager.Game.CurrentSave.gold}";
        }
        private void CreateIcons(EquipType type)
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
                    
                    if(tmp.id == Manager.Game.CurrentSave.wearingId[(int)type]&&tmp.id != 0) // 현재 이 장비는 장착중!
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
                Debug.Log($"장착중이지 않음. 이미지 제거{equipController.weapon.id}");
                apIcon.gameObject.GetComponentInChildren<Image>().color = tmpColor;
            }
            else
            {
                Debug.Log($"장착중! 이미지 추가{equipController.weapon.id}");
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
            invenScroll.SetActive(true);
            CreateIcons(EquipType.Weapon);
        }
        private void OpenAMInven(PointerEventData eventData)
        {
            invenScroll.SetActive(true);
            CreateIcons(EquipType.Armor);
        }
        private void OpenACInven(PointerEventData eventData)
        {
            invenScroll.SetActive(true);
            CreateIcons(EquipType.Accessory);
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
            if(!equipIdDict.TryGetValue(eventData.pointerClick, out int equipId))
            {
                Debug.LogWarning($"장비 딕셔너리에 해당 아이콘이 없음{eventData.pointerClick.name}");
            }
            Debug.Log("클릭됨");
            EquipInfo equipInfoTemp = equipController.FindEquip(equipId);
            ReplaceEquipment(equipInfoTemp.type, equipInfoTemp);
            equipController.UpdateWearing();
            characterLoader.GetCharPrefab();
            CreateEquipedIcons();
            CreateIcons(equipInfoTemp.type);
            UpdateCharacterInfo();
        }
        private void ReplaceEquipment(EquipType type, EquipInfo equipInfo)
        {
            
            switch (type) // 1.무기 2.방어구 3.악세사리
            {
                case EquipType.Weapon:
                    if(Manager.Game.CurrentSave.wearingId[0] != equipInfo.id)
                    {
                        Manager.Game.CurrentSave.wearingId[0] = equipInfo.id; //들어온 장비로 교체
                    }
                    else
                    {
                        Manager.Game.CurrentSave.wearingId[0] = 0; // 장비 해제
                    }
                    break;
                case EquipType.Armor:
                    if(Manager.Game.CurrentSave.wearingId[1] != equipInfo.id)
                    {
                        Manager.Game.CurrentSave.wearingId[1] = equipInfo.id;
                    }
                    else
                    {
                        Manager.Game.CurrentSave.wearingId[1] = 0;
                    }
                    break;
                case EquipType.Accessory:
                    if(Manager.Game.CurrentSave.wearingId[2] != equipInfo.id)
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
            // 캐릭터 정보를 가지고 강화창 구현
            // UIManager에서 선택된 캐릭터의 인덱스 가지고 GameManager의 파티 구성원의 정보에 대한 캐릭터 컨트롤러 정보 불러옴
            // 해당 정보는 강화창에서 불러옴 여기서 안불러옴
            UIManager.selectIndexUI = 1;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
            // UI 생성할 때, UI에다가 이벤트 다세요.
            // Image img = Instantiate();
            // GetEvent($"img.gameObject.name").Click += 이벤트함수;
        }

        private void OpenWPEnhance(PointerEventData eventData)
        {
            UIManager.selectIndexUI = 2;
            // 현재 무기의 정보를 가져가야함
            // 선택하는 UI 정보들은 UIManager를 통해 접근한다.
            // GameManager.Instance.Party[0].
            // UIManager.Instance. 현재 선택한 캐릭의정보 + 무기 -> Enhance 팝업이 불러와야 함
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

        private void OpenAMEnhance(PointerEventData eventData)
        {
            UIManager.selectIndexUI = 3;
            UIManager.Instance.ShowPopUp<EnhancePopUp>();
        }

    }
}


