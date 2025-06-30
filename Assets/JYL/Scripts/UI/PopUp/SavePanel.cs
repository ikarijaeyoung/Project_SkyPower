using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
namespace JYL
{
    public class SavePanel : BaseUI
    {
        private void OnEnable()
        {
            // 게임매니저에서 세이브 파일을 Init함
            // GameManager.Instance.SaveInit();
        }
        void Start()
        {
            GetEvent("SaveBtn1").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn2").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn3").Click += OnSaveSlotBtnClick;
        }

        private void OnSaveSlotBtnClick(PointerEventData eventData)
        {
            // TODO : 세이브 매니저에서 함수 완성 시 구현
            // string SaveManager.Instance.CheckSave()
            string name = eventData.pointerClick.name;
            char lastChar = name[name.Length - 1];
            Util.ExtractTrailNumber(name, out int index);
            // GameManager.saveIndex = index-1;

            // TODO: UI 트랜지션 테스트
            switch(index)
            {
                case 1:
            UIManager.Instance.ShowPopUp<SaveFilePanel>();

                    break;
                case 2:
            UIManager.Instance.ShowPopUp<SaveCreatePanel>();
                    break;
                case 3:
            UIManager.Instance.ShowPopUp<SaveCreatePanel>();
                    break;
            }
        }
        // 여기서 세이브파일 정보들 불러와야 함
        private void SavePanelPop()
        {

            // 게임매니저 역할
            // CharacterSave[] saveFile = new CharacterSave[3]; - 3개의 배열로 가짐
            // Manager.Save.PlayerLoad(CharacterSave[index],index+1); - 게임매니저에서 3개의 파일 로드 시도
            // int saveIndex = 0~2; - 지정된 세이브 파일 인덱스
            // saveFile[saveIndex] - 현재 사용중인 세이브파일

            // UI 클릭
            // 요소 표시 할때 != null -> saveFile[index].name
            // ==null -> empty


            //TODO : 트렌지션 효과(ex.fade out)
            UIManager.Instance.ShowPopUp<SavePanel>();
            // fade in
        }
    }
}

