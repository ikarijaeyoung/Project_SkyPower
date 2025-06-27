using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;
using UnityEngine;
using IO;

namespace JYL
{
    public class TitlePresenter : BaseUI
    {
        private GameObject titleImage;
        private GameObject saveImage;
        private GameObject savePanel;
        private GameObject saveFilePanel;
        private GameObject saveCreatePanel;

        void OnEnable() => Init();
        
        private void Update()
        {

        }
        void OnDisable()
        {
            GetEvent("PressKeyBack").Click -= OnAnyBtnClick;
            GetEvent("SaveBtn1").Click -= OnSaveSlotBtnClick;
            GetEvent("SaveBtn2").Click -= OnSaveSlotBtnClick;
            GetEvent("SaveBtn3").Click -= OnSaveSlotBtnClick;
        }

        private void Init()
        {
            for(int i = 0;i<3;i++)
            {
                //GetUI<TMP_Text>($"SaveText{i+1}").text = Manager.Game.saveFile[i].name;
            }
            titleImage = GetUI("TitleImage");
            saveImage = GetUI("SaveImage");
            savePanel = GetUI("SavePanel");
            saveFilePanel = GetUI("SaveFilePanel");
            saveCreatePanel = GetUI("SaveCreatePanel");

            InputSystem.onAnyButtonPress.Call(ctrl => OnAnyBtnPress(ctrl));

            GetEvent("PressKeyBack").Click += OnAnyBtnClick;
            GetEvent("SaveBtn1").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn2").Click += OnSaveSlotBtnClick;
            GetEvent("SaveBtn3").Click += OnSaveSlotBtnClick;
        }


        private void OnAnyBtnPress(InputControl control)
        {
            // 버튼에 해당하면서 키보드 또는 게임패드 또는 마우스 왼쪽버튼 입력일 경우
            if (control is ButtonControl && (control.device is Keyboard || control.device is Gamepad gamepad))
            {
                SavePanelPop();
            }
        }
        private void OnAnyBtnClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                SavePanelPop();
            }
            
        }
        private void OnSaveSlotBtnClick(PointerEventData eventData)
        {
            // TODO : 세이브 매니저에서 함수 완성 시 구현
            // string SaveManager.Instance.CheckSave() fd
            string name = eventData.pointerClick.name;
            char lastChar = name[name.Length - 1];
            Util.ExtractTrailNumber(name, out int index);
            // GameManager.saveIndex = index-1;

            // TODO: UI 트랜지션 테스트
            if (index == 0)
            {
                savePanel.SetActive(false);
                saveFilePanel.SetActive(true);
            }

        }
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
            titleImage.SetActive(false);
            saveImage.SetActive(true);
            // fade in
        }
    }
}
