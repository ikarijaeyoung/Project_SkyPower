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
                        Debug.LogWarning($"ÇØ´ç °æ·Î¿¡ ÆË¾÷ ÇÁ¸®ÆÕÀÌ ¾øÀ½: {popUpPath}");
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

        // ¼±ÅÃ UI ÀÎµ¦½º
        public int selectIndexUI = 0;

        protected override void Awake() => base.Awake();
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && PopUpUI.IsPopUpActive && !Util.escPressed)
            {
                Instance.ClosePopUp();
                Util.ConsumeESC();
            }
        }
        private void LateUpdate()
        {
            Util.ResetESC();
        }

        // ÆË¾÷ UI¸¦ ²¨³½´Ù
        public T ShowPopUp<T>() where T : BaseUI
        {
            string path = $"{prefabPath}/{typeof(T).Name}";
            T prefab = Resources.Load<T>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"ÇØ´ç °æ·Î¿¡ ÆË¾÷ ÇÁ¸®ÆÕ ¾øÀ½: {path}");
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