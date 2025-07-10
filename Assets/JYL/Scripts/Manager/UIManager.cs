using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] string popUpPath = "JYL/UI/Canvas_PopUp";
        [SerializeField] string prefabPath = "JYL/UI";
        private PopUpUI popUp;
        public PopUpUI PopUp
        {
            get 
            {
                if (popUp == null)
                {
                    popUp = FindObjectOfType<PopUpUI>();
                    if (popUp != null) return popUp;
                    
                    PopUpUI prefab = Resources.Load<PopUpUI>(popUpPath);
                    if (prefab == null)
                    { 
                        Debug.LogWarning($"�ش� ��ο� �˾� �������� ����: {popUpPath}");
                        return null;
                    }
                    PopUpUI go = Instantiate(prefab);
                    DontDestroyOnLoad(go);
                    return go;
                }
                DontDestroyOnLoad(popUp);
                return popUp;
            }
        }

        // ���� UI �ε���
        public static int selectIndexUI { get; set; } = 0;
        public static bool canClosePopUp = true;
        bool canClose => PopUpUI.IsPopUpActive && !Util.escPressed && !PartySetPopUp.isPartySetting & canClosePopUp;

        protected override void Awake() => base.Awake();
        private void Update()
        {
        }
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && canClose)
            {
                Instance.ClosePopUp();
                Util.ConsumeESC();
            }
            Util.ResetESC();
        }

        // �˾� UI�� ������
        public T ShowPopUp<T>() where T : BaseUI
        {
            string path = $"{prefabPath}/{typeof(T).Name}";
            T prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"�ش� ��ο� �˾� ������ ����: {path}");
                return null;
            }
            T instance = Instantiate(prefab, PopUp.transform);
            PopUp.PushUIStack(instance);
            return instance;
        }
        public void ClosePopUp()
        {
            PopUp.PopUIStack();
        }
        public void CleanPopUp()
        {
            while(true)
            {
                PopUp.PopUIStack();
                if (PopUp.StackCount() == 0) break;
            }
        }
    }
}