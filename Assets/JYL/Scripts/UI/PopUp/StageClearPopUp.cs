using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace JYL
{
    public class StageClearPopUp : BaseUI
    {
        // TODO : 해당 팝업은 게임 클리어 시점에서 팝업된다
        private void Start()
        {
            GetEvent("SCNextStageBtn").Click += NextStage;
            GetEvent("SCReBtn").Click += RestartStage;
            GetEvent("SCQuitBtn").Click += QuitStage;
        }
        private void NextStage(PointerEventData eventData)
        {
            // 스테이지를 선택해서 로드하는 것과 같은 효과. 진행 상황 저장은 게임 클리어 시점에 자동으로 수행
            // 페이드인 페이드아웃 효과가 필요한데, 스테이지 매니저에서 해당 기능이 구현되어야 하지 않나 함
            SceneManager.LoadSceneAsync("dStageScene_JYL");
        }
        private void RestartStage(PointerEventData eventData)
        {
            // "다음 스테이지" 기능과 다를 것 없음. 로드하는 것이 현재 스테이지일 뿐
            SceneManager.LoadSceneAsync("dStageScene_JYL");
        }
        private void QuitStage(PointerEventData eventData)
        {
            // 메인화면으로 돌아감
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }
    }

}
