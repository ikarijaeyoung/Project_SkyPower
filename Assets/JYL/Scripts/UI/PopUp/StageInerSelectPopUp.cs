using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KYG_skyPower;
using TMPro;
using YSK;

namespace JYL
{
    public class StageInerSelectPopUp : BaseUI
    {
        private int stageNum = 5;
        private int worldNum;
        private int selectStageNum;
        private Button[] stageBtn;
        void Start()
        {
            stageBtn = new Button[stageNum];
            SetStageButtons();
        }

        void Update() { }
        private void SetStageButtons()
        {
            worldNum = UIManager.Instance.selectIndexUI;
            for (int i = 0; i < stageNum; i++)
            {
                stageBtn[i] = GetUI<Button>($"StageBtn_{i + 1}");
                GetUI<TMP_Text>($"StageText_{i+1}").text = $"Stage {worldNum} - {i+1}";
                if (Manager.SDM.runtimeData[worldNum].subStages[i].isUnlocked)
                {
                    stageBtn[i].interactable = true;
                    GetEvent($"{stageBtn[i].gameObject.name}").Click += SetStageIndex;
                    GetEvent($"{stageBtn[i].gameObject.name}").Click += ChangeSceneToStage;
                }
                else
                {
                    stageBtn[i].interactable = false;
                }

            }
        }
        private void SetStageIndex(PointerEventData eventData)
        {
            Util.ExtractTrailNumber(eventData.pointerClick.gameObject.name,out selectStageNum);
            Debug.Log($"{eventData.pointerClick.gameObject.name}  {selectStageNum}");
            Manager.Game.selectWorldIndex = worldNum+1;
            Manager.Game.selectStageIndex = selectStageNum;
        }
        private void ChangeSceneToStage(PointerEventData eventData)
        {
            Debug.Log($"¿Ã∞≈ ¡¢±Ÿ µ ?{worldNum} {selectStageNum}");
            UIManager.Instance.CleanPopUp();
            Manager.GSM.LoadGameSceneWithStage("dStageScene_JYL", Manager.Game.selectWorldIndex, Manager.Game.selectStageIndex);
        }
    }
}

