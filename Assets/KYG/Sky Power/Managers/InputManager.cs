using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    // 입력 감지만 담당 (Pause 등)
    public class InputManager : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance.isPaused)
                    GameManager.Instance.ResumeGame();
                else
                    GameManager.Instance.PausedGame();
            }
        }
    }
}
