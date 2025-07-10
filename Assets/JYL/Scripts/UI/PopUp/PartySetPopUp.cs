using KYG_skyPower;
using LJ2;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace JYL
{

    public class PartySetPopUp : BaseUI
    {
        private string iconPrefabPath = "JYL/UI/CharacterIconPrefab";
        private CharacterSaveLoader characterLoader;
        private Image mainIllustImg;
        private Image sub1IllustImg;
        private Image sub2IllustImg;
        private Image[] psNImg;
        private Image[] psSSRImg;

        private GameObject iconPrefab;
        private RectTransform parent;
        private List<GameObject> iconList;
        private Dictionary<string, CharactorController> charDict;
        private List<CharacterSave> charDataList;
        public static bool isPartySetting = false;
        private bool isMainSet = false;
        private bool isSub1Set = false;
        private bool isSub2Set = false;

        private TMP_Text warningText;
        private new void Awake()
        {
            base.Awake();
            Init();
        }
        private void OnEnable() { }
        void Start()
        {
            GetEvent("PSNImg1").Click += OpenInvenPopUp;
            GetEvent("PSSSRImg1").Click += OpenInvenPopUp;
            CreateIcons();
            warningText.gameObject.SetActive(false);
        }

        void Update()
        {
            // ��Ƽ ���� ���̸�
            if (isPartySetting)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    characterLoader.GetCharPrefab();
                    CreateIcons();
                    Util.ConsumeESC();
                    isPartySetting = false;
                }
                warningText.gameObject.SetActive(true);
            }
            // ��� ��Ƽ ���� �Ϸ�
            if (isMainSet && isSub1Set && isSub2Set)
            {
                warningText.gameObject.SetActive(false);
            }
            else
            {
                isPartySetting = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Util.ConsumeESC();
                }
                warningText.gameObject.SetActive(true);
            }
        }

        // �ʱ�ȭ
        private void Init()
        {
            psNImg = new Image[3];
            psSSRImg = new Image[3];
            for(int i = 0; i<3;i++)
            {
                psNImg[i] = GetUI<Image>($"PSNImg{i+1}");
                psSSRImg[i] = GetUI<Image>($"PSSSRImg{i+1}");
            }
            characterLoader = GetComponent<CharacterSaveLoader>();
            charDict = new Dictionary<string, CharactorController>();
            charDataList = Manager.Game.CurrentSave.characterInventory.characters;
            iconList = new List<GameObject>();
            //canvasGroup = GetComponent<CanvasGroup>();
            mainIllustImg = GetUI<Image>("PSCharImg1");
            sub1IllustImg = GetUI<Image>("PSCharImg2");
            sub2IllustImg = GetUI<Image>("PSCharImg3");
            parent = GetUI<RectTransform>("Content");
            warningText = GetUI<TMP_Text>("PartySetWarningText");
            //popUpPanel = GetUI<RectTransform>("PartySetPopUp");
            iconPrefab = Resources.Load<GameObject>(iconPrefabPath);
            characterLoader.GetCharPrefab();
        }

        // ���â ����
        private void OpenInvenPopUp(PointerEventData eventData)
        {
            // ���õ� ĳ���� �������� �κ��� ����
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int index);
            // GameManager.Instance.selectSave.party[index] -> ĳ���� ID
            // ĳ���� ��Ʈ�ѷ� (ĳ���� ID)
            UIManager.selectIndexUI = index;
            if(index == 1)
            {
                int mainCharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Main);
                if (mainCharIndex == -1)
                {
                    Debug.Log($"���� ĳ���ʹ� ���� �������");
                }
                else if(!isPartySetting)
                {
                    UIManager.Instance.ShowPopUp<InvenPopUp>();
                }
            }
        }

        // ������ �� �Ϸ���Ʈ���� ����
        private void CreateIcons()
        {
            if (iconList.Count > 0)
            {
                foreach (GameObject icon in iconList)
                {
                    GameObject outIcon = DeleteFromDictionary(icon.gameObject.name, icon.gameObject);
                    Destroy(outIcon);
                }
                iconList.Clear();
                charDict.Clear();
            }

            int imgIndex = 0;

            isMainSet = false;
            isSub1Set = false;
            isSub2Set = false;

            foreach (CharactorController character in characterLoader.charactorController)
            {
                // ���� 1 �̻��� ��쿡�� �̺�Ʈ ���. �������� ĳ���͵���
                if (character.level > 0)
                {
                    GameObject go;
                    go = Instantiate(iconPrefab, parent);
                    go.name = $"StayCharImg{imgIndex + 1}";
                    AddUIToDictionary(go.gameObject);
                    imgIndex++;
                    
                    Image[] tmp = go.GetComponentsInChildren<Image>(true);
                    foreach (Image image in tmp)
                    {
                        if(image.gameObject.name == "CharIconImg")
                        {
                            image.sprite = character.icon;
                        }
                        if(character.grade == Grade.R)
                        {
                            if(image.gameObject.name == "RFrameImg")
                            {
                                image.gameObject.SetActive(true);
                            }
                            
                        }
                        else if(character.grade == Grade.SSR)
                        {
                            if (image.gameObject.name == "SSRFrameImg")
                            {
                                image.gameObject.SetActive(true);
                            }
                        }
                    }
                    //if (character.grade == Grade.R)
                    //{
                    //    frame.rFrame.gameObject.SetActive(true);
                    //    frame.ssrFrame.gameObject.SetActive(false);
                    //}
                    //else if (character.grade == Grade.SSR)
                    //{
                    //    frame.rFrame.gameObject.SetActive(false);
                    //    frame.ssrFrame.gameObject.SetActive(true);
                    //}

                    //CharacterIconPrefab frame = go.GetComponent<CharacterIconPrefab>();
                    //frame.Init();
                    //frame.charImg.sprite = character.icon;
                    //fra
                    //if(character.grade==Grade.R)
                    //{
                    //    frame.rFrame.gameObject.SetActive(true);
                    //    frame.ssrFrame.gameObject.SetActive(false);
                    //}
                    //else if(character.grade == Grade.SSR)
                    //{
                    //    frame.rFrame.gameObject.SetActive(false);
                    //    frame.ssrFrame.gameObject.SetActive(true);
                    //}

                    //go.GetComponentInChildren<Image>().sprite = character.icon;
                    GetEvent($"{go.name}").BeginDrag += BeginIconDrag;
                    GetEvent($"{go.name}").Drag += IconDrag;
                    GetEvent($"{go.name}").EndDrag += OnIconDragEnd;
                    iconList.Add(go);
                    if (!charDict.TryAdd(go.name, character))
                    {
                        Debug.LogWarning($"�̹� charDict�� ����{go.name}");
                    }
                }
                //TODO: �������� �����ܰ� �Ϸ���Ʈ�� ������ �־���� ��

                switch (character.partySet)
                {
                    case PartySet.Main:
                        mainIllustImg.sprite = character.image;
                        if(character.grade == Grade.R)
                        {
                            psNImg[0].gameObject.SetActive(true);
                            psSSRImg[0].gameObject.SetActive(false);
                        }
                        else if(character.grade == Grade.SSR)
                        {
                            psNImg[0].gameObject.SetActive(false);
                            psSSRImg[0].gameObject.SetActive(true);
                        }
                            isMainSet = true;
                        break;
                    case PartySet.Sub1:
                        sub1IllustImg.sprite = character.image;
                        if (character.grade == Grade.R)
                        {
                            psNImg[1].gameObject.SetActive(true);
                            psSSRImg[1].gameObject.SetActive(false);
                        }
                        else if (character.grade == Grade.SSR)
                        {
                            psNImg[1].gameObject.SetActive(false);
                            psSSRImg[1].gameObject.SetActive(true);
                        }
                        isSub1Set = true;
                        break;
                    case PartySet.Sub2:
                        sub2IllustImg.sprite = character.image;
                        if (character.grade == Grade.R)
                        {
                            psNImg[2].gameObject.SetActive(true);
                            psSSRImg[2].gameObject.SetActive(false);
                        }
                        else if (character.grade == Grade.SSR)
                        {
                            psNImg[2].gameObject.SetActive(false);
                            psSSRImg[2].gameObject.SetActive(true);
                        }
                        isSub2Set = true;
                        break;
                }
            }
            CheckPartySlotNull();
 
        }
        private void CheckPartySlotNull()
        {
            // ����
            if (!isMainSet)
            {
                mainIllustImg.sprite = null;
                Color c = mainIllustImg.color;
                c.a = 0f;
                mainIllustImg.color = c;
            }
            else
            {
                Color c = mainIllustImg.color;
                c.a = 1f;
                mainIllustImg.color = c;
            }
            // ����1
            if (!isSub1Set)
            {
                sub1IllustImg.sprite = null;
                Color c = sub1IllustImg.color;
                c.a = 0f;
                sub1IllustImg.color = c;
            }
            else
            {
                Color c = sub1IllustImg.color;
                c.a = 1f;
                sub1IllustImg.color = c;
            }
            // ����2
            if (!isSub2Set)
            {
                sub2IllustImg.sprite = null;
                Color c = sub2IllustImg.color;
                c.a = 0f;
                sub2IllustImg.color = c;
            }
            else
            {
                Color c = sub2IllustImg.color;
                c.a = 1f;
                sub2IllustImg.color = c;
            }
        }
        // ������ �巡�� ����
        private void BeginIconDrag(PointerEventData eventData)
        {
            GameObject dragIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            dragIcon.transform.SetParent(transform.root);
            isPartySetting = true;
        }

        // �巡�� ��
        private void IconDrag(PointerEventData eventData)
        {
            GetUI($"{eventData.pointerDrag.gameObject.name}").transform.position = eventData.position;
        }

        //�巡�� ��
        private void OnIconDragEnd(PointerEventData eventData)
        {
            GameObject selectedIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            if (selectedIcon == null) return;

            //��� ��ġ�� UI���� �˻���
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = eventData.position;
            GraphicRaycaster raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
            raycaster.Raycast(ped, results); //ped ��ġ(�巡�׳�����ġ)�� ���̸� �߻��ؼ� ����Ʈ�� ����� ����

            // ���� �������� ���� �������� ĳ���͸� ã��
            charDict.TryGetValue($"{selectedIcon.name}", out CharactorController character); // ���⼭ �巡������ �������� ĳ���� ��Ʈ�ѷ� ����
            if (character == null)
            {
                Debug.LogWarning($"�巡�� ���� �������� ĳ������Ʈ�ѷ��� ��ųʸ��� ����{selectedIcon.name}");
            }
            // �巡���ϴ� ĳ������ ����
            CharacterSave dragCharData = charDataList.Find(c => c.id == character.id);
            int dragCharDataIndex = charDataList.FindIndex(c => c.id == character.id);

            // ��� ����ĳ��Ʈ ����� ��
            foreach (RaycastResult result in results)
            {
                GameObject targetSlot = result.gameObject;
                // ����, �ش� Ÿ��UI�� �±װ� "��Ƽ����"�̸� �츮�� ã�� UI��.
                if (targetSlot.CompareTag("PartySlot"))
                {
                    // UI ������Ʈ�� �̸��� ���� ����, ���긦 �Ǻ�
                    Util.ExtractTrailNumber($"{targetSlot.name}", out int slotNum);
                    // ����, �巡�� ���ΰͰ� �������°��� ���ٸ� �۾��� ���� �ʴ´�
                    if ((int)dragCharData.partySet+1 != slotNum)
                    {
                        switch (slotNum)
                        {
                            case 1: //����
                                    // ����ĳ���� ���� ��������
                                CharacterSave mainCharData = charDataList.Find(c => c.partySet == PartySet.Main);
                                int mainCharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Main);
                                if (mainCharIndex == -1)
                                {
                                    Debug.Log($"���� ĳ���ʹ� ����־���");
                                }
                                else
                                {
                                    mainCharData.partySet = PartySet.None;
                                    charDataList[mainCharIndex] = mainCharData;
                                }

                                // �巡�� �ϴ� �ָ� �������� �ø�
                                dragCharData.partySet = PartySet.Main;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                            case 2: //����1
                                CharacterSave sub1CharData = charDataList.Find(c => c.partySet == PartySet.Sub1);
                                int sub1CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub1);
                                if (sub1CharIndex == -1)
                                {
                                    Debug.Log($"����1 ĳ���ʹ� ����־���");
                                }
                                else
                                {
                                    sub1CharData.partySet = PartySet.None;
                                    charDataList[sub1CharIndex] = sub1CharData;
                                }

                                dragCharData.partySet = PartySet.Sub1;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                            case 3: //����2
                                CharacterSave sub2CharData = charDataList.Find(c => c.partySet == PartySet.Sub2);
                                int sub2CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub2);
                                if (sub2CharIndex == -1)
                                {
                                    Debug.Log($"����2 ĳ���ʹ� ����־���");
                                }
                                else
                                {
                                    sub2CharData.partySet = PartySet.None;
                                    charDataList[sub2CharIndex] = sub2CharData;
                                }

                                dragCharData.partySet = PartySet.Sub2;
                                charDataList[dragCharDataIndex] = dragCharData;
                                break;
                        }
                    }
                }
            }

            // �Լ� ���� ������ �ٽ� �ʱ�ȭ ��
            characterLoader.GetCharPrefab();
            CreateIcons();
            isPartySetting = false;
        }
    }
}