using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;
using System.Collections;
using UnityEngine.UI;
using KYG_skyPower;

namespace YSK
{
    public class StageManager : MonoBehaviour
    {
        [Header("StageData")]
        [SerializeField] private List<StageData> stageDataList; // 모든 스테이지의 데이터
        [SerializeField] private int maxMainStages = 4; // 메인 스테이지 최대값
        [SerializeField] private int maxSubStages = 5; // 서브 스테이지 최대값
        private StageData currentStage;
        private List<GameObject> MapPrefabs;

        [Header("MoveInfo")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;

        [Header("Transition Settings")]
        [SerializeField] private bool enableTransition = false;
        [SerializeField] private bool useFadeTransition = false;
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Color fadeColor = Color.black;

        [Header("References")]
        //[SerializeField] private GameStateManager gameStateManager;

        private List<GameObject> spawnedMaps = new(); // 프리팹을 이용한 Stage Map 생성
        private List<GameObject> movingMaps = new(); // 현재 이동중인 맵.
        private bool isTransitioning = false;

        [Header("Data Management")]
        [SerializeField] private StageDataManager dataManager;

        #region Unity Lifecycle

        private void Awake()
        {
            // Awake에서는 최소한의 초기화만
        }

        private void Start()
        {
            Debug.Log("=== StageManager Start 시작 ===");
            
            InitializeComponents();
            
            // GameStateManager가 준비될 때까지 대기
            //StartCoroutine(WaitForGameStateManager());
            
            Debug.Log("=== StageManager Start 완료 ===");
        }

        private void Update()
        {
            UpdateMovingMaps();
            CheckInput();
        }

        #endregion


        /// <summary>
        /// 필요한 컴포넌트들을 초기화합니다.
        /// </summary>
        private void InitializeComponents()
        {
            FindTransformPoints();
            InitializeFadePanel();
            
            // DataManager 찾기
            if (dataManager == null)
            {
                dataManager = FindObjectOfType<StageDataManager>();
                if (dataManager == null)
                {
                    Debug.LogWarning("StageDataManager를 찾을 수 없습니다!");
                }
            }
        }

        /// <summary>
        /// 자식 오브젝트에서 StartPoint와 EndPoint를 찾습니다.
        /// </summary>
        private void FindTransformPoints()
        {
            if (startPoint == null)
            {
                startPoint = transform.Find("StartPoint");
                if (startPoint == null)
                {
                    Debug.LogWarning("StartPoint를 찾을 수 없습니다! (씬에서 설정해야 합니다)");
                }
            }

            if (endPoint == null)
            {
                endPoint = transform.Find("EndPoint");
                if (endPoint == null)
                {
                    Debug.LogWarning("EndPoint를 찾을 수 없습니다! (씬에서 설정해야 합니다)");
                }
            }
        }

        /// <summary>
        /// 페이드 패널을 초기화합니다.
        /// </summary>
        private void InitializeFadePanel()
        {
            // GameSceneManager의 로딩 화면이 활성화되어 있으면 비활성화
            if (GameSceneManager.Instance != null && GameSceneManager.Instance.IsLoadingScreenEnabled)
            {
                GameSceneManager.Instance.SetLoadingScreenEnabled(false);
                Debug.Log("GameSceneManager 로딩 화면 비활성화");
            }
            
            // 전환 기능이 비활성화되어 있으면 페이드 패널을 생성하지 않음
            if (!enableTransition || !useFadeTransition)
            {
                Debug.Log("전환 기능이 비활성화되어 페이드 패널을 생성하지 않습니다.");
                return;
            }
            
            if (fadePanel == null)
            {
                CreateFadePanel();
            }
            
            // 초기 상태 설정 - 항상 투명하게 시작
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
            
            Debug.Log("페이드 패널 초기화 완료 - 투명 상태");
        }
        
        private void CreateFadePanel()
        {
            // Canvas 찾기 또는 생성
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("TransitionCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999; // GameSceneManager보다 낮게
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }
            
            // Fade Panel 생성
            GameObject fadeObj = new GameObject("StageFadePanel");
            fadeObj.transform.SetParent(canvas.transform, false);
            
            Image fadeImage = fadeObj.AddComponent<Image>();
            fadeImage.color = fadeColor;
            
            RectTransform rectTransform = fadeObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            fadePanel = fadeObj.AddComponent<CanvasGroup>();
        }

        private void CheckInput()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) HandleKey(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleKey(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleKey(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4)) HandleKey(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5)) HandleKey(5);
                else if (Input.GetKeyDown(KeyCode.Alpha6)) HandleKey(6);
                else if (Input.GetKeyDown(KeyCode.Alpha7)) HandleKey(7);
            }
        }

        private void HandleKey(int keyNumber)
        {
            switch (keyNumber)
            {
                case 1:
                    Debug.Log("1번 키: 스테이지 클리어 및 다음 스테이지로 (페이드 트랜지션 사용)");
                    ClearCurrentStageAndNextWithTransition();
                    break;
                case 2:
                    Debug.Log("2번 키: 스테이지 클리어 및 다음 스테이지로 (페이드 트랜지션 없이)");
                    ClearCurrentStageAndNext();
                    break;
                case 3:
                    Debug.Log("3번 키: 강제 맵 생성 테스트");
                    ForceTestMapGeneration();
                    break;
                case 4:
                    Debug.Log("4번 키: 스테이지 진행 상태 초기화");
                    ResetStageProgress();
                    break;
                case 5:
                    Debug.Log("5번 키: 2-3으로 강제 이동 (페이드 트랜지션 사용)");
                    ForceStageWithTransition(1, 1);
                    break;
                case 6:
                    Debug.Log("6번 키: 현재 상태 정보 출력");
                    DebugCurrentState();
                    break;
                case 7:
                    Debug.Log("7번 키: 페이드 효과 테스트");
                    if (fadePanel != null)
                    {
                        StartCoroutine(FadeOut());
                        StartCoroutine(FadeIn());
                    }
                    else
                    {
                        Debug.LogError("fadePanel이 null입니다!");
                    }
                    break;
                default:
                    Debug.LogWarning("알 수 없는 키 입력");
                    break;
            }
        }

        private void LoadStage(int mainStageID, int subStageID = 1)
        {
            Debug.Log($"=== LoadStage 시작: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID} ===");

            // 기존 맵 정리
            ClearAllMaps();

            // PlayerPrefs 업데이트
            PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
            PlayerPrefs.SetInt("SelectedSubStage", subStageID);
            PlayerPrefs.Save();

            Debug.Log($"스테이지 정보 - 메인: {mainStageID}, 서브: {subStageID}");

            // 스테이지 데이터 리스트가 비어있는지 확인
            if (stageDataList == null || stageDataList.Count == 0)
            {
                Debug.LogError("스테이지 데이터 리스트가 비어있습니다!");
                Debug.LogError("StageManager의 Inspector에서 StageDataList에 StageData 에셋들을 추가해주세요!");
                return;
            }

            Debug.Log($"사용 가능한 스테이지: {string.Join(", ", stageDataList.Select(s => s.stageID))}");

            // 메인 스테이지 ID로 해당 스테이지 데이터 찾기
            currentStage = stageDataList.Find(data => data.stageID == mainStageID);

            if (currentStage == null)
            {
                Debug.LogError($"Main Stage ID {mainStageID} not found!");
                Debug.LogError($"사용 가능한 스테이지: {string.Join(", ", stageDataList.Select(s => s.stageID))}");
                return;
            }

            Debug.Log($"메인 스테이지 {mainStageID} 데이터 로드 완료, 서브 스테이지 {subStageID} 맵 생성 시작");
            SpawnMaps();
            
            Debug.Log($"=== LoadStage 완료: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID} ===");
        }

        private void SpawnMaps()
        {
            Debug.Log($"SpawnMaps 시작: 기존 맵 {spawnedMaps.Count}개 정리");
            spawnedMaps.Clear();
            movingMaps.Clear();

            // currentStage가 null인지 확인
            if (currentStage == null)
            {
                Debug.LogError("currentStage가 null입니다! LoadStage를 먼저 호출해주세요.");
                return;
            }

            // PlayerPrefs에서 현재 서브 스테이지 정보 가져오기
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);

            // 사용할 맵 프리팹 리스트 결정
            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            // 서브 스테이지별 커스텀 맵이 있는지 확인
            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"서브 스테이지 {subStageID} 커스텀 맵 사용");
                }
                else
                {
                    Debug.Log($"서브 스테이지 {subStageID} 기본 맵 사용");
                }
            }

            // 맵 프리팹 리스트가 비어있는지 확인
            if (mapPrefabsToUse == null || mapPrefabsToUse.Count == 0)
            {
                Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹이 비어있습니다!");
                return;
            }

            Debug.Log($"새 맵 생성 시작: {mapPrefabsToUse.Count}개 프리팹 (서브 스테이지 {subStageID})");
            for (int i = 0; i < mapPrefabsToUse.Count; i++)
            {
                // 프리팹이 null인지 확인
                if (mapPrefabsToUse[i] == null)
                {
                    Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹 {i}번이 null입니다!");
                    continue;
                }

                GameObject map = Instantiate(mapPrefabsToUse[i]);

                // startPoint가 null인지 확인
                if (startPoint == null)
                {
                    Debug.LogWarning("startPoint가 null입니다! 맵을 기본 위치에 생성합니다.");
                    map.transform.position = Vector3.back * (mapLength * i);
                }
                else
                {
                    // 맵을 일렬로 배치 (예: Z축 기준)
                    map.transform.position = startPoint.position + Vector3.back * (mapLength * i);
                }

                spawnedMaps.Add(map);
                movingMaps.Add(map);
                Debug.Log($"맵 {i + 1} 생성 완료: {map.name}");
            }

            Debug.Log($"SpawnMaps 완료: 총 {spawnedMaps.Count}개 맵 생성 (서브 스테이지 {subStageID})");
        }

        private void MoveMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            // endPoint가 null인지 확인
            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다! 맵 이동을 중단합니다.");
                return;
            }

            // endPoint를 지나면 startPoint로 재배치
            if (map.transform.position.z < endPoint.position.z)
            {
                float maxZ = GetMaxZPosition();
                map.transform.position = new Vector3(map.transform.position.x, map.transform.position.y, maxZ + mapLength);
            }
        }

        private float GetMaxZPosition()
        {
            float maxZ = float.MinValue;
            foreach (var map in movingMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            return maxZ;
        }

        private void UpdateMovingMaps()
        {
            // movingMaps가 null이거나 비어있는지 확인
            if (movingMaps == null || movingMaps.Count == 0)
            {
                return;
            }

            foreach (var map in movingMaps)
            {
                if (map != null)
                {
                    MoveMap(map);
                }
            }
        }

        public void ChangeStage(int newStageID)
        {
            Debug.Log($"ChangeStage 호출: 스테이지 {newStageID}로 변경");

            // LoadStage에서 이미 ClearAllMaps를 호출하므로 여기서는 생략
            // 단순히 메인 스테이지만 변경하고 서브 스테이지는 1로 초기화
            LoadStage(newStageID);

            // PlayerPrefs에서 현재 설정된 메인 스테이지와 서브 스테이지 정보 가져오기
            int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);

        }

        /// <summary>
        /// 특정 스테이지의 ID로 StageData를 반환합니다.
        /// </summary>
        /// <param name="stageID">찾을 스테이지의 ID</param>
        /// <returns>해당 스테이지의 데이터 또는 null</returns>
        public StageData GetStageData(int stageID)
        {
            return stageDataList.Find(data => data.stageID == stageID);
        }

        /// <summary>
        /// 모든 맵을 정리합니다.
        /// </summary>
        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: {spawnedMaps.Count}개 맵 정리");

            foreach (var map in spawnedMaps)
            {
                if (map != null)
                {
                    Debug.Log($"맵 제거: {map.name}");
                    Destroy(map);
                }
            }
            spawnedMaps.Clear();
            movingMaps.Clear();

            Debug.Log("ClearAllMaps 완료: 모든 맵 제거됨");
        }

        /// <summary>
        /// 현재 설정된 서브 스테이지의 ID를 반환합니다.
        /// </summary>
        /// <returns>현재 서브 스테이지 ID</returns>
        public int GetCurrentSubStageID()
        {
            return PlayerPrefs.GetInt("SelectedSubStage", 1);
        }

        /// <summary>
        /// 현재 설정된 서브 스테이지의 데이터를 반환합니다.
        /// </summary>
        /// <returns>서브 스테이지 데이터 또는 null</returns>
        public SubStageData GetCurrentSubStageData()
        {
            if (currentStage == null) return null;

            int subStageID = GetCurrentSubStageID();
            return currentStage.subStages?.Find(s => s.subStageID == subStageID);
        }

        #region Transition Methods

        /// <summary>
        /// 부드러운 스테이지 전환을 시작합니다.
        /// </summary>
        /// <param name="mainStageID">전환할 메인 스테이지 ID</param>
        /// <param name="subStageID">전환할 서브 스테이지 ID</param>
        /// <param name="isGameStart">게임 시작 여부 (true: 게임 시작, false: 스테이지 전환)</param>
        public void StartStageTransition(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransition 호출: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID}, 게임시작: {isGameStart}");
            
            // 전환 기능이 비활성화되어 있으면 즉시 전환
            if (!enableTransition)
            {
                Debug.Log("전환 기능이 비활성화되어 즉시 전환합니다.");
                LoadStage(mainStageID, subStageID);
                return;
            }
            
            if (!isTransitioning)
            {
                Debug.Log("스테이지 전환 코루틴 시작");
                StartCoroutine(TransitionCoroutine(mainStageID, subStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }

        /// <summary>
        ///  검은화면에서 페이드 인을 이용한 스테이지 전환
        /// </summary>
        public void StartStageTransitionOnlyFadeIn(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransition 호출: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID}, 게임시작: {isGameStart}");

            // 전환 기능이 비활성화되어 있으면 즉시 전환
            if (!enableTransition)
            {
                Debug.Log("전환 기능이 비활성화되어 즉시 전환합니다.");
                LoadStage(mainStageID, subStageID);
                return;
            }

            if (!isTransitioning)
            {
                Debug.Log("스테이지 전환 코루틴 시작");
                StartCoroutine(TransitionWithFadeInCoroutine(mainStageID, subStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }

        /// <summary>
        /// 페이드 인/아웃을 이용한 스테이지 전환
        /// </summary>
        private IEnumerator TransitionCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionCoroutine 시작: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID}");
            isTransitioning = true;
            
            if (isGameStart)
            {
                Debug.Log("게임 시작: 페이드 효과 없이 즉시 전환");
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (useFadeTransition)
                {
                    Debug.Log("스테이지 전환: 페이드 효과 사용");
                    // 페이드 아웃
                    yield return StartCoroutine(FadeOut());
                    
                    // 스테이지 전환
                    LoadStage(mainStageID, subStageID);
                    
                    // 페이드 인
                    yield return StartCoroutine(FadeIn());
                }
                else
                {
                    Debug.Log("스테이지 전환: 페이드 효과 없이 즉시 전환");
                    LoadStage(mainStageID, subStageID);
                }
            }
            
            isTransitioning = false;
            Debug.Log("TransitionCoroutine 완료");
        }


        /// <summary>
        /// 페이드 인만 사용하는 전환 코루틴
        /// </summary>
        private IEnumerator TransitionWithFadeInCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionWithFadeInCoroutine 시작: 메인 스테이지 {mainStageID}, 서브 스테이지 {subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                Debug.Log("게임 시작: 바로 스테이지 로드");
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (useFadeTransition)
                {
                    Debug.Log("스테이지 전환: 페이드 인만 사용");

                    // 페이드 패널을 검은색으로 설정 (페이드 아웃 효과 대신)
                    if (fadePanel != null)
                    {
                        fadePanel.alpha = 1f;
                        fadePanel.gameObject.SetActive(true);
                    }

                    // 먼저 스테이지 로드 (검은 화면 상태)
                    LoadStage(mainStageID, subStageID);

                    // 잠시 대기하여 스테이지가 완전히 로드되도록 함
                    yield return new WaitForSeconds(0.1f);

                    // 페이드 인만 실행
                    yield return StartCoroutine(FadeIn());
                }
                else
                {
                    Debug.Log("스테이지 전환: 페이드 효과 없이 바로 전환");
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionWithFadeInCoroutine 완료");
        }


        private IEnumerator FadeOut()
        {
            if (fadePanel == null)
            {
                Debug.LogWarning("페이드 패널이 null입니다. 즉시 전환합니다.");
                yield break;
            }
            
            fadePanel.gameObject.SetActive(true);
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                fadePanel.alpha = fadeCurve.Evaluate(t);
                yield return null;
            }
            
            fadePanel.alpha = 1f;
        }
        
        private IEnumerator FadeIn()
        {
            if (fadePanel == null)
            {
                Debug.LogWarning("페이드 패널이 null입니다.");
                yield break;
            }
            
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                fadePanel.alpha = 1f - fadeCurve.Evaluate(t);
                yield return null;
            }
            
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 현재 전환 중인지 확인
        /// </summary>
        public bool IsTransitioning => isTransitioning;
        
        /// <summary>
        /// 전환 효과 설정
        /// </summary>
        public void SetFadeDuration(float duration)
        {
            fadeDuration = duration;
        }
        
        /// <summary>
        /// 페이드 패널 색상 설정
        /// </summary>
        public void SetFadeColor(Color color)
        {
            fadeColor = color;
            if (fadePanel != null)
            {
                Image fadeImage = fadePanel.GetComponent<Image>();
                if (fadeImage != null)
                {
                    fadeImage.color = color;
                }
            }
        }
        
        /// <summary>
        /// 스테이지 버튼 클릭 이벤트 (UI에서 호출)
        /// </summary>
        /// <param name="mainStageID">선택된 메인 스테이지 ID</param>
        /// <param name="subStageID">선택된 서브 스테이지 ID</param>
        public void OnStageButtonClick(int mainStageID, int subStageID = 1)
        {
            if (!isTransitioning)
            {
                StartStageTransition(mainStageID, subStageID, false);
            }
        }

        #endregion

        /// <summary>
        /// 현재 스테이지를 클리어하고 다음 스테이지로 전환합니다.
        /// </summary>
        public void ClearCurrentStageAndNext()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            Debug.Log($"스테이지 클리어: {currentMainStage}-{currentSubStage}");
            
            // 다음 스테이지 계산
            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);
            
            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }
            
            // 다음 스테이지로 전환
            LoadStage(nextStage.mainStage, nextStage.subStage);
            Debug.Log($"다음 스테이지로 전환: {nextStage.mainStage}-{nextStage.subStage}");
        }

        /// <summary>
        /// 다음 스테이지 정보를 계산합니다.
        /// </summary>
        /// <param name="currentMainStage">현재 메인 스테이지</param>
        /// <param name="currentSubStage">현재 서브 스테이지</param>
        /// <returns>다음 스테이지 정보</returns>
        private (int mainStage, int subStage, bool isGameComplete) CalculateNextStage(int currentMainStage, int currentSubStage)
        {
            // 다음 서브 스테이지 계산
            int nextSubStage = currentSubStage + 1;
            
            // 서브 스테이지가 최대값을 넘으면 다음 메인 스테이지로
            if (nextSubStage > maxSubStages)
            {
                int nextMainStage = currentMainStage + 1;
                
                // 메인 스테이지가 최대값을 넘으면 게임 클리어
                if (nextMainStage > maxMainStages)
                {
                    return (0, 0, true); // 게임 클리어
                }
                
                // 다음 메인 스테이지의 첫 번째 서브 스테이지로
                return (nextMainStage, 1, false);
            }
            else
            {
                // 현재 메인 스테이지의 다음 서브 스테이지로
                return (currentMainStage, nextSubStage, false);
            }
        }

        /// <summary>
        /// 게임 클리어 시 호출됩니다.
        /// </summary>
        private void OnGameComplete()
        {
            Debug.Log("게임 클리어!");
            
            // GameStateManager에 게임 클리어 알림
            //if (gameStateManager != null)
            //{
            //    gameStateManager.SetGameState(GameState.StageComplete);
            //}
            
            // 결과 화면으로 이동
            if (GameSceneManager.Instance != null)
            {
                //GameSceneManager.Instance.LoadResultScene(1000, true); // 게임 클리어
            }
        }

        /// <summary>
        /// 특정 스테이지로 강제 이동합니다 (테스트용).
        /// </summary>
        public void ForceStage(int mainStageID, int subStageID)
        {
            Debug.Log($"강제 스테이지 이동: {mainStageID}-{subStageID}");
            LoadStage(mainStageID, subStageID);
        }

        /// <summary>
        /// 스테이지 진행 상태를 초기화합니다 (테스트용).
        /// </summary>
        public void ResetStageProgress()
        {
            PlayerPrefs.SetInt("SelectedMainStage", 1);
            PlayerPrefs.SetInt("SelectedSubStage", 1);
            PlayerPrefs.Save();

            // 현재 스테이지도 초기화
            LoadStage(1, 1);

            Debug.Log("스테이지 진행 상태 초기화: 1-1");
        }

        /// <summary>
        /// 강제로 맵 생성을 테스트합니다.
        /// </summary>
        public void ForceTestMapGeneration()
        {
            Debug.Log("=== 강제 맵 생성 테스트 시작 ===");
            
            // 1. StageData 할당 확인
            Debug.Log($"StageDataList 개수: {stageDataList?.Count ?? 0}");
            if (stageDataList != null)
            {
                for (int i = 0; i < stageDataList.Count; i++)
                {
                    Debug.Log($"StageData[{i}]: ID={stageDataList[i]?.stageID}, Name={stageDataList[i]?.stageName}");
                }
            }
            
            // 2. PlayerPrefs 확인
            int mainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            Debug.Log($"PlayerPrefs - Main: {mainStage}, Sub: {subStage}");
            
            // 3. 강제로 맵 생성
            LoadStage(mainStage, subStage);
            
            Debug.Log("=== 강제 맵 생성 테스트 완료 ===");
        }

        /// <summary>
        /// 현재 상태 정보를 출력합니다.
        /// </summary>
        private void DebugCurrentState()
        {
            Debug.Log("=== 현재 상태 정보 ===");
            Debug.Log($"StageDataList: {(stageDataList != null ? stageDataList.Count : 0)}개");
            Debug.Log($"CurrentStage: {(currentStage != null ? currentStage.stageName : "null")}");
            Debug.Log($"SpawnedMaps: {spawnedMaps.Count}개");
            Debug.Log($"MovingMaps: {movingMaps.Count}개");
            Debug.Log($"MaxMainStages: {maxMainStages}, MaxSubStages: {maxSubStages}");
            //Debug.Log($"GameStateManager: {(GameStateManager.Instance != null ? "존재" : "null")}");
            Debug.Log($"PlayerPrefs - Main: {PlayerPrefs.GetInt("SelectedMainStage", 1)}, Sub: {PlayerPrefs.GetInt("SelectedSubStage", 1)}");
            Debug.Log("=== 현재 상태 정보 완료 ===");
        }

        // 페이드 트랜지션을 사용하는 테스트용 메서드들
        public void ClearCurrentStageAndNextWithTransition()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            Debug.Log($"스테이지 클리어 (페이드 트랜지션 사용): {currentMainStage}-{currentSubStage}");
            
            // 다음 스테이지 계산
            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);
            
            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }
            
            // 페이드 트랜지션으로 다음 스테이지로 전환
            StartStageTransition(nextStage.mainStage, nextStage.subStage, false);
            Debug.Log($"다음 스테이지로 전환 (페이드): {nextStage.mainStage}-{nextStage.subStage}");
        }

        public void ForceStageWithTransition(int mainStageID, int subStageID)
        {
            Debug.Log($"강제 스테이지 이동 (페이드 트랜지션 사용): {mainStageID}-{subStageID}");
            
            // 페이드 트랜지션으로 스테이지 전환 (PlayerPrefs는 LoadStage에서 업데이트됨)
            StartStageTransition(mainStageID, subStageID, false);
        }

        /// <summary>
        /// 스테이지 클리어 시 호출
        /// </summary>
        public void OnStageCompleted(int score = 0)
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            
            Debug.Log($"스테이지 완료: {currentMainStage}-{currentSubStage}, 점수: {score}");
            
            if (dataManager != null)
            {
                // 점수 업데이트
                dataManager.UpdateStageScore(currentMainStage, currentSubStage, score);
                
                // 완료 처리
                dataManager.CompleteStage(currentMainStage, currentSubStage, Time.time);
                
                // 다음 스테이지 해금
                UnlockNextStage(currentMainStage, currentSubStage);
            }
            
            // GameManager 이벤트 발생
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameClear();
            }
        }

        /// <summary>
        /// 다음 스테이지 해금
        /// </summary>
        private void UnlockNextStage(int currentMainStage, int currentSubStage)
        {
            if (dataManager == null) return;
            
            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);
            
            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어!");
                return;
            }
            
            // 다음 스테이지 해금
            dataManager.UnlockStage(nextStage.mainStage);
            dataManager.UnlockSubStage(nextStage.mainStage, nextStage.subStage);
            
            Debug.Log($"다음 스테이지 해금: {nextStage.mainStage}-{nextStage.subStage}");
        }

        /// <summary>
        /// 스테이지 로드 전 해금 상태 확인
        /// </summary>
        private bool CanLoadStage(int mainStageID, int subStageID)
        {
            if (dataManager == null) return true;
            
            return dataManager.IsStageUnlocked(mainStageID) && 
                   dataManager.IsSubStageUnlocked(mainStageID, subStageID);
        }
    }
}