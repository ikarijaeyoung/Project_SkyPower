using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace JYL
{
    public class SaveFilePanel : BaseUI
    {
        // 불러왔던 세이브 데이터 기준으로 데이터를 채운다
        void Start()
        {
            //GetUI<TMP_Text>("SaveFileData").text = $"{GameManager.Instance.selectSaveFile.name";
            GetUI<TMP_Text>("SaveFileData").text = $"SkyPower1";
            GetEvent("SaveDelBtn").Click += OnDelClick;
            GetEvent("SaveStartBtn").Click += OnStartClick;
        }

        private void OnDelClick(PointerEventData eventData)
        {
            // 세이브 매니저를 통해 세이브파일 삭제
            //SaveManager.PlayerDelete(index);
            // 텍스트 출력으로 삭제했음을 알림.
            UIManager.Instance.ClosePopUp();
        }
        private void OnStartClick(PointerEventData eventData)
        {
            //씬 넘어감 -> mainScene
            SceneManager.LoadSceneAsync("bMainScene_JYL");
        }
    
    }
}
