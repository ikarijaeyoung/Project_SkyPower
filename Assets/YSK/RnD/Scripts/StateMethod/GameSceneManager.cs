using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using YSK;

namespace YSK
{
    /// <summary>
    /// 게임의 씬 전환을 관리하는 매니저 클래스입니다.
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string stageSelectScene = "StageSelect";
        [SerializeField] private string gameScene = "GameScene";
        [SerializeField] private string resultScene = "ResultScene";
        
        [Header("Development Settings")]
        [SerializeField] private bool useCurrentSceneForTesting = true; // 프로토타입용
        [SerializeField] private bool enableDebugLogs = true; // 디버그 로그 활성화
        
        [Header("Loading Screen")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text progressText;
        [SerializeField] private float minLoadingTime = 1f;
        
        [Header("Transition Settings")]
        [SerializeField] private bool useFadeTransition = true;
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Font Settings")]
        [SerializeField] private TMP_FontAsset notoSansKRFont; // Inspector에서 할당
        
        // UI 참조 저장
        private TextMeshProUGUI stageTextReference;
        
        // 현재 스테이지 ID 저장
        private int currentStageID = 1;
        
        // 싱글톤 패턴
        public static GameSceneManager Instance { get; private set; }
        
        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        public static event Action<float> OnLoadingProgressChanged;
        public static event Action<string> OnSceneModeChanged; // 프로토타입용 이벤트
        
        // 프로퍼티
        public bool IsLoading { get; private set; }
        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsPrototypeMode => useCurrentSceneForTesting;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // 싱글톤 설정
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSceneManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // 프로토타입 시연용: 항상 메인메뉴로 시작
            if (useCurrentSceneForTesting)
            {
                // 잠시 후 메인메뉴 모드로 전환 (다른 컴포넌트 초기화 대기)
                Invoke(nameof(LoadMainMenu), 0.1f);
            }
            else
            {
                // 실제 씬 전환 모드
                if (string.IsNullOrEmpty(CurrentSceneName) || CurrentSceneName == "InitScene")
                {
                    LoadMainMenu();
                }
            }
        }
        
        private void OnDestroy()
        {
            // 씬 전환 이벤트 구독 해제
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            if (enableDebugLogs)
            {
                Debug.Log("GameSceneManager가 파괴되었습니다.");
            }
        }
        
        #endregion
        
        /// <summary>
        /// 씬 매니저를 초기화합니다.
        /// </summary>
        private void InitializeSceneManager()
        {
            // 로딩 화면이 없으면 자동 생성
            if (loadingScreen == null)
            {
                CreateLoadingScreen();
            }
            
            // 씬 전환 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            if (enableDebugLogs)
            {
                Debug.Log($"GameSceneManager 초기화 완료 - 프로토타입 모드: {useCurrentSceneForTesting}");
            }
        }
        
        /// <summary>
        /// 로딩 화면을 자동으로 생성합니다.
        /// </summary>
        private void CreateLoadingScreen()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("LoadingCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // 최상위에 표시
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            // 로딩 화면 배경
            GameObject loadingObj = new GameObject("LoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            Image backgroundImage = loadingObj.AddComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 0.8f);
            
            RectTransform bgRect = loadingObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 진행률 바
            GameObject progressObj = new GameObject("ProgressBar");
            progressObj.transform.SetParent(loadingObj.transform, false);
            
            Slider slider = progressObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
            
            RectTransform sliderRect = progressObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.2f, 0.4f);
            sliderRect.anchorMax = new Vector2(0.8f, 0.5f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            
            // 진행률 텍스트
            GameObject textObj = new GameObject("ProgressText");
            textObj.transform.SetParent(loadingObj.transform, false);
            
            Text text = textObj.AddComponent<Text>();
            text.text = "Loading... 0%";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // 수정된 부분
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.6f);
            textRect.anchorMax = new Vector2(0.8f, 0.7f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // 참조 설정
            loadingScreen = loadingObj;
            progressBar = slider;
            progressText = text;
            
            // 초기 상태 설정
            loadingScreen.SetActive(false);
        }
        
        #region Scene Loading Methods
        
        /// <summary>
        /// 메인메뉴 씬으로 이동합니다.
        /// </summary>
        public void LoadMainMenu()
        {
            if (useCurrentSceneForTesting)
            {
                LoadMainMenuMode();
            }
            else
            {
                StartCoroutine(LoadSceneAsync(mainMenuScene));
            }
        }
        
        /// <summary>
        /// 스테이지 선택 씬으로 이동합니다.
        /// </summary>
        public void LoadStageSelect()
        {
            if (useCurrentSceneForTesting)
            {
                LoadStageSelectMode();
            }
            else
            {
                StartCoroutine(LoadSceneAsync(stageSelectScene));
            }
        }
        
        /// <summary>
        /// 게임 씬으로 이동합니다.
        /// </summary>
        /// <param name="stageID">시작할 스테이지 ID</param>
        public void LoadGameScene(int stageID = 1)
        {
            // 스테이지 ID를 PlayerPrefs에 저장
            PlayerPrefs.SetInt("SelectedStageID", stageID);
            PlayerPrefs.Save();
            
            if (useCurrentSceneForTesting)
            {
                LoadGameMode(stageID);
            }
            else
            {
                StartCoroutine(LoadSceneAsync(gameScene));
            }
        }
        
        /// <summary>
        /// 결과 화면으로 이동합니다.
        /// </summary>
        /// <param name="score">점수</param>
        /// <param name="isWin">승리 여부</param>
        public void LoadResultScene(int score = 0, bool isWin = true)
        {
            PlayerPrefs.SetInt("GameScore", score);
            PlayerPrefs.SetInt("GameWin", isWin ? 1 : 0);
            PlayerPrefs.Save();
            
            if (useCurrentSceneForTesting)
            {
                LoadResultMode(score, isWin);
            }
            else
            {
                StartCoroutine(LoadSceneAsync(resultScene));
            }
        }
        
        /// <summary>
        /// 현재 씬을 다시 로드합니다.
        /// </summary>
        public void ReloadCurrentScene()
        {
            if (useCurrentSceneForTesting)
            {
                ReloadCurrentMode();
            }
            else
            {
                StartCoroutine(LoadSceneAsync(CurrentSceneName));
            }
        }
        
        /// <summary>
        /// 특정 씬으로 이동합니다.
        /// </summary>
        /// <param name="sceneName">씬 이름</param>
        public void LoadScene(string sceneName)
        {
            if (useCurrentSceneForTesting)
            {
                LoadSceneMode(sceneName);
            }
            else
            {
                StartCoroutine(LoadSceneAsync(sceneName));
            }
        }
        
        #endregion
        
        #region Prototype Mode Methods
        
        /// <summary>
        /// 프로토타입 모드: 메인메뉴 모드로 전환
        /// </summary>
        private void LoadMainMenuMode()
        {
            if (enableDebugLogs)
            {
                Debug.Log(" === 메인메뉴 모드로 전환 ===");
                Debug.Log("📋 사용 가능한 기능:");
                Debug.Log("   - 스테이지 선택");
                Debug.Log("   - 게임 시작");
                Debug.Log("   - 설정");
            }
            
            OnSceneModeChanged?.Invoke("MainMenu");
            
            // GameStateManager 상태 변경
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.SetGameState(GameState.MainMenu);
            }
            
            // 여기에 메인메뉴 UI 표시 로직 추가
            ShowMainMenuUI();
        }
        
        /// <summary>
        /// 프로토타입 모드: 스테이지 선택 모드로 전환
        /// </summary>
        private void LoadStageSelectMode()
        {
            if (enableDebugLogs)
            {
                Debug.Log("프로토타입 모드: 스테이지 선택 모드로 전환");
            }
            
            OnSceneModeChanged?.Invoke("StageSelect");
            
            // GameStateManager 상태 변경
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.SetGameState(GameState.StageSelect);
            }
            
            // 여기에 스테이지 선택 UI 표시 로직 추가
            ShowStageSelectUI();
        }
        
        /// <summary>
        /// 프로토타입 모드: 게임 모드로 전환
        /// </summary>
        private void LoadGameMode(int stageID)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"프로토타입 모드: 게임 모드로 전환 (스테이지 {stageID})");
            }
            
            // 현재 스테이지 ID 저장
            currentStageID = stageID;
            
            OnSceneModeChanged?.Invoke("GameScene");
            
            // GameStateManager에서 스테이지 시작
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.StartStage(stageID);
            }
            else
            {
                Debug.LogWarning("GameStateManager를 찾을 수 없습니다!");
            }
            
            // 여기에 게임 UI 표시 로직 추가
            ShowGameUI();
        }
        
        /// <summary>
        /// 프로토타입 모드: 결과 모드로 전환
        /// </summary>
        private void LoadResultMode(int score, bool isWin)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"프로토타입 모드: 결과 모드로 전환 (점수: {score}, 승리: {isWin})");
            }
            
            OnSceneModeChanged?.Invoke("ResultScene");
            
            // 여기에 결과 UI 표시 로직 추가
            ShowResultUI(score, isWin);
        }
        
        /// <summary>
        /// 프로토타입 모드: 현재 모드 다시 로드
        /// </summary>
        private void ReloadCurrentMode()
        {
            if (enableDebugLogs)
            {
                Debug.Log("프로토타입 모드: 현재 모드 다시 로드");
            }
            
            // 현재 상태에 따라 적절한 모드로 전환
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                switch (gameStateManager.CurrentGameState)
                {
                    case GameState.MainMenu:
                        LoadMainMenuMode();
                        break;
                    case GameState.StageSelect:
                        LoadStageSelectMode();
                        break;
                    case GameState.Playing:
                        LoadGameMode(gameStateManager.CurrentStageID);
                        break;
                    default:
                        LoadMainMenuMode();
                        break;
                }
            }
        }
        
        /// <summary>
        /// 프로토타입 모드: 특정 씬 모드로 전환
        /// </summary>
        private void LoadSceneMode(string sceneName)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"프로토타입 모드: {sceneName} 모드로 전환");
            }
            
            OnSceneModeChanged?.Invoke(sceneName);
            
            // 씬 이름에 따라 적절한 모드로 전환
            switch (sceneName.ToLower())
            {
                case "mainmenu":
                    LoadMainMenuMode();
                    break;
                case "stageselect":
                    LoadStageSelectMode();
                    break;
                case "gamescene":
                    LoadGameMode(1);
                    break;
                case "resultscene":
                    LoadResultMode(0, true);
                    break;
                default:
                    Debug.LogWarning($"알 수 없는 씬 이름: {sceneName}");
                    break;
            }
        }
        
        #endregion
        
        #region UI Helper Methods (프로토타입용)
        
        /// <summary>
        /// 메인메뉴 UI 표시 (프로토타입용)
        /// </summary>
        private void ShowMainMenuUI()
        {
            // 기존 UI 정리
            ClearAllUI();
            
            // 메인메뉴 UI 생성
            CreateMainMenuUI();
            
            Debug.Log("메인메뉴 UI 표시 완료");
        }
        
        /// <summary>
        /// 스테이지 선택 UI 표시 (프로토타입용)
        /// </summary>
        private void ShowStageSelectUI()
        {
            // 기존 UI 정리
            ClearAllUI();
            
            // 스테이지 선택 UI 생성
            CreateStageSelectUI();
            
            Debug.Log("스테이지 선택 UI 표시 완료");
        }
        
        /// <summary>
        /// 게임 UI 표시 (프로토타입용)
        /// </summary>
        private void ShowGameUI()
        {
            // 기존 UI 정리
            ClearAllUI();
            
            // 게임 UI 생성
            CreateGameUI();
            
            Debug.Log("게임 UI 표시 완료");
        }
        
        /// <summary>
        /// 모든 UI를 정리합니다.
        /// </summary>
        private void ClearAllUI()
        {
            // UI 참조 정리
            stageTextReference = null;
            
            // Canvas에서 UI 요소들 제거 (태그 대신 이름으로 검색)
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name != "LoadingCanvas" && canvas.name != "TransitionCanvas")
                {
                    Destroy(canvas.gameObject);
                }
            }
        }
        
        /// <summary>
        /// 게임 UI를 생성합니다.
        /// </summary>
        private void CreateGameUI()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("GameCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 게임 UI 패널
            GameObject panelObj = new GameObject("GamePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.3f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.8f);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // 스테이지 정보 텍스트
            GameObject stageTextObj = new GameObject("StageText");
            stageTextObj.transform.SetParent(panelObj.transform, false);
            
            // RectTransform 설정
            RectTransform stageTextRect = stageTextObj.GetComponent<RectTransform>();
            if (stageTextRect == null)
            {
                stageTextRect = stageTextObj.AddComponent<RectTransform>();
            }
            
            stageTextRect.anchorMin = new Vector2(0.05f, 0.1f);
            stageTextRect.anchorMax = new Vector2(0.4f, 0.9f);
            stageTextRect.offsetMin = Vector2.zero;
            stageTextRect.offsetMax = Vector2.zero;
            
            // Text 컴포넌트 추가
            TextMeshProUGUI stageText = stageTextObj.AddComponent<TextMeshProUGUI>();
            
            // StageText 참조 저장
            stageTextReference = stageText;
            
            // 현재 스테이지 ID 가져오기
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            int currentStageID = gameStateManager != null ? gameStateManager.CurrentStageID : 1;
            stageText.text = $"Stage {currentStageID}";
            
            // 폰트 설정 (안전하게)
            try
            {
                TMP_FontAsset fontAsset = LoadNotoSansKRFont();
                if (fontAsset != null)
                {
                    stageText.font = fontAsset;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"스테이지 텍스트 폰트 로드 실패: {e.Message}");
            }
            
            stageText.fontSize = 20;
            stageText.color = Color.white;
            stageText.alignment = TextAlignmentOptions.Left;
            
            // 일시정지 버튼
            CreateButton(panelObj, "PauseButton", "일시정지", new Vector2(0.875f, 0.5f), () => {
                Debug.Log("게임 일시정지/재개 토글");
                GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
                if (gameStateManager != null)
                {
                    gameStateManager.TogglePause();
                    
                    // 버튼 텍스트 업데이트
                    GameObject pauseButton = GameObject.Find("PauseButton");
                    if (pauseButton != null)
                    {
                        TextMeshProUGUI buttonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = gameStateManager.CurrentGameState == GameState.Paused ? "재개" : "일시정지";
                        }
                    }
                }
            });
            
            // 결과 화면으로 넘어가는 버튼 (테스트용)
            CreateButton(panelObj, "ResultButton", "결과화면", new Vector2(0.875f, 0.3f), () => {
                Debug.Log("결과 화면으로 이동");
                LoadResultScene(1500, true); // 테스트용 점수와 승리 상태
            });
            
            Debug.Log($"게임 UI 생성 완료 - 스테이지 {currentStageID}");
        }
        
        /// <summary>
        /// 스테이지 텍스트를 업데이트합니다.
        /// </summary>
        /// <param name="stageID">새로운 스테이지 ID</param>
        public void UpdateStageText(int stageID)
        {
            // 저장된 참조가 있으면 사용
            if (stageTextReference != null)
            {
                stageTextReference.text = $"Stage {stageID}";
                Debug.Log($"스테이지 텍스트 업데이트: Stage {stageID}");
                return;
            }
            
            // 저장된 참조가 없으면 GameObject.Find 사용 (백업)
            GameObject stageTextObj = GameObject.Find("StageText");
            if (stageTextObj != null)
            {
                TextMeshProUGUI stageText = stageTextObj.GetComponent<TextMeshProUGUI>();
                if (stageText != null)
                {
                    stageText.text = $"Stage {stageID}";
                    stageTextReference = stageText; // 참조 저장
                    Debug.Log($"스테이지 텍스트 업데이트: Stage {stageID}");
                }
            }
            else
            {
                Debug.LogWarning("StageText 오브젝트를 찾을 수 없습니다! UI가 아직 생성되지 않았을 수 있습니다.");
            }
        }
        
        /// <summary>
        /// 결과 UI 표시 (프로토타입용)
        /// </summary>
        private void ShowResultUI(int score, bool isWin)
        {
            // 기존 UI 정리
            ClearAllUI();
            
            // 결과 UI 생성
            CreateResultUI(score, isWin);
            
            Debug.Log($"결과 UI 표시 완료 - 점수: {score}, 승리: {isWin}");
        }
        
        /// <summary>
        /// 결과 UI를 생성합니다.
        /// </summary>
        private void CreateResultUI(int score, bool isWin)
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("ResultCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 0.9f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 결과 제목
            GameObject resultTitleObj = new GameObject("ResultTitle");
            resultTitleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI resultTitleText = resultTitleObj.AddComponent<TextMeshProUGUI>();
            resultTitleText.text = isWin ? "승리!" : "패배...";
            
            // NotoSansKR 폰트 로드
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                resultTitleText.font = fontAsset;
            }
            
            resultTitleText.fontSize = 48;
            resultTitleText.color = isWin ? Color.yellow : Color.red;
            resultTitleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform resultTitleRect = resultTitleObj.GetComponent<RectTransform>();
            resultTitleRect.anchorMin = new Vector2(0.2f, 0.7f);
            resultTitleRect.anchorMax = new Vector2(0.8f, 0.9f);
            resultTitleRect.offsetMin = Vector2.zero;
            resultTitleRect.offsetMax = Vector2.zero;
            
            // 점수 표시
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = $"점수: {score}";
            
            // NotoSansKR 폰트 로드
            TMP_FontAsset scoreFontAsset = LoadNotoSansKRFont();
            if (scoreFontAsset != null)
            {
                scoreText.font = scoreFontAsset;
            }
            
            scoreText.fontSize = 32;
            scoreText.color = Color.white;
            scoreText.alignment = TextAlignmentOptions.Center;
            
            RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0.2f, 0.5f);
            scoreRect.anchorMax = new Vector2(0.8f, 0.65f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;
            
            // 버튼들
            CreateButton(canvasObj, "RetryButton", "다시하기", new Vector2(0.3f, 0.3f), () => LoadGameScene(currentStageID));
            CreateButton(canvasObj, "MainMenuButton", "메인메뉴", new Vector2(0.7f, 0.3f), () => LoadMainMenu());
        }
        
        /// <summary>
        /// 메인메뉴 UI를 생성합니다.
        /// </summary>
        private void CreateMainMenuUI()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 0.9f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Sky Power";
            
            // NotoSansKR 폰트 로드
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 48;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.7f);
            titleRect.anchorMax = new Vector2(0.8f, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 스테이지 선택 버튼
            CreateButton(canvasObj, "StageSelectButton", "스테이지 선택", new Vector2(0.5f, 0.5f), () => LoadStageSelect());
            
            // 게임 시작 버튼
            CreateButton(canvasObj, "StartGameButton", "게임 시작", new Vector2(0.5f, 0.35f), () => LoadGameScene(1));
            
            // 종료 버튼
            CreateButton(canvasObj, "QuitButton", "게임 종료", new Vector2(0.5f, 0.2f), () => QuitGame());
        }
        
        /// <summary>
        /// 스테이지 선택 UI를 생성합니다.
        /// </summary>
        private void CreateStageSelectUI()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("StageSelectCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 배경 패널
            GameObject bgPanelObj = new GameObject("BackgroundPanel");
            bgPanelObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = bgPanelObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.3f, 0.9f);
            
            RectTransform bgRect = bgPanelObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 제목 텍스트
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "스테이지 선택";
            
            // NotoSansKR 폰트 로드
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                titleText.font = fontAsset;
            }
            
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.2f, 0.8f);
            titleRect.anchorMax = new Vector2(0.8f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // 스테이지 버튼들
            for (int i = 1; i <= 3; i++)
            {
                int stageID = i;
                CreateButton(canvasObj, $"Stage{i}Button", $"Stage {i}", 
                    new Vector2(0.2f + (i-1) * 0.3f, 0.4f), 
                    () => LoadGameScene(stageID));
            }
            
            // 뒤로가기 버튼
            CreateButton(canvasObj, "BackButton", "뒤로가기", new Vector2(0.5f, 0.1f), () => LoadMainMenu());
        }
        
        /// <summary>
        /// 버튼을 생성하는 헬퍼 메서드입니다.
        /// </summary>
        private void CreateButton(GameObject parent, string name, string text, Vector2 anchorPosition, System.Action onClick)
        {
            try
            {
                Debug.Log($"CreateButton 시작: {name}");
                
                if (parent == null)
                {
                    Debug.LogError("CreateButton: parent가 null입니다!");
                    return;
                }
                
                // 1. 버튼 GameObject 생성
                GameObject buttonObj = new GameObject(name);
                buttonObj.transform.SetParent(parent.transform, false);
                
                // 2. RectTransform 설정 (UI 요소에 필수)
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    rectTransform = buttonObj.AddComponent<RectTransform>();
                }
                
                rectTransform.anchorMin = anchorPosition - new Vector2(0.1f, 0.05f);
                rectTransform.anchorMax = anchorPosition + new Vector2(0.1f, 0.05f);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                // 3. Image 컴포넌트 추가 (버튼 배경)
                Image buttonImage = buttonObj.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                
                // 4. Button 컴포넌트 추가
                Button button = buttonObj.AddComponent<Button>();
                
                // 5. Text GameObject 생성 (별도 오브젝트로)
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(buttonObj.transform, false);
                
                // 6. Text RectTransform 설정
                RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
                if (textRectTransform == null)
                {
                    textRectTransform = textObj.AddComponent<RectTransform>();
                }
                
                textRectTransform.anchorMin = Vector2.zero;
                textRectTransform.anchorMax = Vector2.one;
                textRectTransform.offsetMin = Vector2.zero;
                textRectTransform.offsetMax = Vector2.zero;
                
                // 7. Text 컴포넌트 추가
                TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
                buttonText.text = text;
                buttonText.fontSize = 18;
                buttonText.color = Color.white;
                buttonText.alignment = TextAlignmentOptions.Center;
                
                // 8. 폰트 설정 (안전하게)
                try
                {
                    TMP_FontAsset fontAsset = LoadNotoSansKRFont();
                    if (fontAsset != null)
                    {
                        buttonText.font = fontAsset;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"폰트 로드 실패: {e.Message}");
                }
                
                // 9. 버튼 이벤트 설정
                if (onClick != null)
                {
                    button.onClick.AddListener(() => onClick());
                }
                
                Debug.Log($"CreateButton 완료: {name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CreateButton 오류 ({name}): {e.Message}\n{e.StackTrace}");
            }
        }
        
        #endregion
        
        #region Async Loading
        
        /// <summary>
        /// 씬을 비동기로 로드합니다.
        /// </summary>
        /// <param name="sceneName">로드할 씬 이름</param>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (IsLoading)
            {
                Debug.LogWarning("이미 씬 로딩 중입니다.");
                yield break;
            }
            
            IsLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            
            if (enableDebugLogs)
            {
                Debug.Log($"씬 로딩 시작: {sceneName}");
            }
            
            // 로딩 화면 표시
            ShowLoadingScreen();
            
            float startTime = Time.time;
            
            // 씬 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false; // 자동 전환 방지
            
            // 로딩 진행률 업데이트
            while (asyncLoad.progress < 0.9f)
            {
                float progress = asyncLoad.progress / 0.9f;
                UpdateLoadingProgress(progress);
                yield return null;
            }
            
            // 최소 로딩 시간 보장
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < minLoadingTime)
            {
                float remainingTime = minLoadingTime - elapsedTime;
                while (remainingTime > 0)
                {
                    remainingTime -= Time.deltaTime;
                    UpdateLoadingProgress(0.9f + (remainingTime / minLoadingTime) * 0.1f);
                    yield return null;
                }
            }
            
            // 로딩 완료
            UpdateLoadingProgress(1f);
            yield return new WaitForSeconds(0.1f); // 잠시 대기
            
            // 씬 활성화
            asyncLoad.allowSceneActivation = true;
            
            // 씬 전환이 완료될 때까지 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            // 로딩 화면 숨김
            HideLoadingScreen();
            
            IsLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            
            if (enableDebugLogs)
            {
                Debug.Log($"씬 로딩 완료: {sceneName}");
            }
        }
        
        /// <summary>
        /// 로딩 진행률을 업데이트합니다.
        /// </summary>
        /// <param name="progress">진행률 (0~1)</param>
        private void UpdateLoadingProgress(float progress)
        {
            if (progressBar != null)
                progressBar.value = progress;
            
            if (progressText != null)
                progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            
            OnLoadingProgressChanged?.Invoke(progress);
        }
        
        /// <summary>
        /// 로딩 화면을 표시합니다.
        /// </summary>
        private void ShowLoadingScreen()
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
                
                // 페이드 인 효과
                if (useFadeTransition)
                {
                    StartCoroutine(FadeInLoadingScreen());
                }
            }
        }
        
        /// <summary>
        /// 로딩 화면을 숨깁니다.
        /// </summary>
        private void HideLoadingScreen()
        {
            if (loadingScreen != null)
            {
                // 페이드 아웃 효과
                if (useFadeTransition)
                {
                    StartCoroutine(FadeOutLoadingScreen());
                }
                else
                {
                    loadingScreen.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 로딩 화면 페이드 인 효과
        /// </summary>
        private IEnumerator FadeInLoadingScreen()
        {
            CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = loadingScreen.AddComponent<CanvasGroup>();
            
            canvasGroup.alpha = 0f;
            
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// 로딩 화면 페이드 아웃 효과
        /// </summary>
        private IEnumerator FadeOutLoadingScreen()
        {
            CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = loadingScreen.AddComponent<CanvasGroup>();
            
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = 1f - (elapsed / fadeDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            loadingScreen.SetActive(false);
        }
        
        #endregion
        
        #region Scene Events
        
        /// <summary>
        /// 씬이 로드되었을 때 호출됩니다.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"씬 로드됨: {scene.name}");
            }
            
            // 씬별 초기화
            switch (scene.name)
            {
                case "GameScene":
                    InitializeGameScene();
                    break;
                
                case "MainMenu":
                    InitializeMainMenu();
                    break;
                
                case "StageSelect":
                    InitializeStageSelect();
                    break;
            }
        }
        
        /// <summary>
        /// 씬이 언로드되었을 때 호출됩니다.
        /// </summary>
        private void OnSceneUnloaded(Scene scene)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"씬 언로드됨: {scene.name}");
            }
        }
        
        #endregion
        
        #region Scene Initialization
        
        /// <summary>
        /// 게임 씬 초기화
        /// </summary>
        private void InitializeGameScene()
        {
            // 선택된 스테이지 ID 가져오기
            int selectedStageID = PlayerPrefs.GetInt("SelectedStageID", 1);
            
            // GameStateManager 찾아서 스테이지 시작
            GameStateManager gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                gameStateManager.StartStage(selectedStageID);
            }
            else
            {
                Debug.LogWarning("GameStateManager를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 메인메뉴 초기화
        /// </summary>
        private void InitializeMainMenu()
        {
            // 메인메뉴 관련 초기화
            if (enableDebugLogs)
            {
                Debug.Log("메인메뉴 초기화 완료");
            }
        }
        
        /// <summary>
        /// 스테이지 선택 화면 초기화
        /// </summary>
        private void InitializeStageSelect()
        {
            // 스테이지 선택 화면 초기화
            if (enableDebugLogs)
            {
                Debug.Log("스테이지 선택 화면 초기화 완료");
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// 게임을 종료합니다.
        /// </summary>
        public void QuitGame()
        {
            if (enableDebugLogs)
            {
                Debug.Log("게임 종료");
            }
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// 현재 씬 이름을 가져옵니다.
        /// </summary>
        /// <returns>현재 씬 이름</returns>
        public string GetCurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }
        
        /// <summary>
        /// 씬이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="sceneName">확인할 씬 이름</param>
        /// <returns>존재하면 true</returns>
        public bool DoesSceneExist(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                if (sceneNameFromPath == sceneName)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// 프로토타입 모드를 토글합니다.
        /// </summary>
        public void TogglePrototypeMode()
        {
            useCurrentSceneForTesting = !useCurrentSceneForTesting;
            if (enableDebugLogs)
            {
                Debug.Log($"프로토타입 모드 토글: {useCurrentSceneForTesting}");
            }
        }
        
        /// <summary>
        /// NotoSansKR 폰트를 로드하는 헬퍼 메서드입니다.
        /// </summary>
        private TMP_FontAsset LoadNotoSansKRFont()
        {
            // 방법 1: Inspector에서 할당된 폰트 사용 (가장 안전한 방법)
            if (notoSansKRFont != null)
            {
                Debug.Log("Inspector에서 할당된 NotoSansKR 폰트 사용");
                return notoSansKRFont;
            }
            
            TMP_FontAsset fontAsset = null;
            
            // 방법 2: Resources 폴더에서 로드 시도
            fontAsset = Resources.Load<TMP_FontAsset>("RnD/Font/NotoSansKR-VariableFont_wght SDF");
            
            if (fontAsset == null)
            {
                // 방법 3: 다른 Resources 경로들 시도
                fontAsset = Resources.Load<TMP_FontAsset>("Fonts/NotoSansKR-VariableFont_wght SDF");
            }
            
            if (fontAsset == null)
            {
                // 방법 4: 직접 경로로 로드 시도
                fontAsset = Resources.Load<TMP_FontAsset>("NotoSansKR-VariableFont_wght SDF");
            }
            
            if (fontAsset == null)
            {
                // 방법 5: AssetDatabase 사용 (에디터에서만 작동)
                #if UNITY_EDITOR
                fontAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/YSK/RnD/Font/NotoSansKR-VariableFont_wght SDF.asset");
                #endif
            }
            
            if (fontAsset == null)
            {
                Debug.LogWarning("NotoSansKR 폰트를 찾을 수 없습니다. TMPro 기본 폰트를 사용합니다.");
                // TMPro 기본 폰트 사용
                fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            }
            else
            {
                Debug.Log("NotoSansKR 폰트 로드 성공!");
            }
            
            return fontAsset;
        }
        
        #endregion
    }
}
