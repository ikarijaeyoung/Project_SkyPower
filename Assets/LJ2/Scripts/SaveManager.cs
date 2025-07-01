using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;
using KYG_skyPower;
using YSK;

namespace LJ2
{
    public class SaveManager : Singleton<SaveManager>
    {
        private int subStage { get; set; } = 5;
        protected override void Awake() => base.Awake();

        // 정보 별 저장, 로드, 삭제 함수 따로 구현
        public void PlayerSave(CharictorSave target, int index)
        {
            DataSaveController.Save(target, index);
        }

        public void PlayerLoad(CharictorSave target, int index)
        {
            DataSaveController.Load(ref target, index);
        }

        public void PlayerDelete(CharictorSave target, int index)
        {
            DataSaveController.Delete(target, index);
        }

        // 현재 partial class로 구현된 GameData를 control하는 함수들
        public void GameSave(GameData target, int index,string name)
        {
            //이름 정해줌
            target.playerName = name;
            int dataIndex = 0;
            for (int i = 0; i < Manager.SDM.runtimeData.Count; i++)
            {
                for (int j = 0; j < subStage; j++)
                {
                    StageRuntimeData data = Manager.SDM.runtimeData[dataIndex];
                    target.stageInfo[i, j] = new StageInfo
                    {
                        world = i,
                        stage = j,
                        unlock = data.isUnlocked,
                        isClear = data.isCompleted
                    };
                }
            }   
            DataSaveController.Save(target, index);
        }

        public void GameLoad(ref GameData target, int index)
        {
            DataSaveController.Load(ref target, index);
        }

        public void GameDelete(GameData target, int index)
        {
            DataSaveController.Delete(target, index);
        }
    }
}
