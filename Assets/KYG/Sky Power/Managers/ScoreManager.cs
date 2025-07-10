using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KYG_skyPower
{
    // ���� ���� �Ŵ���
    public class ScoreManager : Singleton<ScoreManager>
    {
        
        public UnityEvent<int> onScoreChanged; // UI ������� UI ������ ���� �̺�Ʈ�� Ȯ�强

        private int score;
        public int Score
        {
            get { return score; }
            private set
            {
                int diff = value - score;
                score = value;
                onScoreChanged?.Invoke(diff); // ���� ��ȭ�ϸ� ����
            }
        }
        public override void Init()
        {
            // �ʿ伺 Ȯ��
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
        }

        public void RecordBestScore()
        {
            int bestScore = Manager.SDM.runtimeData[Manager.Game.selectWorldIndex-1].subStages[Manager.Game.selectStageIndex-1].bestScore;
            if (Score > bestScore)
            {
                // TODO �ű�� �޼�
                Manager.SDM.runtimeData[Manager.Game.selectWorldIndex-1].subStages[Manager.Game.selectStageIndex-1].bestScore = bestScore;
            }
            Manager.Game.CurrentSave.gold += Score; // TODO: ���ھŭ ���� �߰� 
            ResetScore();
            Manager.Game.SaveGameProgress();
        }
    }
}
