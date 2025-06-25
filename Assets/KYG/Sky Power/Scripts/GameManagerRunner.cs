using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class GameManagerRunner : MonoBehaviour
    {

        // GameManagerSO 싱글톤 인스턴스 사용 초기화
        private void Awake()
        {
            GameManagerSO.Instance.Init(); // 싱글톤 패턴을 통해 초기화
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (GameManagerSO.Instance.isGamePaused)
                    GameManagerSO.Instance.ResumeGame();
                else GameManagerSO.Instance.PauseGame();
            }
        }
    }
}

