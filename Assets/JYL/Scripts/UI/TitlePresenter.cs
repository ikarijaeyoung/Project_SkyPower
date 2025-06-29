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

        void OnEnable() => Init();

        void OnDisable()
        {
            GetEvent("PressKeyBack").Click -= OnAnyBtnClick;
        }

        private void Init()
        {
            for(int i = 0;i<3;i++)
            {
                //GetUI<TMP_Text>($"SaveText{i+1}").text = Manager.Game.saveFile[i].name;
            }
            titleImage = GetUI("TitleImage");

            InputSystem.onAnyButtonPress.Call(ctrl => OnAnyBtnPress(ctrl));

            GetEvent("PressKeyBack").Click += OnAnyBtnClick;
        }


        private void OnAnyBtnPress(InputControl control)
        {
            // 버튼에 해당하면서 키보드 또는 게임패드 또는 마우스 왼쪽버튼 입력일 경우
            if (control is ButtonControl && (control.device is Keyboard || control.device is Gamepad gamepad))
            {
                UIManager.Instance.ShowPopUp<SavePanel>();
            }
        }
        private void OnAnyBtnClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                UIManager.Instance.ShowPopUp<SavePanel>();
            }
        }
    }
}
