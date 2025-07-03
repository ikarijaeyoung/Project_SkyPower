using LJ2;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JYL
{
    public class PartySetPopUp : BaseUI
    {
        private CharacterSaveLoader characterLoader;
        private Image mainIllustImg;
        private Image sub1IllustImg;
        private Image sub2IllustImg;
        private Image iconPrefab;
        private RectTransform parent;
        private string iconPrefabPath = "JYL/UI/CharacterIconPrefab";
        private void Awake()
        {
            base.Awake();
            mainIllustImg = GetUI<Image>("PSCharImg1");
            sub1IllustImg = GetUI<Image>("PSCharImg2");
            sub2IllustImg = GetUI<Image>("PSCharImg3");
            characterLoader = GetComponent<CharacterSaveLoader>();
            characterLoader.GetCharPrefab();
            Array.Sort(characterLoader.charactorController, (a, b) => a.partySet.CompareTo(b.partySet)); // 추가적인 정렬도 가능함.
            iconPrefab = Resources.Load<Image>(iconPrefabPath);
            parent = GetUI<RectTransform>("Content");
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
            int imgIndex = 0;
            foreach (CharactorController character in characterLoader.charactorController)
            {
                Image go = Instantiate(iconPrefab, parent);
                go.name = $"StayCharImg{imgIndex + 1}";
                imgIndex++;
                IconSetUp(go, character);
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
            }
        }
        void Start()
        {
            characterLoader = GetComponent<CharacterSaveLoader>();
            GetEvent("PSCharImg1").Click += OpenInvenPopUp;
            GetEvent("PSCharImg2").Click += OpenInvenPopUp;
            GetEvent("PSCharImg3").Click += OpenInvenPopUp;
        }

        void Update() { }
        private void OpenInvenPopUp(PointerEventData eventData)
        {
            // 선택된 캐릭을 기준으로 인벤을 연다
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int index);
            // GameManager.Instance.selectSave.party[index] -> 캐릭터 ID
            // 캐릭터 컨트롤러 (캐릭터 ID)
            Debug.Log($"{index}");
            UIManager.Instance.ShowPopUp<InvenPopUp>();
        }
        private void IconSetUp(Image image, CharactorController characterData)
        {
            image.sprite = characterData.image;

        }

    }
}

