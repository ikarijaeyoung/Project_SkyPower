using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using KYG_skyPower;
using YSK;

namespace JYL
{
    public class StagePopUp : BaseUI
    {
        private TMP_Text stageText => GetUI<TMP_Text>("StageNameText");
        void Start()
        {
            int worldNum = Manager.Game.selectWorldIndex;
            int subStageNum = Manager.Game.selectStageIndex;
            stageText.text = $"STAGE {worldNum} - {subStageNum}";
            GetEvent("StageReBack").Click += RestartStage;
            GetEvent("StageQuitBack").Click += QuitStage;
        }

        void Update()
        {

        }
        private void RestartStage(PointerEventData eventData)
        {
            UIManager.Instance.CleanPopUp();
            Manager.GSM.ReloadCurrentStage();
        }
        private void QuitStage(PointerEventData eventData)
        {
            UIManager.Instance.CleanPopUp();
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }
    }
}

