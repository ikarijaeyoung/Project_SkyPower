using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JYL
{
    public class PartySetPopUp : BaseUI
    {

        private RawImage charImg1;
        private RawImage charImg2;
        private RawImage charImg3;

        private List<RawImage> stayCharImg;
        // Start is called before the first frame update
        private void OnEnable()
        {
            // 게임매니저의 세이브파일을 통해 캐릭터 리스트를 불러옴
            // 캐릭터 숫자 만큼 stayCharImg 생성 List.Length
            // 보유갯수 =0인 캐릭은 흐림 처리
            // 보유 중이면서, 파티에 편성된 캐릭은 회색 처리
            // 드래그&드랍으로 캐릭 편성
            // charImage1~3은 편성된 캐릭의 스프라이트 이미지를 가져옴

        }
        void Start()
        {
            // 
            GetEvent("PartyCharImage1").Click += OpenInvenPopUp;
            GetEvent("PartyCharImage2").Click += OpenInvenPopUp;
            GetEvent("PartyCharImage3").Click += OpenInvenPopUp;
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OpenInvenPopUp(PointerEventData eventData)
        {
            // 선택된 캐릭을 기준으로 인벤을 연다
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int index);
            // GameManager.Instance.selectSave.party[index] -> 캐릭터 ID
            // 캐릭터 컨트롤러 (캐릭터 ID)
            Debug.Log($"{index}");
            UIManager.Instance.ShowPopUp<InvenPopUp>();
        }
    }
}

