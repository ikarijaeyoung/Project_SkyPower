using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class GameModePopUp : BaseUI
    {
        void Start()
        {
            GetEvent("StageModeBtn").Click += data => UIManager.Instance.ShowPopUp<StageSelectPopUp>();
            // 무한모드 추가 시 버튼 할당
        }
    }
}

