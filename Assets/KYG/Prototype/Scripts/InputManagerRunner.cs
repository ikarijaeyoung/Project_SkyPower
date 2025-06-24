using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    public class InputManagerRunner : MonoBehaviour
    {
        public InputManagerSO inputManager;
        public GameManagerSO gameManager;
        public UIManagerSO uiManager;

        void Update()
        {
            if (inputManager.IsPausePressed())
            {
                if (gameManager.isGamePaused)
                {
                    gameManager.ResumeGame();
                    uiManager.ShowPauseUI(false);
                }
                else
                {
                    gameManager.PauseGame();
                    uiManager.ShowPauseUI(true);
                }
            }
        }
    }
}