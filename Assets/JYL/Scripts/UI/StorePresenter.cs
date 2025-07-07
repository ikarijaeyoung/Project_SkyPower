using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using KYG_skyPower;
using TMPro;
using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;


namespace JYL
{
    public class StorePresenter : BaseUI
    {

        private TMP_Text unitText => GetUI<TMP_Text>("StoreUnitText");
        private CharacterSaveLoader characterLoader;
        private EquipController equipController;
        void Start()
        {

            GetEvent("GachaChrBtn1").Click += CharGachaClick;
            GetEvent("GachaChrBtn5").Click += CharGachaClick;
            GetEvent("GachaEquipBtn1").Click += EquipGachaClick;
            GetEvent("GachaEquipBtn5").Click += EquipGachaClick;
            //GetEvent("StoreItemImg").Click += ItemStore;
            characterLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
            equipController = gameObject.GetOrAddComponent<EquipController>();
            characterLoader.GetCharPrefab(); // 이큅컨트롤러가 먼저 초기화 된다.
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !PopUpUI.IsPopUpActive && !Util.escPressed)
            {
                // 씬 전환
                SceneManager.LoadSceneAsync("bMainScene_JYL");
                Util.ConsumeESC();
            }
        }

        private void CharGachaClick(PointerEventData eventData)
        {
            // TODO : 가챠 수행과 동시에, 인벤토리(캐릭터 목록)에 추가. 게임매니저에서 세이브함
            // 저장 완료 후, 가챠 연출 진행. 연출 완료후 결과 팝업 창을 띄움
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int num);
            switch (num)
            {
                case 1:
                    if (Manager.Game.CurrentSave.gold >= 100)
                    {
                        Manager.Game.CurrentSave.gold -= 100;
                        
                        
                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<GachaPopUp>();
                    }
                    break;
                case 5:
                    if(Manager.Game.CurrentSave.gold >= 500)
                    {
                        Manager.Game.CurrentSave.gold -= 500;
                        
                        
                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<Gacha5PopUp>();
                    }
                    break;
            }
        }
        private void EquipGachaClick(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.name, out int num);
            switch (num)
            {
                case 1:
                    if (Manager.Game.CurrentSave.gold >= 100)
                    {
                        Manager.Game.CurrentSave.gold -= 100;


                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<GachaPopUp>();
                    }
                    break;
                case 5:
                    if (Manager.Game.CurrentSave.gold >= 500)
                    {
                        Manager.Game.CurrentSave.gold -= 500;


                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<Gacha5PopUp>();
                    }
                    break;
            }
        }
        private List<CharactorController> CharGachaRoll(int num)
        {
            List<CharactorController> results = new List<CharactorController>(num);
            return null;
        }
        private List<EquipInfo> EquipGachaRoll(int num)
        {
            List<EquipInfo> results = new List<EquipInfo>(num);
            return null;

        }
        //// TODO : 아이템 추가 시 작업
        //private void ItemStore(PointerEventData eventData)
        //{

        //}
    }
}


