using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.UI;
using TMPro;
using KYG_skyPower;

namespace JYL
{
    public class TitlePresenter : BaseUI
    {
        public bool isTitleScene = true;
        private bool isAnyBtnPressed = false;
        void OnEnable() => Init();

        void OnDisable()
        {
            GetEvent("PressKeyBack").Click -= OnAnyBtnClick;
            UIManager.Instance.CleanPopUp();
            isTitleScene = false;
        }
        private void LateUpdate()
        {
            if(isAnyBtnPressed)
            {
                InputSystem.onAnyButtonPress.CallOnce(action => OnAnyBtnPress(action));
            }
        }
        private void Init()
        {
            InputSystem.onAnyButtonPress.CallOnce(action => OnAnyBtnPress(action));

            GetEvent("PressKeyBack").Click += OnAnyBtnClick;
        }

        private void OnAnyBtnPress(InputControl control)
        {
            if (!PopUpUI.IsPopUpActive&&isTitleScene)
            {
                // ��ư�� �ش��ϸ鼭 Ű���� �Ǵ� �����е� �Ǵ� ���콺 ���ʹ�ư �Է��� ���
                if (control is ButtonControl && (control.device is Keyboard || control.device is Gamepad gamepad))
                {
                    UIManager.Instance.ShowPopUp<SavePanel>();
                }
                isAnyBtnPressed = true;
            }
        }
        private void OnAnyBtnClick(PointerEventData eventData)
        {
            if (!PopUpUI.IsPopUpActive)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    UIManager.Instance.ShowPopUp<SavePanel>();
                }
            }
        }
    }
}
