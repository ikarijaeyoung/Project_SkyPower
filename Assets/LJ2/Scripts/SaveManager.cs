using IO;
using KYG_skyPower;
using System;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace LJ2
{
    public class SaveManager : Singleton<SaveManager>
    {
        private int subStage { get; set; } = 5;
        protected override void Awake() => base.Awake();
        
        public void GameSave(GameData target, int index, string name = "")
        {
            if(name != "")
            {
                target.playerName = name;
            }
            GameData saveTargetData = SaveStageInfo(target);
            DataSaveController.Save(saveTargetData, index+1);
        }

        public void GameLoad(ref GameData target, int index)
        {
            DataSaveController.Load(ref target, index);
        }

        public void GameDelete(GameData target, int index)
        {
            DataSaveController.Delete(target, index);
        }

        private GameData SaveStageInfo(GameData target)
        {
            if (target.stageInfo == null)
            {
                target.stageInfo = new StageInfo[Manager.SDM.runtimeData.Count * subStage];
                for (int i = 0; i < Manager.SDM.runtimeData.Count * subStage; i++)
                {
                    target.stageInfo[i] = new StageInfo
                    {
                        world = 1 + i / subStage,
                        stage = 1 + i % subStage,
                        unlock = Manager.SDM.runtimeData[i / subStage].subStages[i % subStage].isUnlocked,
                        isClear = Manager.SDM.runtimeData[i / subStage].subStages[i % subStage].isCompleted
                    };
                }
            }
            else if (target.stageInfo != null)
            {
                StageInfo[] tmp = new StageInfo[Manager.SDM.runtimeData.Count * subStage];
                for (int i = 0; i < Manager.SDM.runtimeData.Count * subStage; i++)
                {
                    tmp[i] = new StageInfo
                    {
                        world = 1 + i / subStage,
                        stage = 1 + i % subStage,
                        unlock = Manager.SDM.runtimeData[i / subStage].subStages[i % subStage].isUnlocked,
                        isClear = Manager.SDM.runtimeData[i / subStage].subStages[i % subStage].isCompleted
                    };
                    target.stageInfo[i] = tmp[i];
                }
            }
            return target;
        }
    }
}
