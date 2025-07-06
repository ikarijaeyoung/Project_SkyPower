using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KYG_skyPower
{
    // 점수 전담 매니저
    public class ScoreManager : Singleton<ScoreManager>
    {
        
        public UnityEvent<int> onScoreChanged; // UI 담당자의 UI 연결을 위해 이벤트로 확장성

        private int score;
        public int Score
        {
            get { return score; }
            private set
            {
                int diff = value - score;
                score = value;
                onScoreChanged?.Invoke(diff); // 점수 변화하면 수행
            }
        }
        public override void Init()
        {
            // 필요성 확인
        }

        /*private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        public void ResetScore() { Score = 0;}
        public void AddScore(int value)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
            Score += value;
            Debug.Log($"Score : {Score}");
        }

        public void RecordBestScore()
        {
            int bestScore = Manager.SDM.runtimeData[Manager.Game.selectWorldIndex].subStages[Manager.Game.selectStageIndex].bestScore;
            if (Score > bestScore)
            {
                // TODO 신기록 달성
                Manager.SDM.runtimeData[Manager.Game.selectWorldIndex].subStages[Manager.Game.selectStageIndex].bestScore = bestScore;
            }
            ResetScore();
        }
    }
}
