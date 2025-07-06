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
            // 여기서 스테이지매니저에서 현재 스테이지의 정보를 받아온다.
            // StageManager.Instance.curStage.name
            int worldNum = Manager.Game.selectStageIndex;
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

