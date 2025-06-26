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
        
 
        
        /// <summary>
        /// 씬 타입 열거형
        /// </summary>
        // public enum SceneType { ... } 삭제
        
        // 싱글톤 패턴
        public static GameSceneManager Instance { get; private set; }
        
        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;
        public static event Action<float> OnLoadingProgressChanged;
        
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
            OnLoadingProgressChanged?.Invoke(progress);
            
            if (progressBar != null && showProgressBar)
            {
                progressBar.value = progress;
                Debug.Log($"프로그레스바 업데이트: {progress:P0}");
            }
            
            if (progressText != null && showProgressPercentage)
            {
                progressText.text = $"{loadingText} {Mathf.RoundToInt(progress * 100)}%";
                Debug.Log($"프로그레스 텍스트 업데이트: {progressText.text}");
            }
        }
        
        // Unity 인스펙터 OnClick()용 메서드들
        public void LoadMainMenu() => LoadGameScene("RnDMainMenu");
        public void LoadMainStageSelect() => LoadGameScene("RnDMainStageSelectScene");
        public void LoadSubStageSelect() => LoadGameScene("RnDSubStageSelectScene");
        public void LoadBaseStage() => LoadGameScene("RnDBaseStageScene");
        public void LoadEndlessStage() => LoadGameScene("RnDEndlessStageScene");
        public void LoadStore() => LoadGameScene("RnDStoreScene");
        public void LoadParty() => LoadGameScene("RnDPartyScene");
        public void LoadTestScene() => LoadGameScene("RnDBaseStageTestScene");
        public void ReloadCurrentScene() => LoadGameScene(CurrentSceneName);
        public void QuitGame() => Application.Quit();
        
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
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null)
            {
                Debug.LogError($"씬 '{sceneName}' 로드에 실패했습니다!");
                IsLoading = false;
                yield break;
            }
            
            asyncLoad.allowSceneActivation = false;
            
            while (asyncLoad.progress < 0.9f)
            {
                UpdateLoadingProgress(asyncLoad.progress / 0.9f);
                yield return null;
            }
            
            yield return StartCoroutine(EnsureMinimumLoadingTime(sceneMinLoadingTime));
            
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
                
                // 컴포넌트들 찾기
                progressBar = loadingScreen.GetComponentInChildren<Slider>();
                progressText = loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
                loadingCanvasGroup = loadingScreen.GetComponent<CanvasGroup>();
                
                Debug.Log("커스텀 로딩 화면 생성 완료");
                return;
            }
            
            Debug.Log("기본 로딩 화면 생성");
            
            // 기본 로딩 화면 생성
            GameObject canvasObj = new GameObject("LoadingCanvas");
            Canvas loadingCanvas = canvasObj.AddComponent<Canvas>();
            loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            loadingCanvas.sortingOrder = 1000;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            Debug.Log($"Canvas 생성 완료: {canvasObj.name}");
            
            GameObject loadingObj = new GameObject("LoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            loadingCanvasGroup = loadingObj.AddComponent<CanvasGroup>();
            loadingCanvasGroup.alpha = 1f;
            
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
            loadingTextComponent.fontSize = 32;
            loadingTextComponent.color = loadingTextColor;
            loadingTextComponent.alignment = TextAlignmentOptions.Center;
            
            RectTransform loadingTextRect = loadingTextObj.GetComponent<RectTransform>();
            loadingTextRect.anchorMin = new Vector2(0.2f, 0.7f);
            loadingTextRect.anchorMax = new Vector2(0.8f, 0.8f);
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
                sliderRect.anchorMin = new Vector2(0.2f, 0.4f);
                sliderRect.anchorMax = new Vector2(0.8f, 0.5f);
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
                progressText.text = $"{loadingText} 0%";
                progressText.font = LoadNotoSansKRFont();
                progressText.fontSize = 24;
                progressText.color = loadingTextColor;
                progressText.alignment = TextAlignmentOptions.Center;
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.2f, 0.6f);
                textRect.anchorMax = new Vector2(0.8f, 0.7f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                Debug.Log("프로그레스 텍스트 생성 완료");
            }
            
            loadingScreen = loadingObj;
            loadingScreen.SetActive(false);
            
            Debug.Log("=== 로딩 화면 생성 완료 ===");
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
                StageTransition stageTransition = stageManager.GetComponentInChildren<StageTransition>();
                if (stageTransition != null)
                {
                    stageManager.SetStageTransition(stageTransition);
                }
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
        
        #endregion
    }
}




