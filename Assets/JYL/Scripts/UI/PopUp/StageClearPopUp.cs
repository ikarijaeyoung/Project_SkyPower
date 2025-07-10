using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KYG_skyPower;
using TMPro;

namespace JYL
{
    public class StageClearPopUp : BaseUI
    {
        private Button nextButton;
        private TMP_Text stageClearText => GetUI<TMP_Text>("StageClearFailText");
        private TMP_Text stageNumText => GetUI<TMP_Text>("StageNumberText");
        // TODO : 해당 팝업은 게임 클리어 시점에서 팝업된다
        private void Start()
        {
            SetNextStageBtn();
            GetEvent("SCReBtn").Click += RestartStage;
            GetEvent("SCQuitBtn").Click += QuitStage;
            if(Manager.Game.isGameCleared) Manager.Score.RecordBestScore();
        }
        private void OnEnable() { UIManager.canClosePopUp = false; }
        private void OnDisable() { UIManager.canClosePopUp = true; }

        private void SetNextStageBtn() // 버튼 활성화 여부.  다음 스테이지 정보가 없을 경우, 비활성화
        {
            stageNumText.text = $"STAGE {Manager.Game.selectWorldIndex} - {Manager.Game.selectStageIndex}";
            nextButton = GetUI<Button>("SCNextStageBtn");
            if (Manager.Game.isGameCleared)
            { 
                stageClearText.text = "스테이지 클리어";
                int worldIndex = Manager.Game.selectWorldIndex;
                int stageIndex = Manager.Game.selectStageIndex;
                if (stageIndex > 5)
                {
                    worldIndex++;
                    stageIndex = 1;
                }
                if (Manager.SDM.runtimeData[worldIndex - 1].subStages[stageIndex - 1] == null)
                {
                    nextButton.interactable = false;
                }
                else
                {
                    GetEvent("SCNextStageBtn").Click += NextStage;
                }
            }
            else
            {

                stageClearText.text = "클리어 실패";
                nextButton.interactable = false;
            }
            
        }
        private void NextStage(PointerEventData eventData)
        {
            Manager.Game.selectStageIndex++;
            if(Manager.Game.selectStageIndex>5)
            {
                Manager.Game.selectWorldIndex++;
                Manager.Game.selectStageIndex = 1;
            }
            UIManager.Instance.CleanPopUp();
            Manager.Game.ResetState();
            Manager.GSM.LoadGameSceneWithStage("dStageScene_JYL", Manager.Game.selectWorldIndex, Manager.Game.selectStageIndex);
        }
        private void RestartStage(PointerEventData eventData)
        {
            UIManager.Instance.CleanPopUp();
            Manager.Game.ResetState();
            Manager.GSM.ReloadCurrentStage();
        }
        private void QuitStage(PointerEventData eventData)
        {
            UIManager.Instance.CleanPopUp();
            Manager.Game.ResetState();
            Manager.GSM.LoadScene("bMainScene_JYL");
        }
    }
}
