// RnD/Scripts/ScriptableObjectMethod/StageSelectUI.cs
using UnityEngine;
using UnityEngine.UI;
using YSK;
using KYG_skyPower;
using JYL;
using TMPro;
using System.Collections;
using System.Net.NetworkInformation;

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

        [Header("Debug")]
        [SerializeField] private bool forceUIUpdate = false;

        private void Start()
        {
            InitializeDataManager();
            ConnectButtons();
            
            // 데이터 로딩을 기다린 후 UI 업데이트
            StartCoroutine(UpdateUIAfterDataLoad());
            
            // StageDataManager 이벤트 구독
            if (StageDataManager.OnStageDataChanged != null)
            {
                StageDataManager.OnStageDataChanged += UpdateUI;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (StageDataManager.OnStageDataChanged != null)
            {
                StageDataManager.OnStageDataChanged -= UpdateUI;
            }
        }

        private System.Collections.IEnumerator UpdateUIAfterDataLoad()
        {
            // StageDataManager가 초기화될 때까지 대기
            yield return new WaitForEndOfFrame();
            
            // 한 프레임 더 대기하여 데이터가 완전히 로드되도록 함
            yield return new WaitForEndOfFrame();
            
            Debug.Log("=== 데이터 로딩 완료 후 UI 업데이트 시작 ===");
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
            Debug.Log("=== ConnectButtons 시작 ===");
            
            
            // 메뉴 버튼들 연결
            ConnectMenuButton(mainMenuButtonName, "bMainScene_JYL");
            ConnectMenuButton(mainStageSelectButtonName, "cStoreScene_JYL");
            ConnectMenuButton(testSceneButtonName, "aTitleScene_JYL");

            // 스테이지 버튼들 연결
            ConnectStageButton(stage1_1ButtonName, 1, 1);
            ConnectStageButton(stage1_2ButtonName, 1, 2);
            ConnectStageButton(stage2_1ButtonName, 2, 1);
            ConnectStageButton(stage2_2ButtonName, 2, 2);
            ConnectStageButton(stage3_1ButtonName, 3, 1);
            
            Debug.Log("=== ConnectButtons 완료 ===");
        }

        private void ConnectMenuButton(string buttonName, string sceneName)
        {
            Debug.Log($"ConnectMenuButton 시도: {buttonName}");
            
            // 1. 게임오브젝트 먼저 찾기
            GameObject buttonGO = GetUI(buttonName);
            if (buttonGO == null)
            {
                Debug.LogError($"게임오브젝트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"게임오브젝트 찾음: {buttonGO.name}");
            
            // 2. Button 컴포넌트 찾기
            Button button = buttonGO.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"Button 컴포넌트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"Button 컴포넌트 찾음: {button.name}");
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                Debug.Log($"{buttonName} 버튼 클릭됨! 씬 {sceneName} 로드");
                LoadScene(sceneName);
            });
            Debug.Log($"{buttonName} 메뉴 버튼 연결 완료");
        }

        private void ConnectStageButton(string buttonName, int mainStage, int subStage)
        {
            Debug.Log($"ConnectStageButton 시도: {buttonName}");
            
            // 1. 게임오브젝트 먼저 찾기
            GameObject buttonGO = GetUI(buttonName);
            if (buttonGO == null)
            {
                Debug.LogError($"게임오브젝트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"게임오브젝트 찾음: {buttonGO.name}");
            
            // 2. Button 컴포넌트 찾기
            Button button = buttonGO.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"Button 컴포넌트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"Button 컴포넌트 찾음: {button.name}");
            
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                Debug.Log($"{buttonName} 버튼 클릭됨! 스테이지 {mainStage}-{subStage} 로드");

                // 임시로 해금 상태 확인 건너뛰기 (테스트용)
                Debug.Log($"스테이지 {mainStage}-{subStage} 로드 시작...");
                LoadStage(mainStage, subStage);
            });
            Debug.Log($"{buttonName} 스테이지 버튼 연결 완료");
        }

        private void UpdateUI()
        {
            if (dataManager == null) 
            {
                Debug.LogError("dataManager가 null입니다!");
                return;
            }

            Debug.Log("=== UpdateUI 시작 ===");
            Debug.Log($"dataManager 상태: {(dataManager != null ? "찾음" : "못 찾음")}");

            // 스테이지 버튼 상태 업데이트
            UpdateStageButtonState(stage1_1ButtonName,  stage1_1ScoreTextName, 1, 1);
            UpdateStageButtonState(stage1_2ButtonName,  stage1_2ScoreTextName, 1, 2);
            UpdateStageButtonState(stage2_1ButtonName,  stage2_1ScoreTextName, 2, 1);
            UpdateStageButtonState(stage2_2ButtonName,  stage2_2ScoreTextName, 2, 2);
            UpdateStageButtonState(stage3_1ButtonName,  stage3_1ScoreTextName, 3, 1);
            
            Debug.Log("=== UpdateUI 완료 ===");
        }

        private void UpdateStageButtonState(string buttonName, string scoreTextName, int mainStage, int subStage)
        {
            Debug.Log($"UpdateStageButtonState 시도: {buttonName} (스테이지 {mainStage}-{subStage})");
            
            // 1. 게임오브젝트 먼저 찾기
            GameObject buttonGO = GetUI(buttonName);
            if (buttonGO == null)
            {
                Debug.LogError($"게임오브젝트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"게임오브젝트 찾음: {buttonGO.name}");
            
            // 2. Button 컴포넌트 찾기
            Button button = buttonGO.GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError($"Button 컴포넌트를 찾을 수 없음: {buttonName}");
                return;
            }
            
            Debug.Log($"Button 컴포넌트 찾음: {button.name}");

            bool isUnlocked = CanPlayStage(mainStage, subStage);
            Debug.Log($"스테이지 {mainStage}-{subStage} 해금 상태: {isUnlocked}");

            // 버튼 활성화/비활성화 - 강제로 설정
            button.interactable = isUnlocked;
            Debug.Log($"버튼 {buttonName} interactable 설정: {isUnlocked} (실제 값: {button.interactable})");

            // 버튼 색상 변경 - 더 명확하게 구분
            ColorBlock colors = button.colors;
            if (isUnlocked)
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                colors.selectedColor = Color.white;
                colors.disabledColor = Color.white;
                Debug.Log($"버튼 {buttonName} 색상: 흰색 (해금됨) - 클릭 가능");
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.gray;
                colors.selectedColor = Color.gray;
                colors.disabledColor = Color.gray;
                Debug.Log($"버튼 {buttonName} 색상: 회색 (잠금됨) - 클릭 불가");
            }
            button.colors = colors;

            // 버튼의 Image 컴포넌트도 확인
            Image buttonImage = buttonGO.GetComponent<Image>();
            if (buttonImage != null)
            {
                if (isUnlocked)
                {
                    buttonImage.color = Color.white;
                    Debug.Log($"버튼 이미지 색상: 흰색으로 설정");
                }
                else
                {
                    buttonImage.color = Color.gray;
                    Debug.Log($"버튼 이미지 색상: 회색으로 설정");
                }
            }

            // 점수 표시

                UpdateScoreDisplay(scoreTextName, mainStage, subStage);
            
            
            Debug.Log($"UpdateStageButtonState 완료: {buttonName} (interactable: {button.interactable})");
        }

        private void UpdateScoreDisplay(string scoreTextName, int mainStage, int subStage)
        {
            if (dataManager == null) return;

            TextMeshProUGUI scoreText = GetUI<TextMeshProUGUI>(scoreTextName);
            if (scoreText != null)
            {
                int score = dataManager.GetStageScore(mainStage, subStage);
                if (score >= 0)
                {
                    scoreText.text = $"최고 점수: {score}";
                    scoreText.gameObject.SetActive(true);
                    Debug.Log($"스테이지 {mainStage}-{subStage} 점수 표시: {score}");
                }
                else
                {
                    scoreText.gameObject.SetActive(false);
                    Debug.Log($"스테이지 {mainStage}-{subStage} 점수 없음");
                }
            }
            else
            {
                Debug.LogWarning($"점수 텍스트 컴포넌트를 찾을 수 없음: {scoreTextName}");
            }
        }

        private bool CanPlayStage(int mainStage, int subStage)
        {
            if (dataManager == null) 
            {
                Debug.LogWarning("dataManager가 null입니다!");
                return false;
            }

            bool stageUnlocked = dataManager.IsStageUnlocked(mainStage);
            bool subStageUnlocked = dataManager.IsSubStageUnlocked(mainStage, subStage);
            
            Debug.Log($"스테이지 {mainStage}-{subStage} 확인:");
            Debug.Log($"  - 메인 스테이지 {mainStage} 해금: {stageUnlocked}");
            Debug.Log($"  - 서브 스테이지 {subStage} 해금: {subStageUnlocked}");
            
            bool canPlay = stageUnlocked && subStageUnlocked;
            Debug.Log($"  - 최종 결과: {canPlay}");
            
            return canPlay;
        }

        private void ShowLockMessage(int mainStage, int subStage)
        {
            if (dataManager.IsSubStageUnlocked(mainStage, subStage))
            {
                Debug.Log($"스테이지 {mainStage}-{subStage}는 아직 해금되지 않았습니다!");
            }
            
        }

        private void LoadStage(int mainStage, int subStage)
        {
            Debug.Log($"LoadStage 호출: {mainStage}-{subStage}");
            
            if (GameSceneManager.Instance != null)
            {
                Debug.Log($"GameSceneManager.Instance 찾음, 씬 로드 시작");
                GameSceneManager.Instance.LoadGameSceneWithStage("dStageScene_JYL", mainStage, subStage);
            }
            else
            {
                Debug.LogError("GameSceneManager.Instance가 null입니다!");
                Debug.LogError("씬에 GameSceneManager 오브젝트가 있는지 확인해주세요!");
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

        private void ClearStage()
        {
            
        }

        private void Update()
        {
            // 인스펙터에서 체크박스를 토글하면 UI 업데이트
            if (forceUIUpdate)
            {
                UpdateUI();
                forceUIUpdate = false;
                Debug.Log("수동 UI 업데이트 실행");
            }
        }
    }
}