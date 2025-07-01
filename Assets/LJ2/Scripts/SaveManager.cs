using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

namespace LJ2
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

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

        public void GameSave(GameData target, int index)
        {
            DataSaveController.Save(target, index);
        }

        public void GameLoad(GameData target, int index)
        {
            DataSaveController.Load(ref target, index);
        }

        public void GameDelete(GameData target, int index)
        {
            DataSaveController.Delete(target, index);
        }
    }
}
