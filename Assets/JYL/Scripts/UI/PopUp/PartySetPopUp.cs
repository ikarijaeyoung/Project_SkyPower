using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KYG_skyPower;
using TMPro;

namespace JYL
{
    public class PartySetPopUp : BaseUI
    {
        private event Action OnPartySetEnter;
        private string iconPrefabPath = "JYL/UI/CharacterIconPrefab";
        private CharacterSaveLoader characterLoader;
        private Image mainIllustImg;
        private Image sub1IllustImg;
        private Image sub2IllustImg;
        private Image iconPrefab;
        private RectTransform parent;
        private List<Image> iconList;
        private Dictionary<string, CharactorController> charDict;
        private List<CharacterSave> charDataList;
        private RectTransform dragIconTransform;
        private Vector2 originAnchoredPos;
        private RectTransform popUpPanel;
        private CanvasGroup canvasGroup;
        public static bool isPartySetting = false;
        private new void Awake()
        {
            base.Awake();
            Init();
        }
        private void OnEnable()
        {
            // 게임매니저의 세이브파일을 통해 캐릭터 리스트를 불러옴
            // 캐릭터 숫자 만큼 stayCharImg 생성 List.Length
            // 보유갯수 =0인 캐릭은 흐림 처리
            // 보유 중이면서, 파티에 편성된 캐릭은 회색 처리
            // 드래그&드랍으로 캐릭 편성
            // charImage1~3은 편성된 캐릭의 스프라이트 이미지를 가져옴
            //characterLoader.charactorController[0].image
           
        }
        void Start()
        {
            characterLoader = GetComponent<CharacterSaveLoader>();
            GetEvent("PSCharImg1").Click += OpenInvenPopUp;
            GetEvent("PSCharImg2").Click += OpenInvenPopUp;
            GetEvent("PSCharImg3").Click += OpenInvenPopUp;
            CreateIcons();
        }

        void Update() 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(isPartySetting)
                {
                    characterLoader.GetCharPrefab();
                    CreateIcons();
                    Util.ConsumeESC();
                    isPartySetting = false;
                }
            }
        }
        private void LateUpdate() { }
        private void Init()
        {
            charDict = new Dictionary<string, CharactorController>();
            charDataList = Manager.Game.saveFiles[Manager.Game.currentSaveIndex].characterInventory.characters;
            iconList = new List<Image>();
            characterLoader = GetComponent<CharacterSaveLoader>();
            //canvasGroup = GetComponent<CanvasGroup>();
            mainIllustImg = GetUI<Image>("PSCharImg1");
            sub1IllustImg = GetUI<Image>("PSCharImg2");
            sub2IllustImg = GetUI<Image>("PSCharImg3");
            parent = GetUI<RectTransform>("Content");
            //popUpPanel = GetUI<RectTransform>("PartySetPopUp");
            iconPrefab = Resources.Load<Image>(iconPrefabPath);
            characterLoader.GetCharPrefab();
        }
        private void OpenInvenPopUp(PointerEventData eventData)
        {
            // 선택된 캐릭을 기준으로 인벤을 연다
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int index);
            // GameManager.Instance.selectSave.party[index] -> 캐릭터 ID
            // 캐릭터 컨트롤러 (캐릭터 ID)
            UIManager.Instance.selectIndexUI = index;
            UIManager.Instance.ShowPopUp<InvenPopUp>();
        }
        private void CreateIcons()
        {
            if(iconList.Count>0)
            {
                foreach(Image icon in iconList)
                {
                    GameObject outIcon = DeleteFromDictionary(icon.gameObject.name,icon.gameObject);
                    Destroy(outIcon);
                }
                iconList.Clear();
                charDict.Clear();
            }

            int imgIndex = 0;
            foreach (CharactorController character in characterLoader.charactorController)
            {
                // 레벨 1 이상인 경우에만 이벤트 등록. 소유중인 캐릭터들임
                if (character.level >0)
                {
                    Image go;
                    go = Instantiate(iconPrefab, parent);
                    go.name = $"StayCharImg{imgIndex + 1}";
                    // TODO Add Test
                    AddUIToDictionary(go.gameObject);
                    imgIndex++;
                    go.sprite = character.image;
                    GetEvent($"{go.name}").Drag += BeginIconDrag;
                    GetEvent($"{go.name}").Drag += IconDrag;
                    GetEvent($"{go.name}").EndDrag += OnIconDragEnd;
                    iconList.Add(go);
                    if(!charDict.TryAdd(go.name, character))
                    {
                        Debug.LogWarning($"이미 charDict에 있음{go.name}");
                    }
                }
                //TODO: 에셋으로 아이콘과 일러스트가 들어오면 넣어줘야 함
                
                switch (character.partySet)
                {
                    case PartySet.Main:
                        mainIllustImg.sprite = character.image;
                        break;
                    case PartySet.Sub1:
                        sub1IllustImg.sprite = character.image;
                        break;
                    case PartySet.Sub2:
                        sub2IllustImg.sprite = character.image;
                        break;
                }
            }
        }
        // 아이콘 드래그 시작
        private void BeginIconDrag(PointerEventData eventData)
        {

            GameObject dragIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            dragIcon.transform.SetParent(transform.root);
            //canvasGroup.blocksRaycasts = false;
            dragIconTransform = dragIcon.GetComponent<RectTransform>();
            originAnchoredPos = dragIconTransform.anchoredPosition;
            isPartySetting = true;
        }

        // 드래그 중
        private void IconDrag(PointerEventData eventData)
        {
            GetUI($"{eventData.pointerDrag.gameObject.name}").transform.position = eventData.position;
        }

        //드래그 끝
        private void OnIconDragEnd(PointerEventData eventData)
        {
            GameObject selectedIcon = GetUI($"{eventData.pointerDrag.gameObject.name}");
            if (selectedIcon == null) return;

            //드롭 위치 UI 검색함
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = eventData.position;
            GraphicRaycaster raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
            raycaster.Raycast(ped, results); //ped 위치(드래그끝난위치)로 레이를 발사해서 리스트로 결과를 담음
            
            // 현재 아이콘의 값을 기준으로 캐릭터를 찾음
            charDict.TryGetValue($"{selectedIcon.name}", out CharactorController character); // 여기서 드래그중인 아이콘의 캐릭터 컨트롤러 나옴
            if (character == null)
            {
                Debug.LogWarning($"해당 캐릭터컨트롤러가 딕셔너리에 없음{selectedIcon.name}");
            }
            // 드래그하던 캐릭터의 정보
            CharacterSave dragCharData = charDataList.Find(c => c.id == character.id);
            int dragCharDataIndex = charDataList.FindIndex(c => c.id == character.id);

            // 모든 레이캐스트 결과를 비교
            foreach(RaycastResult result in results)
            {
                GameObject targetSlot = result.gameObject;
                // 만약, 해당 타겟UI의 태그가 "파티슬롯"이면 우리가 찾던 UI다.
                if(targetSlot.CompareTag("PartySlot"))
                {
                    Util.ExtractTrailNumber($"{targetSlot.name}",out int slotNum);
                    // UI 오브젝트의 이름을 통해 메인, 서브를 판별
                    if((int)dragCharData.partySet == slotNum)
                    {
                        ResetDragIcon(selectedIcon);
                        return;
                    }
                    switch(slotNum)
                    {
                        case 1: //메인
                            // 메인캐릭터 정보 가져오기
                            CharacterSave mainCharData = charDataList.Find(c => c.partySet == PartySet.Main);
                            int mainCharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Main);
                            
                            mainCharData.partySet = PartySet.None;
                            Debug.Log($"Main Index : {mainCharIndex}");
                            charDataList[mainCharIndex] = mainCharData;
                            
                            // 드래그 하던 애를 메인으로 올림
                            dragCharData.partySet = PartySet.Main;
                            charDataList[dragCharDataIndex] = dragCharData;
                            break;
                        case 2: //서브1
                            CharacterSave sub1CharData = charDataList.Find(c => c.partySet == PartySet.Sub1);
                            int sub1CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub1);
                            sub1CharData.partySet = PartySet.None;
                            Debug.Log($"Sub1 Index : {sub1CharIndex}");
                            charDataList[sub1CharIndex] = sub1CharData;

                            dragCharData.partySet = PartySet.Sub1;
                            charDataList[dragCharDataIndex] = dragCharData;
                            break;
                        case 3: //서브2
                            CharacterSave sub2CharData = charDataList.Find(c => c.partySet == PartySet.Sub2);
                            int sub2CharIndex = charDataList.FindIndex(c => c.partySet == PartySet.Sub2);
                            sub2CharData.partySet = PartySet.None;
                            Debug.Log($"Sub2 Index : {sub2CharIndex}");
                            charDataList[sub2CharIndex] = sub2CharData;


                            // 드래그 하던 애를 서브2로 올림
                            dragCharData.partySet = PartySet.Sub2;
                            charDataList[dragCharDataIndex] = dragCharData;
                            break;
                    }
                    // 여기서 캐릭터 컨트롤러들 최신화
                    // 최신화한 캐릭터 컨트롤러 정보들을 기준으로 UI 최신화

                    //// 이미지 컴포넌트 부착 시도 후 null체크. null이 아니면, 해당 UI 이름의 끝 숫자를 가져온다.
                    //Image image = targetSlot.GetComponent<Image>();
                    //if(image == null)
                    //{
                    //    Debug.LogWarning($"파티슬롯 게임오브젝트에 이미지 컴포넌트가 없음.");
                    //}
                }
                // 파티슬롯에 아이콘이 놓여지지 않았다
                else
                {
                    ResetDragIcon(selectedIcon);
                    isPartySetting = false;
                }
            }
            characterLoader.GetCharPrefab();
            CreateIcons();
            // 아이콘의 드래그가 끝난 지점이 "파티 지정 이미지"들 위면
            // if(같은 애면 아이콘을 원래 위치로 되돌림)
            // else //같은 애가 아니면
            // 해당 파티 지정 이미지를 아이콘의 캐릭터 일러스트로 대체함
            // 세이브 데이터에 접근해서 파티 구성 변경함
        }
        private void ResetDragIcon(GameObject icon)
        {
            icon.transform.SetParent(parent);
            icon.GetComponent<RectTransform>().anchoredPosition = originAnchoredPos;
        }
    }
}