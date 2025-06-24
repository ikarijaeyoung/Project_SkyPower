using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "GameManagerSO", menuName = "Manager/GameManager")]
    public class GameManagerSO : ScriptableObject
    {

        private static GameManagerSO instance;

        public static GameManagerSO Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.Load<GameManagerSO>("GameManagerSO");
                return instance;
            }
        }

        [Header("Game State")]
        public int score; // 게임 스코어
        [SerializeField] private int defaultPlayerLives = 5; // 초기화 값 에디터에서 조정 가능
        [SerializeField] public int playerLives; // 플레이어 체력
        public bool isGameOver; // 게임 오버 여부

        public UnityEvent onGameOver;
        public UnityEvent<int> onScoreChanged; // 이벤트 기반 확장 코드

        public void Init() // 게임 초기화
        {
            score = 0;
            playerLives = defaultPlayerLives; // default값
            isGameOver = false;
            Debug.Log("Game Initialized"); // 디버그용 코드
        }

        public void AddScore(int amount) // 게임 점수
        {
            score += amount;
            onScoreChanged?.Invoke(score);
        }

        public void PlayerHit() // 플레이어 피격
        {
            playerLives--;
            if (playerLives <= 0)
            {
                isGameOver = true;
                onGameOver?.Invoke();
            }

        }       

    }
}
