using KYG_skyPower;
using LJ2;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace JYL
{
    public class StorePresenter : BaseUI
    {

        private TMP_Text unitText => GetUI<TMP_Text>("StoreUnitText");
        private CharacterSaveLoader characterLoader;
        private EquipController equipController;
        private List<GachaData> charGachaList;
        private List<GachaData> equipGachaList;
        public static List<int> gachaResult;
        public static bool isStoreReturn = false;
        public static bool isCharGacha = false;

        [Header("Set CSV")]
        [SerializeField]private string charGachaCsvPath = "CSV/Char_GachaTable";
        [SerializeField]private string equipGachaCsvPath = "CSV/Equipt_GachaTable";
        [SerializeField]private char splitSymbol = ',';
        
        private float charTotalRate = 0f;
        private float equipTotalRate = 0f;

        void Start()
        {
            unitText.text = $"{Manager.Game.CurrentSave.gold}";
            gachaResult = new List<int>();
            charGachaList = new List<GachaData>();
            equipGachaList = new List<GachaData>();
            GetEvent("GachaChrBtn1").Click += CharGachaClick;
            GetEvent("GachaChrBtn5").Click += CharGachaClick;
            GetEvent("GachaEquipBtn1").Click += EquipGachaClick;
            GetEvent("GachaEquipBtn5").Click += EquipGachaClick;
            //GetEvent("StoreItemImg").Click += ItemStore;
            characterLoader = gameObject.GetOrAddComponent<CharacterSaveLoader>();
            equipController = gameObject.GetOrAddComponent<EquipController>();
            characterLoader.GetCharPrefab(); // 이큅컨트롤러가 먼저 초기화 된다.
            GetCSVTable();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !PopUpUI.IsPopUpActive && !Util.escPressed)
            {
                // 씬 전환
                SceneManager.LoadSceneAsync("bMainScene_JYL");
                Util.ConsumeESC();
            }
            if(isStoreReturn)
            {
                ResetState();
                isStoreReturn = false;
            }
        }

        private void ResetState()
        {
            if(gachaResult.Count>0)
            {
                gachaResult.Clear();
            }
            else if(gachaResult == null)
            {
                gachaResult = new List<int>();
            }
            characterLoader.GetCharPrefab();
            if (isStoreReturn) isStoreReturn = false;
            unitText.text = $"{Manager.Game.CurrentSave.gold}";
        }
        private void GetCSVTable()
        {
            TextAsset charCsvFile = Resources.Load<TextAsset>(charGachaCsvPath);
            TextAsset equipCsvFile = Resources.Load<TextAsset>(equipGachaCsvPath);
            if (charCsvFile == null)
            {
                Debug.LogError($"CSV파일을 못찾음{charGachaCsvPath}");
            }
            if (equipCsvFile == null)
            {
                Debug.LogError($"CSV파일을 못찾음{equipGachaCsvPath}");
            }

            string[] charLines = charCsvFile.text.Split('\n');
            for(int i =2;i<charLines.Length;i++)
            {
                if (string.IsNullOrWhiteSpace(charLines[i])) continue; //널이거나 빈칸이면(엑셀) 넘어감

                string[] tokens = charLines[i].Split(splitSymbol);
                int id = int.Parse(tokens[0]);
                float rate = float.Parse(tokens[2]);

                charGachaList.Add(new GachaData(id,rate));
                charTotalRate += rate;
            }

            string[] equipLines = equipCsvFile.text.Split("\n");
            for(int i  = 2;i<equipLines.Length;i++)
            {
                if (string.IsNullOrWhiteSpace(equipLines[i])) continue;

                string[] tokens = equipLines[i].Split(splitSymbol);
                int id = int.Parse(tokens[0]);
                float rate = float.Parse(tokens[1]);

                equipGachaList.Add(new GachaData(id, rate));
                equipTotalRate += rate;
            }
        }


        private void CharRollGacha(int num)
        {
            int counter = num;
            for(int i = 0;i<counter;i++)
            {
                SingleCharGacha();
            }
        }
        private void SingleCharGacha()
        {
            float roll = UnityEngine.Random.Range(0f, charTotalRate);
            float sum = 0f;
            for(int i = 0; i< charGachaList.Count;i++)
            {
                sum += charGachaList[i].rate;
                if (roll <= sum )
                {
                    gachaResult.Add(charGachaList[i].id);
                    Debug.Log($"현재 들어가는 캐릭 ID{charGachaList[i].id}");
                    break;
                }
            }
        }

        private void EquipRollGacha(int num)
        {
            int counter = num;
            for(int i = 0; i<counter;i++)
            {
                SingleEquipGacha();
            }
        }

        private void SingleEquipGacha()
        {
            float rateSum = 0f;
            float roll = UnityEngine.Random.Range(0f, equipTotalRate);

            for (int i = 0; i < equipGachaList.Count; i++)
            {
                rateSum += equipGachaList[i].rate;
                if (roll <= rateSum)
                {
                    gachaResult.Add(equipGachaList[i].id);
                    return;
                }
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
                    isCharGacha = true;
                    if (Manager.Game.CurrentSave.gold >= 200)
                    {
                        Manager.Game.CurrentSave.gold -= 200;
                        CharRollGacha(1);
                        foreach(int id in gachaResult)
                        {
                            Manager.Game.CurrentSave.characterInventory.GachaAdd(id);
                        }
                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<GachaPopUp>();
                    }
                    break;
                case 5:
                    isCharGacha = true;
                    if(Manager.Game.CurrentSave.gold >= 1000)
                    {
                        Manager.Game.CurrentSave.gold -= 1000;
                        CharRollGacha(5);
                        foreach (int id in gachaResult)
                        {
                            Manager.Game.CurrentSave.characterInventory.GachaAdd(id);
                        }
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
                    if (Manager.Game.CurrentSave.gold >= 200)
                    {
                        Manager.Game.CurrentSave.gold -= 200;
                        EquipRollGacha(1);
                        foreach (int id in gachaResult)
                        {
                            equipController.AddEquipment(id);
                        }
                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<GachaPopUp>();
                    }
                    break;
                case 5:
                    if (Manager.Game.CurrentSave.gold >= 1000)
                    {
                        Manager.Game.CurrentSave.gold -= 1000;
                        EquipRollGacha(5);
                        foreach (int id in gachaResult)
                        {
                            equipController.AddEquipment(id);
                        }
                        Manager.Game.SaveGameProgress();
                        UIManager.Instance.ShowPopUp<Gacha5PopUp>();
                    }
                    break;
            }
        }
    }

    [System.Serializable]
    public class GachaData
    {
        public int id;
        public float rate;

        public GachaData(int id, float rate )
        {
            this.id = id;
            this.rate = rate;
        }
    }
}


