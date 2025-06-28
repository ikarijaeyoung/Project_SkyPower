using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using YSK;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace YSK
{
    /// <summary>
    /// 게임의 씬 전환을 관리하는 매니저 클래스입니다.
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        [Header("Scene Data")]
        [SerializeField] private SceneData sceneData;
        
        [Header("Loading Screen Settings")]
        [SerializeField] private bool enableLoadingScreen = true;
        [SerializeField] private bool showLoadingOnSceneStart = true;
        [SerializeField] private float minLoadingTime = 1f;
        [SerializeField] private TMP_FontAsset notoSansKRFont;
        
        [Header("Loading Screen UI")]
        [SerializeField] private GameObject customLoadingScreenPrefab;
        [SerializeField] private Color loadingBackgroundColor = new Color(0.1f, 0.1f, 0.3f, 0.9f);
        [SerializeField] private Color loadingTextColor = Color.white;
        [SerializeField] private Color progressBarColor = new Color(0.2f, 0.6f, 1f, 1f);
        [SerializeField] private string loadingText = "로딩 중...";
        [SerializeField] private bool showProgressPercentage = true;
        [SerializeField] private bool showProgressBar = true;
        
 
        
        
        // 싱글톤 패턴
        public static GameSceneManager Instance { get; private set; }
        
        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        //public static event Action<float> OnLoadingProgressChanged; // 사용하지 않아 Unity 경고 예외 처리를 위해 주석처리함.
        
        // 프로퍼티
        public bool IsLoading { get; private set; }
        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsLoadingScreenEnabled => enableLoadingScreen;
        
        // 로딩 화면 관련
        private GameObject loadingScreen;
        private Slider progressBar;
        private TextMeshProUGUI progressText;
        private TextMeshProUGUI loadingTextComponent;
        private CanvasGroup loadingCanvasGroup;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(transform.parent?.name == "PeristentManagers" ? transform.parent.gameObject : gameObject);
                InitializeSceneManager();
            }
            else
            {
                Destroy(transform.parent?.name == "PeristentManagers" ? transform.parent.gameObject : gameObject);
            }
        }
        
        private void Start()
        {
            if (sceneData == null)
            {
                Debug.LogError("SceneData가 할당되지 않았습니다!");
                return;
            }
            
            // 씬 시작 시 로딩 화면 표시 옵션
            if (showLoadingOnSceneStart && enableLoadingScreen)
            {
                ShowLoadingScreen();
                StartCoroutine(HideLoadingScreenAfterDelay(0.5f));
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        private void Update()
        {
  
   
            // 새로 추가할 테스트용 키 입력
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("T키: 테스트 씬 로드 (1-1 스테이지)");
                LoadTestSceneWithStage1_1();
            }
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Debug.Log("Y키: 테스트 씬 로드 (2-1 스테이지)");
                LoadTestSceneWithStage2_1();
            }
            
            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("U키: 테스트 씬 로드 (3-1 스테이지)");
                LoadTestSceneWithStage3_1();
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Unity의 기본 SceneManager.LoadScene을 사용합니다.
        /// </summary>
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
        public void LoadScene(int sceneBuildIndex) => SceneManager.LoadScene(sceneBuildIndex);
        
        /// <summary>
        /// 우리만의 특수한 씬 로드 기능 (로딩 화면, UI 자동 생성 등)
        /// </summary>
        public void LoadGameScene(string sceneName, int mainStageID = 1, int subStageID = 1, int score = 0, bool isWin = true)
        {
            HandleSceneSpecificData(sceneName, mainStageID, subStageID, score, isWin);
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        /// <summary>
        /// 로딩 화면 표시 여부를 설정합니다.
        /// </summary>
        public void SetLoadingScreenEnabled(bool enabled)
        {
            enableLoadingScreen = enabled;
            if (!enabled && loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
        }
        
        /// <summary>
        /// 씬 시작 시 로딩 화면 표시 여부를 설정합니다.
        /// </summary>
        public void SetShowLoadingOnSceneStart(bool show)
        {
            showLoadingOnSceneStart = show;
        }
        
        /// <summary>
        /// 로딩 화면을 수동으로 표시합니다.
        /// </summary>
        public void ShowLoadingScreen()
        {
            Debug.Log("=== ShowLoadingScreen 호출 ===");
            
            if (!enableLoadingScreen)
            {
                Debug.Log("로딩 화면이 비활성화되어 있습니다.");
                return;
            }
            
            if (loadingScreen == null)
            {
                Debug.LogError("로딩 화면이 생성되지 않았습니다!");
                return;
            }
            
            Debug.Log("로딩 화면 활성화");
            loadingScreen.SetActive(true);
            
            if (loadingCanvasGroup != null)
            {
                loadingCanvasGroup.alpha = 1f;
                Debug.Log($"CanvasGroup 알파값 설정: {loadingCanvasGroup.alpha}");
            }
            
            if (progressBar != null)
            {
                progressBar.value = 0f;
                Debug.Log("프로그레스바 초기화");
            }
            
            if (progressText != null)
            {
                progressText.text = $"{loadingText} 0%";
                Debug.Log($"프로그레스 텍스트 설정: {progressText.text}");
            }
            
            if (loadingTextComponent != null)
            {
                loadingTextComponent.text = loadingText;
                Debug.Log($"로딩 텍스트 설정: {loadingTextComponent.text}");
            }
            
            Debug.Log("=== ShowLoadingScreen 완료 ===");
        }
        
        /// <summary>
        /// 로딩 화면을 수동으로 숨깁니다.
        /// </summary>
        public void HideLoadingScreen()
        {
            if (loadingScreen == null) return;
            
            StartCoroutine(FadeOutLoadingScreen());
        }
        
        /// <summary>
        /// 로딩 진행률을 업데이트합니다.
        /// </summary>
        public void UpdateLoadingProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            if (progressBar != null && showProgressBar)
                progressBar.value = progress;
            if (progressText != null && showProgressPercentage)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
        
        // Unity 인스펙터 OnClick()용 메서드들
        public void LoadMainMenu() 
        {
            Debug.Log("LoadMainMenu 버튼 클릭됨!");
            LoadGameScene("RnDMainMenu");
        }

        public void LoadMainStageSelect() 
        {
            Debug.Log("LoadMainStageSelect 버튼 클릭됨!");
            LoadGameScene("RnDMainStageSelectScene");
        }

        public void LoadSubStageSelect() 
        {
            Debug.Log("LoadSubStageSelect 버튼 클릭됨!");
            LoadGameScene("RnDSubStageSelectScene");
        }

        public void LoadBaseStage() 
        {
            Debug.Log("LoadBaseStage 버튼 클릭됨!");
            LoadGameScene("RnDBaseStageScene");
        }

        public void LoadEndlessStage() 
        {
            Debug.Log("LoadEndlessStage 버튼 클릭됨!");
            LoadGameScene("RnDEndlessStageScene");
        }

        public void LoadStore() 
        {
            Debug.Log("LoadStore 버튼 클릭됨!");
            LoadGameScene("RnDStoreScene");
        }

        public void LoadParty() 
        {
            Debug.Log("LoadParty 버튼 클릭됨!");
            LoadGameScene("RnDPartyScene");
        }

        public void LoadTestScene() 
        {
            Debug.Log("LoadTestScene 버튼 클릭됨!");
            LoadGameScene("RnDBaseStageTestScene");
        }

        // 새로 추가할 테스트용 메서드들
        public void LoadTestSceneWithStage1_1() 
        {
            Debug.Log("LoadTestSceneWithStage1_1 버튼 클릭됨!");
            LoadGameSceneWithStage("RnDBaseStageTestScene", 1, 1);
        }

        public void LoadTestSceneWithStage1_2() 
        {
            Debug.Log("LoadTestSceneWithStage1_2 버튼 클릭됨!");
            LoadGameSceneWithStage("RnDBaseStageTestScene", 1, 2);
        }

        public void LoadTestSceneWithStage2_1() 
        {
            Debug.Log("LoadTestSceneWithStage2_1 버튼 클릭됨!");
            LoadGameSceneWithStage("RnDBaseStageTestScene", 2, 1);
        }

        public void LoadTestSceneWithStage2_2() 
        {
            Debug.Log("LoadTestSceneWithStage2_2 버튼 클릭됨!");
            LoadGameSceneWithStage("RnDBaseStageTestScene", 2, 2);
        }

        public void LoadTestSceneWithStage3_1() 
        {
            Debug.Log("LoadTestSceneWithStage3_1 버튼 클릭됨!");
            LoadGameSceneWithStage("RnDBaseStageTestScene", 3, 1);
        }

        public void ReloadCurrentScene() 
        {
            Debug.Log("ReloadCurrentScene 버튼 클릭됨!");
            LoadGameScene(CurrentSceneName);
        }

        public void QuitGame() 
        {
            Debug.Log("QuitGame 버튼 클릭됨!");
            Application.Quit();
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeSceneManager()
        {
            CreateLoadingScreen();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }
        
        private void HandleSceneSpecificData(string sceneName, int mainStageID, int subStageID, int score, bool isWin)
        {
            switch (sceneName)
            {
                case "RnDBaseStageScene":
                    PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
                    PlayerPrefs.SetInt("SelectedSubStage", subStageID);
                    break;
                case "RnDResultScene":
                    PlayerPrefs.SetInt("GameScore", score);
                    PlayerPrefs.SetInt("GameWin", isWin ? 1 : 0);
                    break;
            }
            PlayerPrefs.Save();
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (IsLoading || string.IsNullOrEmpty(sceneName) || !DoesSceneExist(sceneName))
            {
                Debug.LogWarning($"씬 로드 조건 불만족: IsLoading={IsLoading}, sceneName={sceneName}, exists={DoesSceneExist(sceneName)}");
                yield break;
            }
            
            Debug.Log($"=== 씬 로드 시작: {sceneName} ===");
            
            IsLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // 씬 정보 가져오기
            SceneData.SceneInfo sceneInfo = sceneData.GetSceneInfo(sceneName);
            bool showLoadingScreen = (sceneInfo?.requiresLoadingScreen ?? true) && enableLoadingScreen;
            float sceneMinLoadingTime = sceneInfo?.minLoadingTime ?? minLoadingTime;
            
            Debug.Log($"씬 정보: showLoadingScreen={showLoadingScreen}, minLoadingTime={sceneMinLoadingTime}");
            
            if (showLoadingScreen)
            {
                ShowLoadingScreen();
                // 초기 진행률 설정
                UpdateLoadingProgress(0f);
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                Debug.LogError($"씬 '{sceneName}' 로드에 실패했습니다!");
                IsLoading = false;
                yield break;
            }
            
            asyncLoad.allowSceneActivation = false;
            
            // 진행률 업데이트 루프 개선
            while (asyncLoad.progress < 0.9f)
            {
                float normalizedProgress = asyncLoad.progress / 0.9f;
                UpdateLoadingProgress(normalizedProgress);
                yield return null;
            }
            
            // 최소 로딩 시간 대기
            yield return StartCoroutine(EnsureMinimumLoadingTime(sceneMinLoadingTime));
            
            // 100% 완료 표시
            UpdateLoadingProgress(1f);
            yield return new WaitForSeconds(0.1f);
            
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone) yield return null;
            
            if (showLoadingScreen)
            {
                HideLoadingScreen();
            }
            
            IsLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            
            Debug.Log($"=== 씬 로드 완료: {sceneName} ===");
        }
        
        private IEnumerator EnsureMinimumLoadingTime(float loadingTime)
        {
            float startTime = Time.time;
            while (Time.time - startTime < loadingTime)
            {
                yield return null;
            }
        }
        
        private IEnumerator HideLoadingScreenAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideLoadingScreen();
        }
        
        private IEnumerator FadeOutLoadingScreen()
        {
            if (loadingCanvasGroup == null)
            {
                loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();
                if (loadingCanvasGroup == null)
                {
                    loadingCanvasGroup = loadingScreen.AddComponent<CanvasGroup>();
                }
            }
            
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                loadingCanvasGroup.alpha = 1f - (elapsed / 0.5f);
                yield return null;
            }
            
            loadingCanvasGroup.alpha = 0f;
            loadingScreen.SetActive(false);
        }
        
        private void CreateLoadingScreen()
        {
            Debug.Log("=== 로딩 화면 생성 시작 ===");
            
            // 커스텀 로딩 화면 프리팹이 있으면 사용
            if (customLoadingScreenPrefab != null)
            {
                Debug.Log("커스텀 로딩 화면 프리팹 사용");
                loadingScreen = Instantiate(customLoadingScreenPrefab);
                Canvas canvas = loadingScreen.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.sortingOrder = 1000;
                    DontDestroyOnLoad(canvas.gameObject);
                }
                
                // 컴포넌트들 찾기 - 더 정확한 검색
                Debug.Log("=== 컴포넌트 검색 시작 ===");
                
                // 1. Slider 컴포넌트 직접 찾기
                progressBar = loadingScreen.GetComponentInChildren<Slider>();
                if (progressBar != null)
                {
                    Debug.Log($"Slider 컴포넌트 찾음: {progressBar.name}");
                    progressBar.minValue = 0f;
                    progressBar.maxValue = 1f;
                    progressBar.value = 0f;
                }
                else
                {
                    Debug.LogWarning("Slider 컴포넌트를 찾을 수 없습니다!");
                }
                
                // 2. TextMeshProUGUI 컴포넌트들 찾기
                TextMeshProUGUI[] textComponents = loadingScreen.GetComponentsInChildren<TextMeshProUGUI>();
                Debug.Log($"TextMeshProUGUI 컴포넌트 개수: {textComponents.Length}");
                
                foreach (var text in textComponents)
                {
                    Debug.Log($"Text 컴포넌트: {text.name} - '{text.text}'");
                    
                    // 로딩 텍스트 찾기 (기본 텍스트)
                    if (loadingTextComponent == null && text.text.Contains("로딩"))
                    {
                        loadingTextComponent = text;
                        Debug.Log($"로딩 텍스트 컴포넌트 찾음: {text.name}");
                    }
                    
                    // 프로그레스 텍스트 찾기 (% 포함)
                    if (progressText == null && text.text.Contains("%"))
                    {
                        progressText = text;
                        Debug.Log($"프로그레스 텍스트 컴포넌트 찾음: {text.name}");
                    }
                }
                
                // 3. CanvasGroup 찾기
                loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();
                if (loadingCanvasGroup == null)
                {
                    loadingCanvasGroup = loadingScreen.AddComponent<CanvasGroup>();
                    Debug.Log("CanvasGroup 컴포넌트 추가");
                }
                
                // 4. 컴포넌트 검증
                Debug.Log("=== 컴포넌트 검증 ===");
                Debug.Log($"ProgressBar: {(progressBar != null ? "찾음" : "못 찾음")}");
                Debug.Log($"ProgressText: {(progressText != null ? "찾음" : "못 찾음")}");
                Debug.Log($"LoadingText: {(loadingTextComponent != null ? "찾음" : "못 찾음")}");
                Debug.Log($"CanvasGroup: {(loadingCanvasGroup != null ? "찾음" : "못 찾음")}");
                
                Debug.Log("커스텀 로딩 화면 생성 완료");
                return;
            }
            
            Debug.Log("기본 로딩 화면 생성");
            
            // 기본 로딩 화면 생성
            GameObject canvasObj = new GameObject("LoadingCanvas");
            Canvas loadingCanvas = canvasObj.AddComponent<Canvas>();
            loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            loadingCanvas.sortingOrder = 1000;
            
            // CanvasScaler 추가
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            Debug.Log($"Canvas 생성 완료: {canvasObj.name}, SortingOrder: {loadingCanvas.sortingOrder}");
            
            // 로딩 화면 배경
            GameObject loadingObj = new GameObject("LoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            // CanvasGroup 추가 및 초기화
            loadingCanvasGroup = loadingObj.AddComponent<CanvasGroup>();
            loadingCanvasGroup.alpha = 1f;
            loadingCanvasGroup.interactable = true;
            loadingCanvasGroup.blocksRaycasts = true;
            
            // 배경 이미지
            Image bgImage = loadingObj.AddComponent<Image>();
            bgImage.color = loadingBackgroundColor;
            Debug.Log($"배경 이미지 색상 설정: {loadingBackgroundColor}");
            
            RectTransform bgRect = loadingObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 로딩 텍스트
            GameObject loadingTextObj = new GameObject("LoadingText");
            loadingTextObj.transform.SetParent(loadingObj.transform, false);
            loadingTextComponent = loadingTextObj.AddComponent<TextMeshProUGUI>();
            loadingTextComponent.text = loadingText;
            loadingTextComponent.font = LoadNotoSansKRFont();
            loadingTextComponent.fontSize = 48; // 크기 증가
            loadingTextComponent.color = loadingTextColor;
            loadingTextComponent.alignment = TextAlignmentOptions.Center;
            
            RectTransform loadingTextRect = loadingTextObj.GetComponent<RectTransform>();
            loadingTextRect.anchorMin = new Vector2(0.1f, 0.6f);
            loadingTextRect.anchorMax = new Vector2(0.9f, 0.8f);
            loadingTextRect.offsetMin = Vector2.zero;
            loadingTextRect.offsetMax = Vector2.zero;
            
            Debug.Log($"로딩 텍스트 생성 완료: {loadingText}");
            
            // Progress Bar (옵션)
            if (showProgressBar)
            {
                Debug.Log("프로그레스바 생성");
                GameObject progressObj = new GameObject("ProgressBar");
                progressObj.transform.SetParent(loadingObj.transform, false);
                progressBar = progressObj.AddComponent<Slider>();
                progressBar.minValue = 0f;
                progressBar.maxValue = 1f;
                progressBar.value = 0f;
                
                // Progress Bar 배경
                Image progressBg = progressBar.GetComponent<Image>();
                if (progressBg != null)
                {
                    progressBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                }
                
                // Progress Bar Fill
                Transform fillArea = progressBar.transform.Find("Fill Area");
                if (fillArea != null)
                {
                    Transform fill = fillArea.Find("Fill");
                    if (fill != null)
                    {
                        Image fillImage = fill.GetComponent<Image>();
                        if (fillImage != null)
                        {
                            fillImage.color = progressBarColor;
                        }
                    }
                }
                
                RectTransform sliderRect = progressObj.GetComponent<RectTransform>();
                sliderRect.anchorMin = new Vector2(0.2f, 0.3f);
                sliderRect.anchorMax = new Vector2(0.8f, 0.4f);
                sliderRect.offsetMin = Vector2.zero;
                sliderRect.offsetMax = Vector2.zero;
                
                Debug.Log("프로그레스바 생성 완료");
            }
            
            // Progress Text (옵션)
            if (showProgressPercentage)
            {
                Debug.Log("프로그레스 텍스트 생성");
                GameObject textObj = new GameObject("ProgressText");
                textObj.transform.SetParent(loadingObj.transform, false);
                progressText = textObj.AddComponent<TextMeshProUGUI>();
                progressText.text = "0%";
                progressText.font = LoadNotoSansKRFont();
                progressText.fontSize = 32;
                progressText.color = loadingTextColor;
                progressText.alignment = TextAlignmentOptions.Center;
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.2f, 0.45f);
                textRect.anchorMax = new Vector2(0.8f, 0.55f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                Debug.Log("프로그레스 텍스트 생성 완료");
            }
            
            loadingScreen = loadingObj;
            
            // 초기 상태 설정 - 활성화 상태로 생성
            loadingScreen.SetActive(true);
            loadingCanvasGroup.alpha = 1f;
            
            Debug.Log($"=== 로딩 화면 생성 완료 ===\n" +
                      $"Canvas: {canvasObj.name}\n" +
                      $"LoadingScreen: {loadingScreen.name}\n" +
                      $"Active: {loadingScreen.activeInHierarchy}\n" +
                      $"CanvasGroup Alpha: {loadingCanvasGroup.alpha}\n" +
                      $"SortingOrder: {loadingCanvas.sortingOrder}");
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeScene(scene.name);
        }
        
        private void OnSceneUnloaded(Scene scene) { }
        
        private void InitializeScene(string sceneName)
        {
            SceneData.SceneInfo sceneInfo = sceneData?.GetSceneInfo(sceneName);
            if (sceneInfo == null) return;
            
            // 씬별 공통 처리
            HandleSceneInitialization(sceneName);
        }
        
        private void HandleSceneInitialization(string sceneName)
        {
            switch (sceneName)
            {
                case "RnDMainMenu":
                case "RnDMainStageSelectScene":
                case "RnDSubStageSelectScene":
                case "RnDStoreScene":
                case "RnDPartyScene":
                    Debug.Log("메뉴 씬 초기화");
                    break;
                case "RnDBaseStageScene":
                case "RnDEndlessStageScene":
                case "RnDBaseStageTestScene":
                    Debug.Log("게임 씬 초기화");
                    ConnectStageManagers();
                    break;
                default:
                    Debug.Log($"기타 씬 초기화: {sceneName}");
                    break;
            }
        }
        
        private void ConnectStageManagers()
        {
            StageManager stageManager = FindObjectOfType<StageManager>();
            
            if (stageManager != null)
            {
                Debug.Log("StageManager 발견 - 전환 기능이 통합되어 있습니다.");
            }
            else
            {
                Debug.LogWarning("StageManager를 찾을 수 없습니다!");
            }
        }
        
        private void EnsureEventSystemExists()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
            }
        }
        
        private bool DoesSceneExist(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameFromPath == sceneName) return true;
            }
            return false;
        }
        
        private TMP_FontAsset LoadNotoSansKRFont()
        {
            if (notoSansKRFont != null) return notoSansKRFont;
            
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("RnD/Font/NotoSansKR-VariableFont_wght SDF");
            if (fontAsset == null) fontAsset = Resources.Load<TMP_FontAsset>("Fonts/NotoSansKR-VariableFont_wght SDF");
            if (fontAsset == null) fontAsset = Resources.Load<TMP_FontAsset>("NotoSansKR-VariableFont_wght SDF");
            
            #if UNITY_EDITOR
            if (fontAsset == null) fontAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/YSK/RnD/Font/NotoSansKR-VariableFont_wght SDF.asset");
            #endif
            
            return fontAsset ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }
        
        
        // 특정 스테이지와 함께 씬을 로드하는 새로운 메서드
        public void LoadGameSceneWithStage(string sceneName, int mainStageID, int subStageID)
        {
            Debug.Log($"=== 특정 스테이지와 함께 씬 로드: {sceneName}, 스테이지 {mainStageID}-{subStageID} ===");
            
            // 오브젝트가 비활성화되어 있으면 활성화
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogWarning("GameSceneManager가 비활성화되어 있어서 활성화합니다.");
                gameObject.SetActive(true);
            }
            
            HandleSceneSpecificData(sceneName, mainStageID, subStageID, 0, false);
            StartCoroutine(LoadSceneAsyncWithStage(sceneName, mainStageID, subStageID));
        }
        
        // 스테이지 정보와 함께 씬을 로드하는 코루틴
        private IEnumerator LoadSceneAsyncWithStage(string sceneName, int mainStageID, int subStageID)
        {
            if (IsLoading || string.IsNullOrEmpty(sceneName) || !DoesSceneExist(sceneName))
            {
                Debug.LogWarning($"씬 로드 조건 불만족: IsLoading={IsLoading}, sceneName={sceneName}, exists={DoesSceneExist(sceneName)}");
                yield break;
            }
            
            Debug.Log($"=== 특정 스테이지와 함께 씬 로드 시작: {sceneName}, 스테이지 {mainStageID}-{subStageID} ===");
            
            IsLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // 씬 정보 가져오기
            SceneData.SceneInfo sceneInfo = sceneData.GetSceneInfo(sceneName);
            bool showLoadingScreen = (sceneInfo?.requiresLoadingScreen ?? true) && enableLoadingScreen;
            float sceneMinLoadingTime = sceneInfo?.minLoadingTime ?? minLoadingTime;
            
            Debug.Log($"씬 정보: showLoadingScreen={showLoadingScreen}, minLoadingTime={sceneMinLoadingTime}");
            
            if (showLoadingScreen)
            {
                ShowLoadingScreen();
                UpdateLoadingProgress(0f);
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                Debug.LogError($"씬 '{sceneName}' 로드에 실패했습니다!");
                IsLoading = false;
                yield break;
            }
            
            asyncLoad.allowSceneActivation = false;
            
            // 진행률 업데이트 루프
            while (asyncLoad.progress < 0.9f)
            {
                float normalizedProgress = asyncLoad.progress / 0.9f;
                UpdateLoadingProgress(normalizedProgress);
                yield return null;
            }
            
            // 최소 로딩 시간 대기
            yield return StartCoroutine(EnsureMinimumLoadingTime(sceneMinLoadingTime));
            
            // 100% 완료 표시
            UpdateLoadingProgress(1f);
            yield return new WaitForSeconds(0.1f);
            
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone) yield return null;
            
            if (showLoadingScreen)
            {
                HideLoadingScreen();
            }
            
            IsLoading = false;
            OnSceneLoadCompleted?.Invoke(sceneName);
            
            // 씬 로드 완료 후 스테이지 설정
            yield return StartCoroutine(SetStageAfterSceneLoad(mainStageID, subStageID));
            
            Debug.Log($"=== 특정 스테이지와 함께 씬 로드 완료: {sceneName}, 스테이지 {mainStageID}-{subStageID} ===");
        }
        
        // 씬 로드 완료 후 스테이지를 설정하는 코루틴
        private IEnumerator SetStageAfterSceneLoad(int mainStageID, int subStageID)
        {
            Debug.Log($"스테이지 설정 대기 중: {mainStageID}-{subStageID}");
            
            // StageManager가 준비될 때까지 대기
            StageManager stageManager = null;
            float waitTime = 0f;
            const float maxWaitTime = 5f; // 최대 5초 대기
            
            while (stageManager == null && waitTime < maxWaitTime)
            {
                stageManager = FindObjectOfType<StageManager>();
                if (stageManager == null)
                {
                    yield return new WaitForSeconds(0.1f);
                    waitTime += 0.1f;
                }
            }
            
            if (stageManager == null)
            {
                Debug.LogError("StageManager를 찾을 수 없습니다! 스테이지 설정을 건너뜁니다.");
                yield break;
            }
            
            Debug.Log($"StageManager 발견! 스테이지 설정: {mainStageID}-{subStageID}");
            
            // 한 프레임 더 대기하여 StageManager가 완전히 초기화되도록 함
            yield return null;
            
            // 스테이지 설정
            stageManager.ForceStage(mainStageID, subStageID);
            
            Debug.Log($"스테이지 설정 완료: {mainStageID}-{subStageID}");
        }
        
        #endregion
    }
}




