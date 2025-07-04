using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

namespace JYL
{
    public class StagePopUp : BaseUI
    {
        private TMP_Text stageText => GetUI<TMP_Text>("StageNameText");
        void Start()
        {
            // 여기서 스테이지매니저에서 현재 스테이지의 정보를 받아온다.
            // StageManager.Instance.curStage.name
            stageText.text = $"STAGE 1-1";
            GetEvent("StageReBack").Click += RestartStage;
            GetEvent("StageQuitBack").Click += QuitStage;
        }

        void Update()
        {

        }
        private void RestartStage(PointerEventData eventData)
        {
            // 스테이지 매니저의 리스타트 기능을 활용함
            // StageManager.Instance.restart();
            SceneManager.LoadSceneAsync("dStageScene_JYL");
        }
        private void QuitStage(PointerEventData eventData)
        {
            // 스테이지 또는 씬 매니저의 기능을 통해 구현. 메인 씬으로 돌아간다
            // StageManager.Instance.SceneChange();
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }
    }
}

