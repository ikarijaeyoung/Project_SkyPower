using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    public class GameManagerRunner : MonoBehaviour
    {
        public GameManagerSO gameManager;

        void Start()
        {
            gameManager.ResetScore(); // 게임 재시작 시점에 초기화
                                      // 시작 시 게임 재개 상태로 초기화
            gameManager.ResumeGame();

        }
    }
}