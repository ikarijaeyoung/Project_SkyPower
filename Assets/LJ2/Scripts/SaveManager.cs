using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;
using JYL;

namespace LJ2
{
    public class SaveManager : Singleton<SaveManager>
    {
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
            target.playerName = name;
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
