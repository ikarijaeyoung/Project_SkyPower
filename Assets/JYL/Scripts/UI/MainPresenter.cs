using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace JYL
{
    public class MainPresenter : BaseUI
    {
        private GameObject mainScreen;
        private RawImage charImg1;
        private RawImage charImg2;
        private RawImage charImg3;
        
        void Start()
        {
            mainScreen = GetUI("MainScreen");
            charImg1 = GetUI<RawImage>("CharImage1");
            charImg2 = GetUI<RawImage>("CharImage2");
            charImg3 = GetUI<RawImage>("CharImage3");
            
            GetEvent("ShopBtn").Click += OpenShop;
            GetEvent("PartySetBtn").Click += OpenPartySetting;
            GetEvent("PlayBtn").Click += OpenGameMode;
            GetEvent("InfoBtn").Click += OpenGameInfo;
        }
        private void OpenShop(PointerEventData eventData)
        {
            // TODO : 상점 구현
            // GameSceneManager.Instance.SceneChange("Shop");
        }
        private void OpenPartySetting(PointerEventData eventData)
        {
            UIManager.Instance.ShowPopUp<PartySetPopUp>();
        }
        private void OpenGameMode(PointerEventData eventData)
        {
            UIManager.Instance.ShowPopUp<GameModePopUp>();
        }
        private void OpenGameInfo(PointerEventData eventData)
        {
            // TODO : 후순위 구현 예정
        }
    }
}

