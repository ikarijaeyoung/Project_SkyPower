using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        void Update() { }
        private void LateUpdate()
        {

        }
        private void Init()
        {
            charDict = new Dictionary<string, CharactorController>();
            iconList = new List<Image>();
            characterLoader = GetComponent<CharacterSaveLoader>();
            mainIllustImg = GetUI<Image>("PSCharImg1");
            sub1IllustImg = GetUI<Image>("PSCharImg2");
            sub2IllustImg = GetUI<Image>("PSCharImg3");
            parent = GetUI<RectTransform>("Content");
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
                foreach(var icon in iconList)
                {
                    Destroy(icon);
                }
                iconList.Clear();
            }

            int imgIndex = 0;
            foreach (CharactorController character in characterLoader.charactorController)
            {
                Image go = Instantiate(iconPrefab, parent);
                go.name = $"StayCharImg{imgIndex + 1}";
                imgIndex++;
                go.sprite = character.image;
                //TODO: 에셋으로 아이콘과 일러스트가 들어오면 넣어줘야 함
                if (character.level < 0)
                {
                    // 캐릭터가 레벨 1보다 작으면 미소유 캐릭터
                    go.gameObject.SetActive(false);
                }
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
                GetEvent($"{go.name}").Drag += IconDrag;
                //GetEvent($"{go.name}").EndDrag +=;
                iconList.Add(go);
                charDict.Add(go.name, character);
            }
        }
        private void IconDrag(PointerEventData eventData)
        {
            GetUI($"{eventData.pointerDrag.gameObject.name}").transform.position = eventData.position;
        }
        private void OnIconDragEnd(PointerEventData eventData)
        {
            // 아이콘의 드래그가 끝난 지점이 "파티 지정 이미지"들 위면
            // if(같은 애면 아이콘을 원래 위치로 되돌림)
            // else //같은 애가 아니면
            // 해당 파티 지정 이미지를 아이콘의 캐릭터 일러스트로 대체함
            // 세이브 데이터에 접근해서 파티 구성 변경함
        }
    }
}