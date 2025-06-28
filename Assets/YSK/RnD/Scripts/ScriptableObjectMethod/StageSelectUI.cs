// RnD/Scripts/ScriptableObjectMethod/StageSelectUI.cs
using UnityEngine;
using UnityEngine.UI;
using YSK;
using KYG_skyPower;
using JYL;
using TMPro;

namespace YSK
{
    public class StageSelectUI : BaseUI
    {
        [Header("Stage Data")]
        [SerializeField] private StageDataManager dataManager;

        [Header("UI Button Names")]
        [SerializeField] private string mainMenuButtonName = "MainMenuButton";
        [SerializeField] private string mainStageSelectButtonName = "MainStageSelectButton";
        [SerializeField] private string testSceneButtonName = "TestSceneButton";

        [Header("Stage Button Names")]
        [SerializeField] private string stage1_1ButtonName = "Stage1_1Button";
        [SerializeField] private string stage1_2ButtonName = "Stage1_2Button";
        [SerializeField] private string stage2_1ButtonName = "Stage2_1Button";
        [SerializeField] private string stage2_2ButtonName = "Stage2_2Button";
        [SerializeField] private string stage3_1ButtonName = "Stage3_1Button";

        [Header("Score Display Names")]
        [SerializeField] private string stage1_1ScoreTextName = "Stage1_1ScoreText";
        [SerializeField] private string stage1_2ScoreTextName = "Stage1_2ScoreText";
        [SerializeField] private string stage2_1ScoreTextName = "Stage2_1ScoreText";
        [SerializeField] private string stage2_2ScoreTextName = "Stage2_2ScoreText";
        [SerializeField] private string stage3_1ScoreTextName = "Stage3_1ScoreText";

        [Header("Lock Icon Names")]
        [SerializeField] private string stage1_1LockIconName = "Stage1_1LockIcon";
        [SerializeField] private string stage1_2LockIconName = "Stage1_2LockIcon";
        [SerializeField] private string stage2_1LockIconName = "Stage2_1LockIcon";
        [SerializeField] private string stage2_2LockIconName = "Stage2_2LockIcon";
        [SerializeField] private string stage3_1LockIconName = "Stage3_1LockIcon";

        private void Start()
        {
            InitializeDataManager();
            ConnectButtons();
            UpdateUI();
        }

        private void InitializeDataManager()
        {
            if (dataManager == null)
            {
                dataManager = FindObjectOfType<StageDataManager>();
                if (dataManager == null)
                {
                    Debug.LogWarning("StageDataManager를 찾을 수 없습니다!");
                }
            }
        }

        private void ConnectButtons()
        {
            // 메뉴 버튼들 연결
            ConnectMenuButton(mainMenuButtonName, "RnDMainMenu");
            ConnectMenuButton(mainStageSelectButtonName, "RnDMainStageSelectScene");
            ConnectMenuButton(testSceneButtonName, "RnDBaseStageTestScene");

            // 스테이지 버튼들 연결
            ConnectStageButton(stage1_1ButtonName, 1, 1);
            ConnectStageButton(stage1_2ButtonName, 1, 2);
            ConnectStageButton(stage2_1ButtonName, 2, 1);
            ConnectStageButton(stage2_2ButtonName, 2, 2);
            ConnectStageButton(stage3_1ButtonName, 3, 1);
        }

        private void ConnectMenuButton(string buttonName, string sceneName)
        {
            Button button = GetUI<Button>(buttonName);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log($"{buttonName} 버튼 클릭됨! 씬 {sceneName} 로드");
                    LoadScene(sceneName);
                });
                Debug.Log($"{buttonName} 메뉴 버튼 연결 완료");
            }
            else
            {
                Debug.LogWarning($"{buttonName} 버튼을 찾을 수 없습니다!");
            }
        }

        private void ConnectStageButton(string buttonName, int mainStage, int subStage)
        {
            Button button = GetUI<Button>(buttonName);
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log($"{buttonName} 버튼 클릭됨! 스테이지 {mainStage}-{subStage} 로드");

                    // 해금 상태 확인
                    if (dataManager != null && !CanPlayStage(mainStage, subStage))
                    {
                        Debug.Log($"스테이지 {mainStage}-{subStage}가 해금되지 않았습니다!");
                        ShowLockMessage(mainStage, subStage);
                        return;
                    }

                    LoadStage(mainStage, subStage);
                });
                Debug.Log($"{buttonName} 스테이지 버튼 연결 완료");
            }
            else
            {
                Debug.LogWarning($"{buttonName} 버튼을 찾을 수 없습니다!");
            }
        }

        private void UpdateUI()
        {
            if (dataManager == null) return;

            // 스테이지 버튼 상태 업데이트
            UpdateStageButtonState(stage1_1ButtonName, stage1_1LockIconName, stage1_1ScoreTextName, 1, 1);
            UpdateStageButtonState(stage1_2ButtonName, stage1_2LockIconName, stage1_2ScoreTextName, 1, 2);
            UpdateStageButtonState(stage2_1ButtonName, stage2_1LockIconName, stage2_1ScoreTextName, 2, 1);
            UpdateStageButtonState(stage2_2ButtonName, stage2_2LockIconName, stage2_2ScoreTextName, 2, 2);
            UpdateStageButtonState(stage3_1ButtonName, stage3_1LockIconName, stage3_1ScoreTextName, 3, 1);
        }

        private void UpdateStageButtonState(string buttonName, string lockIconName, string scoreTextName, int mainStage, int subStage)
        {
            Button button = GetUI<Button>(buttonName);
            if (button == null) return;

            bool isUnlocked = CanPlayStage(mainStage, subStage);

            // 버튼 활성화/비활성화
            button.interactable = isUnlocked;

            // 버튼 색상 변경
            ColorBlock colors = button.colors;
            if (isUnlocked)
            {
                colors.normalColor = Color.white;
                colors.disabledColor = Color.white;
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.disabledColor = Color.gray;
            }
            button.colors = colors;

            // 잠금 아이콘 표시/숨기기
            GameObject lockIcon = GetUI(lockIconName);
            if (lockIcon != null)
            {
                lockIcon.SetActive(!isUnlocked);
            }

            // 점수 표시
            if (isUnlocked)
            {
                UpdateScoreDisplay(scoreTextName, mainStage, subStage);
            }
        }

        private void UpdateScoreDisplay(string scoreTextName, int mainStage, int subStage)
        {
            if (dataManager == null) return;

            TextMeshProUGUI scoreText = GetUI<TextMeshProUGUI>(scoreTextName);
            if (scoreText != null)
            {
                int score = dataManager.GetStageScore(mainStage, subStage);
                if (score > 0)
                {
                    scoreText.text = $"최고 점수: {score}";
                    scoreText.gameObject.SetActive(true);
                }
                else
                {
                    scoreText.gameObject.SetActive(false);
                }
            }
        }

        private bool CanPlayStage(int mainStage, int subStage)
        {
            if (dataManager == null) return true;

            return dataManager.IsStageUnlocked(mainStage) &&
                   dataManager.IsSubStageUnlocked(mainStage, subStage);
        }

        private void ShowLockMessage(int mainStage, int subStage)
        {
            Debug.Log($"스테이지 {mainStage}-{subStage}는 아직 해금되지 않았습니다!");
        }

        private void LoadStage(int mainStage, int subStage)
        {
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.LoadGameSceneWithStage("RnDBaseStageTestScene", mainStage, subStage);
            }
            else
            {
                Debug.LogError("GameSceneManager.Instance가 null입니다!");
            }
        }

        private void LoadScene(string sceneName)
        {
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.LoadGameScene(sceneName);
            }
            else
            {
                Debug.LogError("GameSceneManager.Instance가 null입니다!");
            }
        }

        // 외부에서 UI 업데이트를 요청할 때 사용
        public void RefreshUI()
        {
            UpdateUI();
        }
    }
}