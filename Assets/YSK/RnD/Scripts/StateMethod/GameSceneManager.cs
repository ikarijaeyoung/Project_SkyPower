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
        [Header("Scene Management")]
        [SerializeField] private bool enableDebugLogs = true;
        
        [Header("Loading Screen")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private float minLoadingTime = 1f;
        
        [Header("Transition Settings")]
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 0.5f;
        
        [Header("Font Settings")]
        [SerializeField] private TMP_FontAsset notoSansKRFont;
        
        /// <summary>
        /// 씬 타입 열거형
        /// </summary>
        public enum SceneType
        {
            Bootstrap,
            MainMenu,
            MainStageSelect,
            SubStageSelect,
            BaseStage,
            EndlessStage,
            Store,
            Party,
            Result,
            NewScene
            
        }
        
        // 씬 이름 딕셔너리
        private Dictionary<SceneType, string> sceneNames = new Dictionary<SceneType, string>
        {
            { SceneType.Bootstrap, "RndBootstrapScene" },
            { SceneType.MainMenu, "RnDMainMenu" },
            { SceneType.MainStageSelect, "RnDMainStageSelectScene" },
            { SceneType.SubStageSelect, "RnDSubStageSelectScene" },
            { SceneType.BaseStage, "RnDBaseStageScene" },
            { SceneType.EndlessStage, "RnDEndlessStageScene" },
            { SceneType.Store, "RnDStoreScene" },
            { SceneType.Party, "RnDPartyScene" },
            { SceneType.Result, "RnDResultScene" },
            {SceneType.NewScene, "TestDummyMainScene" }
        };
        
        // 싱글톤 패턴
        public static GameSceneManager Instance { get; private set; }
        
        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        public static event Action<float> OnLoadingProgressChanged;
        
        // 프로퍼티
        public bool IsLoading { get; private set; }
        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        
        // 로딩 화면 관련
        private GameObject loadingScreen;
        private TextMeshProUGUI pressAnyKeyText;
        private GameObject pressAnyKeyObject;
        private bool waitingForKeyPress = false;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // 싱글톤 설정
            if (Instance == null)
            {
                Instance = this;
                
                // PeristentManagers 하위에 있다면 부모도 DontDestroyOnLoad로 설정
                if (transform.parent != null && transform.parent.name == "PeristentManagers")
                {
                    DontDestroyOnLoad(transform.parent.gameObject);
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
                
                InitializeSceneManager();
            }
            else
            {
                // 이미 인스턴스가 존재하면 파괴
                if (transform.parent != null && transform.parent.name == "PeristentManagers")
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void Start()
        {
            // Bootstrap 씬에서 시작하는 경우
            if (CurrentSceneName == GetSceneName(SceneType.Bootstrap))
            {
                // EventSystem 초기 생성
                EnsureEventSystemExists();
                
                if (enableDebugLogs)
                {
                    Debug.Log("Bootstrap 씬에서 시작 - 단순화된 로딩 시스템");
                }
                
                // UIFactory 초기화 확인
                EnsureUIFactoryExists();
                
                // 로딩화면 표시 후 메인메뉴 씬을 Additive로 로드
                StartCoroutine(LoadMainMenuAdditive());
            }
            else
            {
                // 다른 씬에서 시작하는 경우
                if (enableDebugLogs)
                {
                    Debug.Log($"다른 씬에서 시작: {CurrentSceneName}");
                }
            }
        }
        
        private void Update()
        {
            // 로딩 화면에서 키 입력 대기 중일 때
            if (waitingForKeyPress && Input.anyKeyDown)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("키 입력 감지: 로딩 화면에서 키 입력 처리");
                }
                
                // 키 입력 대기 상태 해제
                waitingForKeyPress = false;
                
                // "아무키나 눌러주세요" 텍스트 숨김
                if (pressAnyKeyObject != null)
                {
                    pressAnyKeyObject.SetActive(false);
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
        
        #region Scene Loading Methods
        
        /// <summary>
        /// 통합 씬 로딩 메서드
        /// </summary>
        /// <param name="sceneType">로드할 씬 타입</param>
        /// <param name="mainStageID">메인 스테이지 ID (BaseStage에서만 사용)</param>
        /// <param name="subStageID">서브 스테이지 ID (BaseStage에서만 사용)</param>
        /// <param name="score">점수 (Result에서만 사용)</param>
        /// <param name="isWin">승리 여부 (Result에서만 사용)</param>
        public void LoadScene(SceneType sceneType, int mainStageID = 1, int subStageID = 1, int score = 0, bool isWin = true)
        {
            // 씬 타입별 특별 처리
            switch (sceneType)
            {
                case SceneType.BaseStage:
                    // 스테이지 정보 저장
                    PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
                    PlayerPrefs.SetInt("SelectedSubStage", subStageID);
                    PlayerPrefs.Save();
                    break;
                    
                case SceneType.Result:
                    // 결과 정보 저장
                    PlayerPrefs.SetInt("GameScore", score);
                    PlayerPrefs.SetInt("GameWin", isWin ? 1 : 0);
                    PlayerPrefs.Save();
                    break;
            }
            
            string sceneName = GetSceneName(sceneType);
            if (!string.IsNullOrEmpty(sceneName))
            {
                StartCoroutine(LoadSceneAsync(sceneName));
            }
        }
        
        /// <summary>
        /// 현재 씬을 다시 로드합니다.
        /// </summary>
        public void ReloadCurrentScene()
        {
            StartCoroutine(LoadSceneAsync(CurrentSceneName));
        }
        
        /// <summary>
        /// 특정 씬으로 이동합니다.
        /// </summary>
        /// <param name="sceneName">씬 이름</param>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
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
            
            // 씬 이름 유효성 검사
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("씬 이름이 null이거나 비어있습니다!");
                yield break;
            }
            
            // 씬이 Build Settings에 있는지 확인
            if (!DoesSceneExist(sceneName))
            {
                Debug.LogError($"씬 '{sceneName}'이 Build Settings에 추가되지 않았습니다!");
                Debug.LogError("File → Build Settings에서 씬을 추가해주세요.");
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
            if (asyncLoad == null)
            {
                Debug.LogError($"씬 '{sceneName}' 로드에 실패했습니다!");
                IsLoading = false;
                HideLoadingScreen();
                yield break;
            }
            
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
                    float additionalProgress = (minLoadingTime - remainingTime) / minLoadingTime;
                    UpdateLoadingProgress(0.9f + additionalProgress * 0.1f);
                    yield return null;
                }
            }
            
            // 로딩 완료
            UpdateLoadingProgress(1f);
            yield return new WaitForSeconds(0.1f);
            
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
            // 진행률 범위 검증
            progress = Mathf.Clamp01(progress);
            
            if (enableDebugLogs)
            {
                Debug.Log($"UpdateLoadingProgress 호출: {progress * 100:F1}% (progressBar: {(progressBar != null ? "있음" : "없음")}, progressText: {(progressText != null ? "있음" : "없음")})");
            }
            
            // 진행률 바 업데이트
            if (progressBar != null)
            {
                progressBar.value = progress;
                if (enableDebugLogs)
                {
                    Debug.Log($"진행률 바 업데이트: {progressBar.value * 100:F1}%");
                }
            }
            else
            {
                Debug.LogWarning("progressBar가 null입니다!");
            }
            
            // 진행률 텍스트 업데이트
            if (progressText != null)
            {
                progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
                if (enableDebugLogs)
                {
                    Debug.Log($"진행률 텍스트 업데이트: {progressText.text}");
                }
            }
            else
            {
                Debug.LogWarning("progressText가 null입니다!");
            }
            
            // 100%에 도달하면 "아무키나 눌러주세요" 표시
            if (progress >= 1f && pressAnyKeyObject != null && !waitingForKeyPress)
            {
                pressAnyKeyObject.SetActive(true);
                waitingForKeyPress = true;
                
                if (enableDebugLogs)
                {
                    Debug.Log("로딩 완료: 아무키나 눌러주세요");
                }
            }
            
            OnLoadingProgressChanged?.Invoke(progress);
        }
        
        /// <summary>
        /// 로딩 화면을 표시합니다.
        /// </summary>
        private void ShowLoadingScreen()
        {
            if (loadingScreen != null)
            {
                // 로딩 화면 활성화
                loadingScreen.SetActive(true);
                
                // 진행률 완전 초기화 (중요!)
                if (progressBar != null)
                {
                    progressBar.value = 0f;
                    progressBar.minValue = 0f;
                    progressBar.maxValue = 1f;
                }
                
                if (progressText != null)
                {
                    progressText.text = "Loading... 0%";
                }
                
                // "아무키나 눌러주세요" 텍스트 숨김
                if (pressAnyKeyObject != null)
                {
                    pressAnyKeyObject.SetActive(false);
                }
                
                // 키 입력 대기 상태 초기화
                waitingForKeyPress = false;
                
                // 페이드 인 효과 없이 즉시 표시 (동기적)
                CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = loadingScreen.AddComponent<CanvasGroup>();
                
                canvasGroup.alpha = 1f; // 즉시 완전히 보이게 설정
                
                if (enableDebugLogs)
                {
                    Debug.Log("로딩 화면 표시 및 진행률 완전 초기화 완료 (0%)");
                }
            }
            else
            {
                Debug.LogError("로딩 화면이 null입니다!");
            }
        }
        
        /// <summary>
        /// 로딩 화면을 페이드 인과 함께 표시합니다 (코루틴용).
        /// </summary>
        private IEnumerator ShowLoadingScreenWithFade()
        {
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(true);
                
                // 페이드 인 효과
                yield return StartCoroutine(FadeInLoadingScreen());
            }
        }
        
        /// <summary>
        /// 로딩 화면을 숨깁니다.
        /// </summary>
        private void HideLoadingScreen()
        {
            if (loadingScreen != null)
            {
                // "아무키나 눌러주세요" 텍스트 숨김
                if (pressAnyKeyObject != null)
                {
                    pressAnyKeyObject.SetActive(false);
                }
                
                // 페이드 아웃 효과
                StartCoroutine(FadeOutLoadingScreen());
            }
            
            // 키 입력 대기 상태 초기화
            waitingForKeyPress = false;
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
            SceneType sceneType = GetSceneKeyByName(scene.name);
            InitializeScene(sceneType);
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
        /// 통합 씬 초기화 메서드
        /// </summary>
        /// <param name="sceneType">초기화할 씬 타입</param>
        private void InitializeScene(SceneType sceneType)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"씬 초기화 시작: {sceneType}");
            }
            
            switch (sceneType)
            {
                case SceneType.Bootstrap:
                    // Bootstrap 씬은 특별한 처리가 필요 없음 (Start에서 처리됨)
                    break;
                    
                case SceneType.MainMenu:
                    CreateMainMenuUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("메인메뉴 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.Party:
                    CreatePartyUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("캐릭터 선택 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.MainStageSelect:
                    CreateMainStageSelectUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("메인 스테이지 선택 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.SubStageSelect:
                    CreateSubStageSelectUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("서브 스테이지 선택 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.BaseStage:
                    ConnectStageManagers();
                    CreateBaseStageUI();
                    if (enableDebugLogs)
                    {
                        int selectedMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
                        int selectedSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
                        Debug.Log($"3D 탄막 게임 씬 초기화 완료 - 스테이지 {selectedMainStage}-{selectedSubStage}");
                    }
                    break;
                    
                case SceneType.EndlessStage:
                    ConnectStageManagers();
                    CreateEndlessStageUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("무한 모드 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.Store:
                    CreateStoreUI();
                    if (enableDebugLogs)
                    {
                        Debug.Log("상점 씬 초기화 완료");
                    }
                    break;
                    
                case SceneType.Result:
                    // 결과 씬은 특별한 처리가 필요 없음
                    if (enableDebugLogs)
                    {
                        Debug.Log("결과 씬 초기화 완료");
                    }
                    break;
                    
                default:
                    if (enableDebugLogs)
                    {
                        Debug.LogWarning($"알 수 없는 씬 타입: {sceneType}");
                    }
                    break;
            }
        }
        
        #endregion
        
        #region Utility Methods
        
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
                Debug.Log($"GameSceneManager 초기화 완료");
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
            backgroundImage.color = new Color(0, 0, 0, 1f); // 불투명한 검은색 배경
            
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
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Loading... 0%";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                text.font = fontAsset;
            }
            
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.6f);
            textRect.anchorMax = new Vector2(0.8f, 0.7f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // "아무키나 눌러주세요" 텍스트
            GameObject pressKeyObj = new GameObject("PressKeyText");
            pressKeyObj.transform.SetParent(loadingObj.transform, false);
            
            TextMeshProUGUI pressKeyText = pressKeyObj.AddComponent<TextMeshProUGUI>();
            pressKeyText.text = "아무키나 눌러주세요";
            
            // 폰트 설정
            if (fontAsset != null)
            {
                pressKeyText.font = fontAsset;
            }
            
            pressKeyText.fontSize = 20;
            pressKeyText.color = Color.yellow;
            pressKeyText.alignment = TextAlignmentOptions.Center;
            
            RectTransform pressKeyRect = pressKeyObj.GetComponent<RectTransform>();
            pressKeyRect.anchorMin = new Vector2(0.2f, 0.2f);
            pressKeyRect.anchorMax = new Vector2(0.8f, 0.3f);
            pressKeyRect.offsetMin = Vector2.zero;
            pressKeyRect.offsetMax = Vector2.zero;
            
            // 초기에는 숨김
            pressKeyObj.SetActive(false);
            
            // 참조 설정
            loadingScreen = loadingObj;
            progressBar = slider;
            progressText = text;
            pressAnyKeyText = pressKeyText;
            pressAnyKeyObject = pressKeyObj;
            
            // 초기 상태 설정
            loadingScreen.SetActive(false);
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
        /// UIFactory가 존재하는지 확인하고, 필요시 초기화합니다.
        /// </summary>
        private void EnsureUIFactoryExists()
        {
            if (UIFactory.Instance == null)
            {
                Debug.LogWarning("UIFactory.Instance가 null입니다. UIFactory를 찾아보겠습니다.");
                UIFactory uiFactory = FindObjectOfType<UIFactory>();
                if (uiFactory != null)
                {
                    Debug.Log("UIFactory를 찾았습니다. Awake가 호출될 때까지 대기합니다.");
                }
                else
                {
                    Debug.LogError("UIFactory를 찾을 수 없습니다! UIFactory 프리팹이 씬에 있어야 합니다.");
                    Debug.LogError("Bootstrap 씬의 PeristentManagers 하위에 UIFactory가 있는지 확인해주세요.");
                }
            }
            else
            {
                Debug.Log("UIFactory.Instance가 정상적으로 초기화되었습니다.");
            }
        }
        
        /// <summary>
        /// GameStateManager와 StageManager 간의 참조를 연결합니다.
        /// </summary>
        private void ConnectStageManagers()
        {
            if (enableDebugLogs)
            {
                Debug.Log("=== 스테이지 매니저 참조 연결 시작 ===");
            }
            
            // 1. GameStateManager 인스턴스 확인
            GameStateManager gameStateManager = GameStateManager.Instance;
            if (gameStateManager == null)
            {
                Debug.LogError("GameStateManager.Instance가 null입니다!");
                return;
            }
            
            // 2. 현재 씬에서 StageManager 찾기
            StageManager stageManager = FindObjectOfType<StageManager>();
            if (stageManager == null)
            {
                Debug.LogError("StageManager를 찾을 수 없습니다!");
                return;
            }
            
            // 3. 참조 연결
            stageManager.SetGameStateManager(gameStateManager);
            gameStateManager.SetStageManager(stageManager);
            
            // 4. StageTransition 연결
            StageTransition stageTransition = stageManager.GetComponentInChildren<StageTransition>();
            if (stageTransition != null)
            {
                gameStateManager.SetStageTransition(stageTransition);
                stageManager.SetStageTransition(stageTransition);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("=== 스테이지 매니저 참조 연결 완료 ===");
            }
            
            // 5. 지연 후 게임 상태 설정 (StageManager가 이벤트 구독할 시간 확보)
            StartCoroutine(DelayedGameStateSetup(gameStateManager));
        }
        
        private IEnumerator DelayedGameStateSetup(GameStateManager gameStateManager)
        {
            // StageManager가 이벤트를 구독할 시간을 확보
            yield return new WaitForSeconds(0.2f);
            
            // 게임 상태를 Playing으로 설정하여 맵 생성 트리거
            Debug.Log("게임 상태를 Playing으로 설정하여 맵 생성 트리거");
            gameStateManager.SetGameState(GameState.Playing);
        }
        
        #endregion

        #region UI Creation Methods
        
        /// <summary>
        /// 메인메뉴 UI를 생성합니다.
        /// </summary>
        private void CreateMainMenuUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("메인메뉴 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateMainMenuUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 메인메뉴 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 캐릭터 선택 UI를 생성합니다.
        /// </summary>
        private void CreatePartyUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("캐릭터 선택 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreatePartyUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 캐릭터 선택 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 메인 스테이지 선택 UI를 생성합니다.
        /// </summary>
        private void CreateMainStageSelectUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("메인 스테이지 선택 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateMainStageSelectUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 메인 스테이지 선택 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 서브 스테이지 선택 UI를 생성합니다.
        /// </summary>
        private void CreateSubStageSelectUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("서브 스테이지 선택 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateSubStageSelectUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 서브 스테이지 선택 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 3D 탄막 게임 UI를 생성합니다.
        /// </summary>
        private void CreateBaseStageUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("3D 탄막 게임 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateBaseStageUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 3D 탄막 게임 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 무한 모드 UI를 생성합니다.
        /// </summary>
        private void CreateEndlessStageUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("무한 모드 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateEndlessStageUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 무한 모드 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 상점 UI를 생성합니다.
        /// </summary>
        private void CreateStoreUI()
        {
            if (enableDebugLogs)
            {
                Debug.Log("상점 UI 생성 시작");
            }
            
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.CreateStoreUI();
                if (enableDebugLogs)
                {
                    Debug.Log("UIFactory를 통한 상점 UI 생성 완료");
                }
            }
            else
            {
                Debug.LogError("UIFactory를 찾을 수 없습니다!");
            }
        }
        
        /// <summary>
        /// 모든 UI를 정리합니다.
        /// </summary>
        private void ClearAllUI()
        {
            // Canvas 오브젝트들을 찾아서 제거
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                // Bootstrap 로딩 화면과 로딩 화면은 제외
                if (canvas.name != "BootstrapLoadingCanvas" && 
                    canvas.name != "LoadingCanvas" &&
                    canvas.name != "TransitionCanvas")
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log($"UI 정리: {canvas.name} 제거");
                    }
                    Destroy(canvas.gameObject);
                }
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("모든 UI 정리 완료");
            }
        }
        
        #endregion

        #region Scene Name Management
        
        /// <summary>
        /// 딕셔너리에서 씬 이름을 가져옵니다.
        /// </summary>
        /// <param name="sceneType">씬 타입</param>
        /// <returns>씬 이름 또는 null</returns>
        private string GetSceneName(SceneType sceneType)
        {
            if (sceneNames.TryGetValue(sceneType, out string sceneName))
            {
                return sceneName;
            }
            
            Debug.LogError($"씬 타입 '{sceneType}'를 찾을 수 없습니다!");
            return null;
        }
        
        /// <summary>
        /// 씬 이름으로 씬 타입을 찾습니다.
        /// </summary>
        /// <param name="sceneName">찾을 씬 이름</param>
        /// <returns>씬 타입 또는 Bootstrap (기본값)</returns>
        private SceneType GetSceneKeyByName(string sceneName)
        {
            foreach (var kvp in sceneNames)
            {
                if (kvp.Value == sceneName)
                {
                    return kvp.Key;
                }
            }
            return SceneType.Bootstrap; // 기본값
        }
        
        /// <summary>
        /// 현재 씬의 타입을 가져옵니다.
        /// </summary>
        /// <returns>현재 씬의 타입</returns>
        public SceneType GetCurrentSceneType()
        {
            return GetSceneKeyByName(CurrentSceneName);
        }
        
        #endregion

        private void EnsureEventSystemExists()
        {
            EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
            if (existingEventSystem == null)
            {
                Debug.Log("Bootstrap에서 EventSystem 생성");
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
            }
            else
            {
                Debug.Log("기존 EventSystem 발견, DontDestroyOnLoad로 설정");
                DontDestroyOnLoad(existingEventSystem.gameObject);
            }
        }

        #region Bootstrap Loading Methods
        
        /// <summary>
        /// Bootstrap에서 메인메뉴 씬을 Additive로 로드하는 코루틴
        /// </summary>
        private IEnumerator LoadMainMenuAdditive()
        {
            if (enableDebugLogs)
            {
                Debug.Log("=== Bootstrap 로딩 시스템 시작 ===");
            }
            
            // 1. Bootstrap 로딩 화면 생성 및 표시
            GameObject bootstrapLoadingScreen = CreateBootstrapLoadingScreen();
            yield return new WaitForSeconds(0.1f); // 로딩 화면 표시 대기
            
            // 초기 진행률을 0%로 설정
            UpdateBootstrapLoadingProgress(bootstrapLoadingScreen, 0f);
            
            if (enableDebugLogs)
            {
                Debug.Log("Bootstrap 로딩 화면 완전 표시 완료 - 0% 시작");
            }
            
            // 2. 메인메뉴 씬을 Additive로 로드 시작
            if (enableDebugLogs)
            {
                Debug.Log("메인메뉴 씬을 Additive로 로드 시작");
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GetSceneName(SceneType.MainMenu), LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Debug.LogError("메인메뉴 씬 로드에 실패했습니다!");
                Destroy(bootstrapLoadingScreen);
                yield break;
            }
            
            // 씬 로드가 완료될 때까지 자동 전환 방지
            asyncLoad.allowSceneActivation = false;
            
            // 3. 실제 씬 로딩 진행률 표시 (0~85%)
            float lastProgress = 0f;
            while (asyncLoad.progress < 0.9f)
            {
                float progress = asyncLoad.progress / 0.9f * 0.85f; // 0~85% 범위로 변환
                
                // 부드러운 진행률 업데이트 (갑작스러운 점프 방지)
                if (progress > lastProgress + 0.01f)
                {
                    UpdateBootstrapLoadingProgress(bootstrapLoadingScreen, progress);
                    lastProgress = progress;
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"Bootstrap 로딩 진행률: {Mathf.RoundToInt(progress * 100)}%");
                    }
                }
                
                yield return null;
            }
            
            // 4. 씬 로딩 완료 (85%)
            UpdateBootstrapLoadingProgress(bootstrapLoadingScreen, 0.85f);
            
            if (enableDebugLogs)
            {
                Debug.Log("씬 로딩 완료 - 85%");
            }
            
            // 5. 씬 활성화
            asyncLoad.allowSceneActivation = true;
            
            // 씬 전환이 완료될 때까지 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            // 6. 씬 로딩 완료 - 100%
            UpdateBootstrapLoadingProgress(bootstrapLoadingScreen, 1f);
            
            if (enableDebugLogs)
            {
                Debug.Log("메인메뉴 씬 로드 완료 - 100%");
            }
            
            // 7. "아무키나 눌러주세요" 표시
            ShowBootstrapPressKeyText(bootstrapLoadingScreen);
            
            // 8. 키 입력 대기 (로딩화면 비활성화용)
            bool keyPressed = false;
            while (!keyPressed)
            {
                if (Input.anyKeyDown)
                {
                    keyPressed = true;
                    if (enableDebugLogs)
                    {
                        Debug.Log("키 입력 감지: Bootstrap 로딩화면 비활성화");
                    }
                }
                yield return null;
            }
            
            // 9. 메인메뉴 씬을 활성 씬으로 설정
            Scene loadedMainMenuScene = SceneManager.GetSceneByName(GetSceneName(SceneType.MainMenu));
            if (loadedMainMenuScene.isLoaded)
            {
                SceneManager.SetActiveScene(loadedMainMenuScene);
                
                if (enableDebugLogs)
                {
                    Debug.Log("메인메뉴 씬을 활성 씬으로 설정");
                }
            }
            
            // 10. UI 생성
            yield return new WaitForSeconds(0.1f); // 씬 전환 완료 대기
            CreateMainMenuUI();
            
            // 11. Bootstrap 로딩화면 제거 (중요!)
            yield return new WaitForSeconds(0.3f); // UI 생성 완료 대기
            if (bootstrapLoadingScreen != null)
            {
                Destroy(bootstrapLoadingScreen);
                if (enableDebugLogs)
                {
                    Debug.Log("Bootstrap 로딩화면 제거 완료");
                }
            }
            
            // 12. Bootstrap 씬 언로드 (매니저들은 DontDestroyOnLoad로 보존됨)
            yield return new WaitForSeconds(0.2f); // UI 표시 완료 대기
            UnloadBootstrapScene();
            
            if (enableDebugLogs)
            {
                Debug.Log("=== Bootstrap 로딩 시스템 완료 ===");
            }
        }
        
        /// <summary>
        /// Bootstrap 씬을 안전하게 언로드합니다.
        /// 매니저들은 DontDestroyOnLoad로 보존되므로 씬만 언로드됩니다.
        /// </summary>
        private void UnloadBootstrapScene()
        {
            if (enableDebugLogs)
            {
                Debug.Log("Bootstrap 씬 언로드 시작");
            }
            
            // Bootstrap 씬 찾기
            Scene bootstrapSceneInstance = SceneManager.GetSceneByName(GetSceneName(SceneType.Bootstrap));
            if (bootstrapSceneInstance.isLoaded)
            {
                // Bootstrap 씬의 모든 오브젝트 중 매니저가 아닌 것들만 정리
                GameObject[] bootstrapObjects = bootstrapSceneInstance.GetRootGameObjects();
                foreach (GameObject obj in bootstrapObjects)
                {
                    // PeristentManagers 하위의 매니저들은 보존
                    if (obj.name != "PeristentManagers")
                    {
                        if (enableDebugLogs)
                        {
                            Debug.Log($"Bootstrap 오브젝트 제거: {obj.name}");
                        }
                        Destroy(obj);
                    }
                }
                
                // Bootstrap 씬 언로드
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(bootstrapSceneInstance);
                if (unloadOperation != null)
                {
                    StartCoroutine(UnloadBootstrapSceneCoroutine(unloadOperation));
                }
                else
                {
                    Debug.LogWarning("Bootstrap 씬 언로드에 실패했습니다.");
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.Log("Bootstrap 씬이 이미 언로드되었거나 로드되지 않았습니다.");
                }
            }
        }
        
        /// <summary>
        /// Bootstrap 씬 언로드 코루틴
        /// </summary>
        private IEnumerator UnloadBootstrapSceneCoroutine(AsyncOperation unloadOperation)
        {
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("Bootstrap 씬 언로드 완료");
            }
            
            // 메모리 정리
            System.GC.Collect();
        }
        
        /// <summary>
        /// Bootstrap 전용 로딩 화면을 생성합니다.
        /// </summary>
        private GameObject CreateBootstrapLoadingScreen()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("BootstrapLoadingCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2000; // 최상위에 표시
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            // 로딩 화면 배경
            GameObject loadingObj = new GameObject("BootstrapLoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            Image backgroundImage = loadingObj.AddComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 1f); // 불투명한 검은색 배경
            
            RectTransform bgRect = loadingObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // 진행률 바
            GameObject progressObj = new GameObject("BootstrapProgressBar");
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
            GameObject textObj = new GameObject("BootstrapProgressText");
            textObj.transform.SetParent(loadingObj.transform, false);
            
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Loading... 0%";
            
            // 폰트 설정
            TMP_FontAsset fontAsset = LoadNotoSansKRFont();
            if (fontAsset != null)
            {
                text.font = fontAsset;
            }
            
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.6f);
            textRect.anchorMax = new Vector2(0.8f, 0.7f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // "아무키나 눌러주세요" 텍스트
            GameObject pressKeyObj = new GameObject("BootstrapPressKeyText");
            pressKeyObj.transform.SetParent(loadingObj.transform, false);
            
            TextMeshProUGUI pressKeyText = pressKeyObj.AddComponent<TextMeshProUGUI>();
            pressKeyText.text = "아무키나 눌러주세요";
            
            // 폰트 설정
            if (fontAsset != null)
            {
                pressKeyText.font = fontAsset;
            }
            
            pressKeyText.fontSize = 20;
            pressKeyText.color = Color.yellow;
            pressKeyText.alignment = TextAlignmentOptions.Center;
            
            RectTransform pressKeyRect = pressKeyObj.GetComponent<RectTransform>();
            pressKeyRect.anchorMin = new Vector2(0.2f, 0.2f);
            pressKeyRect.anchorMax = new Vector2(0.8f, 0.3f);
            pressKeyRect.offsetMin = Vector2.zero;
            pressKeyRect.offsetMax = Vector2.zero;
            
            // 초기에는 숨김
            pressKeyObj.SetActive(false);
            
            // 참조 저장을 위한 태그 설정
            loadingObj.tag = "BootstrapLoadingScreen";
            
            return loadingObj;
        }
        
        /// <summary>
        /// Bootstrap 로딩 진행률을 업데이트합니다.
        /// </summary>
        /// <param name="loadingScreen">로딩 화면 오브젝트</param>
        /// <param name="progress">진행률 (0~1)</param>
        private void UpdateBootstrapLoadingProgress(GameObject loadingScreen, float progress)
        {
            if (loadingScreen == null) return;
            
            // 진행률 바 업데이트
            Slider progressBar = loadingScreen.GetComponentInChildren<Slider>();
            if (progressBar != null)
            {
                progressBar.value = progress;
            }
            
            // 진행률 텍스트 업데이트
            TextMeshProUGUI progressText = loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (progressText != null && progressText.name == "BootstrapProgressText")
            {
                progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            }
        }
        
        /// <summary>
        /// Bootstrap "아무키나 눌러주세요" 텍스트를 표시합니다.
        /// </summary>
        /// <param name="loadingScreen">로딩 화면 오브젝트</param>
        private void ShowBootstrapPressKeyText(GameObject loadingScreen)
        {
            if (loadingScreen == null) return;
            
            TextMeshProUGUI pressKeyText = loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (pressKeyText != null && pressKeyText.name == "BootstrapPressKeyText")
            {
                pressKeyText.gameObject.SetActive(true);
            }
        }
        
        #endregion
    }
}




