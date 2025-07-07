using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KYG_skyPower;
using UnityEngine.SceneManagement;

namespace JYL 
{
    public class PopUpUI : MonoBehaviour
    {
        // TODO: 팝업 되면 팝업 판넬 범위 바깥을 1. 클릭하지 못하게 막거나, 2. 클릭 시 팝업 이전으로 돌아가게 한다.
        private Stack<BaseUI> stack = new Stack<BaseUI>();
        public static bool IsPopUpActive { get; private set; } = false;

        [SerializeField] GameObject blocker;
        public void OnApplicationQuit()
        {
            DestroyImmediate(gameObject);
        }
        public void PushUIStack(BaseUI ui)
        {
            IsPopUpActive = true;
            if (stack.Count > 0)
            {
                BaseUI top = stack.Peek();
                top.gameObject.SetActive(false);
            }

            // Fix: Wrap the PlayBGM call in a lambda to match the Action<string> delegate type
            if(ui is PartySetPopUp)
            {
                AudioManager.Instance.PlayBGM("PartyBuild_BGM");
            }
            else if(ui is GameModePopUp)
            {
                AudioManager.Instance.PlayBGM("StageSelect_BGM");
            }
                stack.Push(ui);
            blocker.SetActive(true);
        }
        public void PopUIStack()
        {
            if (stack.Count == 1)
            {
                IsPopUpActive = false;
            }
            if (stack.Count <= 0)
            {
                IsPopUpActive = false;
                return;
            }

            Destroy(stack.Pop().gameObject);
            if (stack.Count > 0)
            {
                BaseUI top = stack.Peek();
                top.gameObject.SetActive(true);
            }
            else
            {
                string sceneName = SceneManager.GetActiveScene().name;
                string sceneBGM = "";
                switch(sceneName)
                {
                    case "aTitleScene_JYL":
                        sceneBGM = "StarMenu_BGM";
                        break;
                    case "bMainScene_JYL":
                        sceneBGM = "StarMenu_BGM";
                        break;
                    case "cStoreScene_JYL":
                        sceneBGM = "Shop_BGM";
                        break;
                    default:
                        sceneBGM = "";
                        break;
                }
                AudioManager.Instance.PlayBGM(sceneBGM);

                blocker.SetActive(false);
            }

        }
        public int StackCount()
        {
            return stack.Count;
        }
    }
}

