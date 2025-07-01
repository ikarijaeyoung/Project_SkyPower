using LJ2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LJ2
{
    public class SaveTester : MonoBehaviour
    {
        [SerializeField] public GameData gameData;
        public int index = 0;

        private void Awake()
        {
            if (gameData == null)
            {
                gameData = new GameData();
            }
        }

        public void SaveGameData()
        {
            SaveManager.Instance.GameSave(gameData, index);
            Debug.Log("Game data saved successfully.");
        }

        public void LoadGameData()
        {
            SaveManager.Instance.GameLoad(gameData, index);
        }

        public void DeleteGameData()
        {
            SaveManager.Instance.GameDelete(gameData, index);
            Debug.Log("Game data cleared successfully.");
        }
    }
}