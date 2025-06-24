using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    [CreateAssetMenu(fileName = "GameManager", menuName = "Managers/GameManager")]
    public class GameManagerSO : ScriptableObject
    {
        // 게임 일시 정지 상태
        [HideInInspector] public bool isGamePaused;

        // 플레이어 점수
        [HideInInspector] public int score;

        // 점수 추가
        public void AddScore(int value)
        {
            score += value;
        }

        // 게임 일시 정지
        public void PauseGame()
        {
            isGamePaused = true;
            Time.timeScale = 0f;
        }

        // 게임 재개
        public void ResumeGame()
        {
            isGamePaused = false;
            Time.timeScale = 1f;
        }

        public void ResetScore()
        {
            score = 0;
        }
    }
}