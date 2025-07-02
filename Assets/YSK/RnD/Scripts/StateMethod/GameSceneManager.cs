using KYG_skyPower;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace YSK
{
    /// <summary>
    /// 게임의 씬 전환을 관리하는 매니저 클래스입니다.
    /// </summary>
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        [Header("Loading Screen Settings")]
        [Tooltip("로딩 화면을 사용할지 여부 (기본값: false)")]
        [SerializeField] private bool enableLoadingScreen = false;

        [Tooltip("씬 시작 시 로딩 화면을 표시할지 여부")]
        [SerializeField] private bool showLoadingOnSceneStart = false;

        [Tooltip("최소 로딩 시간 (초)")]
        [SerializeField] private float minLoadingTime = 1f;

        [Header("Loading Screen UI")]
        [Tooltip("로딩 화면 프리팹 (null이면 로딩 화면을 사용하지 않음)")]
        [SerializeField] private GameObject customLoadingScreenPrefab;

        // 이벤트
        public static event Action<string> OnSceneLoadStarted;
        public static event Action<string> OnSceneLoadCompleted;

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

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // 씬 시작 시 로딩 화면 표시 옵션
            if (showLoadingOnSceneStart && enableLoadingScreen && customLoadingScreenPrefab != null)
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
            // 오브젝트가 비활성화되어 있으면 활성화
            if (!gameObject.activeInHierarchy)
            {
                Debug.LogWarning("GameSceneManager가 비활성화되어 있어서 활성화합니다.");
                gameObject.SetActive(true);

                // 활성화 후 잠시 대기
                StartCoroutine(LoadSceneAfterActivation(sceneName, mainStageID, subStageID, score, isWin));
                return;
            }

            HandleSceneSpecificData(sceneName, mainStageID, subStageID, score, isWin);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>
        /// 특정 스테이지와 함께 씬을 로드합니다.
        /// </summary>
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

            if (customLoadingScreenPrefab == null)
            {
                Debug.LogWarning("로딩 화면 프리팹이 설정되지 않았습니다! 로딩 화면을 건너뜁니다.");
                return;
            }

            if (loadingScreen == null)
            {
                Debug.LogWarning("로딩 화면이 생성되지 않았습니다! 로딩 화면을 건너뜁니다.");
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
                progressText.text = "0%";
                Debug.Log($"프로그레스 텍스트 설정: {progressText.text}");
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
            if (progressBar != null)
                progressBar.value = progress;
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }

        // Unity 인스펙터 OnClick()용 메서드들 - 하드코딩된 씬 이름 사용
        public void LoadTitleScene()
        {
            Debug.Log("LoadTitleScene 버튼 클릭됨!");
            LoadGameScene("aTitleScene_JYL");
        }

        public void LoadMainScene()
        {
            Debug.Log("LoadMainScene 버튼 클릭됨!");
            LoadGameScene("bMainScene_JYL");
        }

        public void LoadStoreScene()
        {
            Debug.Log("LoadStoreScene 버튼 클릭됨!");
            LoadGameScene("cStoreScene_JYL");
        }

        public void LoadStageStage()
        {
            Debug.Log("LoadStageScene 버튼 클릭됨!");
            LoadGameScene("dStageScene_JYL");
        }

        // UI를 위한 PointerEventData data 매개변수 넣은 Overloading
        public void LoadTitleScene(PointerEventData data)
        {
            Debug.Log("LoadTitleScene 버튼 클릭됨!");
            LoadGameScene("aTitleScene_JYL");
        }

        public void LoadMainScene(PointerEventData data)
        {
            Debug.Log("LoadMainScene 버튼 클릭됨!");
            LoadGameScene("bMainScene_JYL");
        }

        public void LoadStoreScene(PointerEventData data)
        {
            Debug.Log("LoadStoreScene 버튼 클릭됨!");
            LoadGameScene("cStoreScene_JYL");
        }

        public void LoadStageStage(PointerEventData data)
        {
            Debug.Log("LoadStageScene 버튼 클릭됨!");
            LoadGameScene("dStageScene_JYL");
        }

        public void ReloadCurrentStage(PointerEventData data)
        {
            Debug.Log("ReloadCurrentStage 버튼 클릭됨!");
            ReloadCurrentStage();
        }

        public void ReloadCurrentScene()
        {
            Debug.Log("ReloadCurrentScene 버튼 클릭됨!");
            LoadGameScene(CurrentSceneName);
        }

        /// <summary>
        /// 현재 스테이지를 다시 로드합니다.
        /// </summary>
        public void ReloadCurrentStage()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"GameSceneManager: 현재 스테이지 다시 로드 - {currentMainStage}-{currentSubStage}");

            // 현재 씬에서 현재 스테이지로 다시 로드
            LoadGameSceneWithStage(CurrentSceneName, currentMainStage, currentSubStage);
        }

        /// <summary>
        /// 특정 스테이지를 다시 로드합니다.
        /// </summary>
        /// <param name="mainStageID">메인 스테이지 ID</param>
        /// <param name="subStageID">서브 스테이지 ID</param>
        public void ReloadStage(int mainStageID, int subStageID)
        {
            Debug.Log($"GameSceneManager: 특정 스테이지 다시 로드 - {mainStageID}-{subStageID}");

            // 현재 씬에서 특정 스테이지로 다시 로드
            LoadGameSceneWithStage(CurrentSceneName, mainStageID, subStageID);
        }

        public void QuitGame()
        {
            Debug.Log("QuitGame 버튼 클릭됨!");
            Application.Quit();
        }

        #endregion

        #region Private Methods

        public override void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void HandleSceneSpecificData(string sceneName, int mainStageID, int subStageID, int score, bool isWin)
        {
            switch (sceneName)
            {
                case "dStageScene_JYL":
                    PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
                    PlayerPrefs.SetInt("SelectedSubStage", subStageID);
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

            bool showLoadingScreen = enableLoadingScreen && customLoadingScreenPrefab != null;

            Debug.Log($"씬 정보: showLoadingScreen={showLoadingScreen}, minLoadingTime={minLoadingTime}");

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
                if (showLoadingScreen)
                {
                    UpdateLoadingProgress(normalizedProgress);
                }
                yield return null;
            }

            // 최소 로딩 시간 대기 (로딩 화면이 있을 때만)
            if (showLoadingScreen)
            {
                yield return StartCoroutine(EnsureMinimumLoadingTime(minLoadingTime));
                UpdateLoadingProgress(1f);
                yield return new WaitForSeconds(0.1f);
            }

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

            bool showLoadingScreen = enableLoadingScreen && customLoadingScreenPrefab != null;

            Debug.Log($"씬 정보: showLoadingScreen={showLoadingScreen}, minLoadingTime={minLoadingTime}");

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
                if (showLoadingScreen)
                {
                    UpdateLoadingProgress(normalizedProgress);
                }
                yield return null;
            }

            // 최소 로딩 시간 대기 (로딩 화면이 있을 때만)
            if (showLoadingScreen)
            {
                yield return StartCoroutine(EnsureMinimumLoadingTime(minLoadingTime));
                UpdateLoadingProgress(1f);
                yield return new WaitForSeconds(0.1f);
            }

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

            // 프리팹이 없으면 생성하지 않음
            if (customLoadingScreenPrefab == null)
            {
                Debug.LogWarning("로딩 화면 프리팹이 설정되지 않았습니다! 로딩 화면을 생성하지 않습니다.");
                return;
            }

            Debug.Log("로딩 화면 프리팹 사용");
            loadingScreen = Instantiate(customLoadingScreenPrefab);
            Canvas canvas = loadingScreen.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 1000;
                DontDestroyOnLoad(canvas.gameObject);
            }

            // 컴포넌트들 찾기
            Debug.Log("=== 컴포넌트 검색 시작 ===");

            // 1. Slider 컴포넌트 찾기
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
            Debug.Log($"CanvasGroup: {(loadingCanvasGroup != null ? "찾음" : "못 찾음")}");

            // 초기 상태 설정 - 비활성화 상태로 생성
            loadingScreen.SetActive(false);
            loadingCanvasGroup.alpha = 0f;

            Debug.Log("로딩 화면 프리팹 생성 완료");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeScene(scene.name);
        }

        private void OnSceneUnloaded(Scene scene) { }

        private void InitializeScene(string sceneName)
        {
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

        // 새로 추가할 코루틴
        private IEnumerator LoadSceneAfterActivation(string sceneName, int mainStageID, int subStageID, int score, bool isWin)
        {
            // 한 프레임 대기하여 오브젝트가 완전히 활성화되도록 함
            yield return null;

            HandleSceneSpecificData(sceneName, mainStageID, subStageID, score, isWin);
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        #endregion
    }
}




