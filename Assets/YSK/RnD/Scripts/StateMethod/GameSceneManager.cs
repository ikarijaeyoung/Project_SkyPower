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
        [SerializeField] private Color loadingBackgroundColor = Color.black;
        [SerializeField] private Color loadingTextColor = Color.white;
        [SerializeField] private Color progressBarColor = Color.blue;
        [SerializeField] private string loadingText = "Loading...";
        [SerializeField] private bool showProgressPercentage = true;
        [SerializeField] private bool showProgressBar = true;
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        
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
            
            // Bootstrap 씬에서 시작하는 경우
            if (CurrentSceneName == "RndBootstrapScene")
            {
                Debug.Log("Bootstrap 씬에서 시작");
                EnsureEventSystemExists();
                StartCoroutine(LoadMainMenuAdditive());
            }
            // 씬 시작 시 로딩 화면 표시 옵션
            else if (showLoadingOnSceneStart && enableLoadingScreen)
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
            if (!enableLoadingScreen || loadingScreen == null) return;
            
            loadingScreen.SetActive(true);
            if (loadingCanvasGroup != null)
            {
                loadingCanvasGroup.alpha = 1f;
            }
            if (progressBar != null) progressBar.value = 0f;
            if (progressText != null) progressText.text = $"{loadingText} 0%";
            if (loadingTextComponent != null) loadingTextComponent.text = loadingText;
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
            
            if (progressBar != null && showProgressBar) progressBar.value = progress;
            if (progressText != null && showProgressPercentage) 
            {
                progressText.text = $"{loadingText} {Mathf.RoundToInt(progress * 100)}%";
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
        public void LoadBootstrap() => LoadGameScene("RndBootstrapScene");
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
                yield break;
            }
            
            IsLoading = true;
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // 씬 정보 가져오기
            SceneData.SceneInfo sceneInfo = sceneData.GetSceneInfo(sceneName);
            bool showLoadingScreen = (sceneInfo?.requiresLoadingScreen ?? true) && enableLoadingScreen;
            float sceneMinLoadingTime = sceneInfo?.minLoadingTime ?? minLoadingTime;
            
            if (showLoadingScreen)
            {
                ShowLoadingScreen();
            }
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            if (asyncLoad == null) yield break;
            
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
            // 커스텀 로딩 화면 프리팹이 있으면 사용
            if (customLoadingScreenPrefab != null)
            {
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
                
                return;
            }
            
            // 기본 로딩 화면 생성
            GameObject canvasObj = new GameObject("LoadingCanvas");
            Canvas loadingCanvas = canvasObj.AddComponent<Canvas>();
            loadingCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            loadingCanvas.sortingOrder = 1000;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            GameObject loadingObj = new GameObject("LoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            loadingCanvasGroup = loadingObj.AddComponent<CanvasGroup>();
            loadingCanvasGroup.alpha = 1f;
            
            Image bgImage = loadingObj.AddComponent<Image>();
            bgImage.color = loadingBackgroundColor;
            
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
            
            // Progress Bar (옵션)
            if (showProgressBar)
            {
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
            }
            
            // Progress Text (옵션)
            if (showProgressPercentage)
            {
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
            }
            
            loadingScreen = loadingObj;
            loadingScreen.SetActive(false);
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
            
            // 카테고리별 공통 처리
            HandleSceneCategory(sceneInfo.sceneCategory);
        }
        
        private void HandleSceneCategory(SceneData.SceneCategory category)
        {
            switch (category)
            {
                case SceneData.SceneCategory.Bootstrap:
                    Debug.Log("Bootstrap 씬 초기화");
                    break;
                case SceneData.SceneCategory.Menu:
                    Debug.Log("메뉴 씬 초기화");
                    break;
                case SceneData.SceneCategory.Game:
                    Debug.Log("게임 씬 초기화");
                    ConnectStageManagers();
                    break;
                case SceneData.SceneCategory.UI:
                    Debug.Log("UI 씬 초기화");
                    break;
                case SceneData.SceneCategory.Test:
                    Debug.Log("테스트 씬 초기화");
                    break;
            }
        }
        
        private void ConnectStageManagers()
        {
            GameStateManager gameStateManager = GameStateManager.Instance;
            StageManager stageManager = FindObjectOfType<StageManager>();
            
            if (gameStateManager != null && stageManager != null)
            {
                stageManager.SetGameStateManager(gameStateManager);
                gameStateManager.SetStageManager(stageManager);
                
                StageTransition stageTransition = stageManager.GetComponentInChildren<StageTransition>();
                if (stageTransition != null)
                {
                    gameStateManager.SetStageTransition(stageTransition);
                    stageManager.SetStageTransition(stageTransition);
                }
                
                StartCoroutine(DelayedGameStateSetup(gameStateManager));
            }
        }
        
        private IEnumerator DelayedGameStateSetup(GameStateManager gameStateManager)
        {
            yield return new WaitForSeconds(0.2f);
            gameStateManager.SetGameState(GameState.Playing);
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
        
        #region Bootstrap Loading
        
        private IEnumerator LoadMainMenuAdditive()
        {
            if (sceneData == null)
            {
                Debug.LogError("SceneData가 할당되지 않았습니다!");
                yield break;
            }
            
            Debug.Log("=== Bootstrap 로딩 시스템 시작 ===");
            
            GameObject bootstrapLoadingScreen = CreateBootstrapLoadingScreen();
            yield return new WaitForSeconds(0.1f);
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("RnDMainMenu", LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                Debug.LogError("메인메뉴 씬 로드에 실패했습니다!");
                if (bootstrapLoadingScreen != null) Destroy(bootstrapLoadingScreen);
                yield break;
            }
            
            asyncLoad.allowSceneActivation = false;
            
            while (asyncLoad.progress < 0.9f)
            {
                UpdateBootstrapLoadingProgress(bootstrapLoadingScreen, asyncLoad.progress / 0.9f);
                yield return null;
            }
            
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone) yield return null;
            
            Scene loadedMainMenuScene = SceneManager.GetSceneByName("RnDMainMenu");
            if (loadedMainMenuScene.isLoaded) SceneManager.SetActiveScene(loadedMainMenuScene);
            
            yield return new WaitForSeconds(0.1f);
            
            yield return new WaitForSeconds(0.3f);
            if (bootstrapLoadingScreen != null) Destroy(bootstrapLoadingScreen);
            
            yield return new WaitForSeconds(0.2f);
            UnloadBootstrapScene();
            
            Debug.Log("=== Bootstrap 로딩 시스템 완료 ===");
        }
        
        private void UnloadBootstrapScene()
        {
            Scene bootstrapSceneInstance = SceneManager.GetSceneByName("RndBootstrapScene");
            if (bootstrapSceneInstance.isLoaded)
            {
                GameObject[] bootstrapObjects = bootstrapSceneInstance.GetRootGameObjects();
                foreach (GameObject obj in bootstrapObjects)
                {
                    if (obj.name != "PeristentManagers") Destroy(obj);
                }
                
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(bootstrapSceneInstance);
                if (unloadOperation != null) StartCoroutine(UnloadBootstrapSceneCoroutine(unloadOperation));
            }
        }
        
        private IEnumerator UnloadBootstrapSceneCoroutine(AsyncOperation unloadOperation)
        {
            while (!unloadOperation.isDone) yield return null;
            System.GC.Collect();
        }
        
        private GameObject CreateBootstrapLoadingScreen()
        {
            GameObject canvasObj = new GameObject("BootstrapLoadingCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2000;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
            
            GameObject loadingObj = new GameObject("BootstrapLoadingScreen");
            loadingObj.transform.SetParent(canvasObj.transform, false);
            
            Image bgImage = loadingObj.AddComponent<Image>();
            bgImage.color = Color.black;
            
            RectTransform bgRect = loadingObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Bootstrap Progress Bar
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
            
            // Bootstrap Progress Text
            GameObject textObj = new GameObject("BootstrapProgressText");
            textObj.transform.SetParent(loadingObj.transform, false);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Loading... 0%";
            text.font = LoadNotoSansKRFont();
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.6f);
            textRect.anchorMax = new Vector2(0.8f, 0.7f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return loadingObj;
        }
        
        private void UpdateBootstrapLoadingProgress(GameObject loadingScreen, float progress)
        {
            if (loadingScreen == null) return;
            
            Slider progressBar = loadingScreen.GetComponentInChildren<Slider>();
            if (progressBar != null) progressBar.value = progress;
            
            TextMeshProUGUI progressText = loadingScreen.GetComponentInChildren<TextMeshProUGUI>();
            if (progressText != null && progressText.name == "BootstrapProgressText")
            {
                progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            }
        }
        
        #endregion
    }
}




