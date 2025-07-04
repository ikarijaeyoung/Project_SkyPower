using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;
using System.Collections;
using UnityEngine.UI;
using KYG_skyPower;

namespace YSK
{
    public class RnDStageManager : MonoBehaviour
    {
        [Header("Stage Data")]
        [SerializeField] private List<StageData> stageDataList;
        [SerializeField] private int maxMainStages = 4;
        [SerializeField] private int maxSubStages = 5;
        [SerializeField] private StageDataManager dataManager;

        private StageData currentStage;

        [Header("Map System")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;

        // 맵 연결 시스템
        [Header("Map Connection System")]
        [SerializeField] private bool enableMapConnection = true;
        [SerializeField] private float mapConnectionDistance = 0.1f;
        [SerializeField] private bool enableMapTransition = true;
        [SerializeField] private float mapTransitionDuration = 1f;
        [SerializeField] private AnimationCurve mapTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private List<GameObject> spawnedMaps = new();
        private List<GameObject> movingMaps = new();
        private List<GameObject> mapBuffer = new(); // 맵 버퍼 (제거 대기)
        private bool isMapTransitioning = false;

        [Header("Transition Settings")]
        [SerializeField] private bool useGameSceneManagerTransition = true;
        [SerializeField] private bool enableTransition = true;

        private bool isTransitioning = false;

        // 이벤트
        public static System.Action<int, int> OnStageChanged;
        public static System.Action OnMapTransitionStarted;
        public static System.Action OnMapTransitionCompleted;

        #region Unity Lifecycle

        private void Awake()
        {
            // 최소한의 초기화만
        }

        private void Start()
        {
            Debug.Log("=== StageManager Start 시작 ===");
            InitializeComponents();
            Debug.Log("=== StageManager Start 완료 ===");
        }

        private void Update()
        {
            if (!isMapTransitioning)
            {
                UpdateMovingMaps();
                if (enableMapConnection)
                {
                    UpdateMapConnections();
                }
            }
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            FindTransformPoints();
            FindDataManager();
        }

        private void FindTransformPoints()
        {
            if (startPoint == null)
            {
                startPoint = transform.Find("StartPoint");
                if (startPoint == null)
                {
                    Debug.LogWarning("StartPoint를 찾을 수 없습니다!");
                }
            }

            if (endPoint == null)
            {
                endPoint = transform.Find("EndPoint");
                if (endPoint == null)
                {
                    Debug.LogWarning("EndPoint를 찾을 수 없습니다!");
                }
            }
        }

        private void FindDataManager()
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

        #endregion

        #region Map Connection System

        private void UpdateMapConnections()
        {
            if (movingMaps.Count < 2) return;

            // 맵들을 Z 위치 순으로 정렬
            var sortedMaps = movingMaps.OrderBy(m => m.transform.position.z).ToList();

            for (int i = 0; i < sortedMaps.Count - 1; i++)
            {
                GameObject currentMap = sortedMaps[i];
                GameObject nextMap = sortedMaps[i + 1];

                if (currentMap == null || nextMap == null) continue;

                float distance = Mathf.Abs(nextMap.transform.position.z - currentMap.transform.position.z);
                
                // 맵 간격이 너무 크면 조정
                if (distance > mapLength + mapConnectionDistance)
                {
                    Vector3 newPosition = currentMap.transform.position;
                    newPosition.z += mapLength;
                    nextMap.transform.position = newPosition;
                    
                    Debug.Log($"맵 연결 조정: {distance:F2} -> {mapLength:F2}");
                }
            }
        }

        #endregion

        #region Map Management

        private void LoadStage(int mainStageID, int subStageID = 1)
        {
            Debug.Log($"=== LoadStage 시작: {mainStageID}-{subStageID} ===");

            if (!enableMapTransition)
            {
                // 기존 방식: 즉시 맵 교체
                ClearAllMaps();
                UpdatePlayerPrefs(mainStageID, subStageID);

                if (!ValidateStageData(mainStageID))
                    return;

                currentStage = stageDataList.Find(data => data.stageID == mainStageID);
                SpawnMaps();
            }
            else
            {
                // 개선된 방식: 맵 연결 전환
                StartCoroutine(ConnectMapTransition(mainStageID, subStageID));
            }

            Debug.Log($"=== LoadStage 완료: {mainStageID}-{subStageID} ===");
        }

        private IEnumerator ConnectMapTransition(int mainStageID, int subStageID)
        {
            Debug.Log($"맵 연결 전환 시작: {mainStageID}-{subStageID}");
            isMapTransitioning = true;
            OnMapTransitionStarted?.Invoke();

            // 1. 새 스테이지 데이터 준비
            UpdatePlayerPrefs(mainStageID, subStageID);

            if (!ValidateStageData(mainStageID))
            {
                isMapTransitioning = false;
                yield break;
            }

            StageData newStage = stageDataList.Find(data => data.stageID == mainStageID);
            List<GameObject> newMapPrefabs = GetMapPrefabsForSubStage(subStageID);

            if (newMapPrefabs == null || newMapPrefabs.Count == 0)
            {
                Debug.LogWarning($"새 스테이지 {mainStageID}의 맵 프리팹이 없습니다!");
                isMapTransitioning = false;
                yield break;
            }

            // 2. 현재 맵들을 버퍼로 이동 (제거 대기)
            MoveCurrentMapsToBuffer();

            // 3. 새 맵들을 현재 맵 뒤에 연결
            yield return StartCoroutine(SpawnAndConnectNewMaps(newMapPrefabs));

            // 4. 스테이지 데이터 업데이트
            currentStage = newStage;

            // 5. 이벤트 발생
            OnStageChanged?.Invoke(mainStageID, subStageID);

            isMapTransitioning = false;
            OnMapTransitionCompleted?.Invoke();
            Debug.Log("맵 연결 전환 완료");
        }

        private void MoveCurrentMapsToBuffer()
        {
            Debug.Log($"현재 맵들을 버퍼로 이동: {spawnedMaps.Count}개");
            
            foreach (var map in spawnedMaps)
            {
                if (map != null)
                {
                    mapBuffer.Add(map);
                }
            }
            
            spawnedMaps.Clear();
            movingMaps.Clear();
        }

        private IEnumerator SpawnAndConnectNewMaps(List<GameObject> mapPrefabs)
        {
            Debug.Log($"새 맵들 연결 생성: {mapPrefabs.Count}개");

            float maxZ = GetMaxZPositionFromBuffer();
            if (maxZ == float.MinValue)
            {
                maxZ = startPoint != null ? startPoint.position.z : 0f;
            }

            for (int i = 0; i < mapPrefabs.Count; i++)
            {
                if (mapPrefabs[i] == null) continue;

                GameObject map = Instantiate(mapPrefabs[i]);
                Vector3 spawnPosition = new Vector3(
                    startPoint != null ? startPoint.position.x : 0f,
                    startPoint != null ? startPoint.position.y : 0f,
                    maxZ + (mapLength * i)
                );

                map.transform.position = spawnPosition;
                spawnedMaps.Add(map);
                movingMaps.Add(map);

                // 부드러운 등장 효과
                if (enableMapTransition)
                {
                    yield return StartCoroutine(FadeInMap(map));
                }
            }

            Debug.Log($"새 맵들 연결 완료: {spawnedMaps.Count}개");
        }

        private IEnumerator FadeInMap(GameObject map)
        {
            CanvasGroup cg = map.GetComponent<CanvasGroup>();
            if (cg == null) cg = map.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < mapTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / mapTransitionDuration;
                cg.alpha = mapTransitionCurve.Evaluate(t);
                yield return null;
            }

            cg.alpha = 1f;
        }

        private float GetMaxZPositionFromBuffer()
        {
            float maxZ = float.MinValue;
            foreach (var map in mapBuffer)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            return maxZ;
        }

        private bool ValidateStageData(int mainStageID)
        {
            if (stageDataList == null || stageDataList.Count == 0)
            {
                Debug.LogError("스테이지 데이터 리스트가 비어있습니다!");
                return false;
            }

            if (!stageDataList.Exists(data => data.stageID == mainStageID))
            {
                Debug.LogError($"Main Stage ID {mainStageID} not found!");
                return false;
            }

            return true;
        }

        private void UpdatePlayerPrefs(int mainStageID, int subStageID)
        {
            PlayerPrefs.SetInt("SelectedMainStage", mainStageID);
            PlayerPrefs.SetInt("SelectedSubStage", subStageID);
            PlayerPrefs.Save();
        }

        private void SpawnMaps()
        {
            spawnedMaps.Clear();
            movingMaps.Clear();

            if (currentStage == null)
            {
                Debug.LogError("currentStage가 null입니다!");
                return;
            }

            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);
            List<GameObject> mapPrefabsToUse = GetMapPrefabsForSubStage(subStageID);

            if (mapPrefabsToUse == null || mapPrefabsToUse.Count == 0)
            {
                Debug.LogWarning($"스테이지 {currentStage.stageID}의 맵 프리팹이 비어있습니다!");
                return;
            }

            Debug.Log($"새 맵 생성 시작: {mapPrefabsToUse.Count}개 프리팹 (서브 스테이지 {subStageID})");

            for (int i = 0; i < mapPrefabsToUse.Count; i++)
            {
                if (mapPrefabsToUse[i] == null)
                {
                    Debug.LogWarning($"맵 프리팹 {i}번이 null입니다!");
                    continue;
                }

                GameObject map = Instantiate(mapPrefabsToUse[i]);
                Vector3 spawnPosition = startPoint != null
                    ? startPoint.position + Vector3.forward * (mapLength * i)
                    : Vector3.forward * (mapLength * i);

                map.transform.position = spawnPosition;
                spawnedMaps.Add(map);
                movingMaps.Add(map);
            }

            Debug.Log($"SpawnMaps 완료: 총 {spawnedMaps.Count}개 맵 생성");
        }

        private List<GameObject> GetMapPrefabsForSubStage(int subStageID)
        {
            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"서브 스테이지 {subStageID} 커스텀 맵 사용");
                }
            }

            return mapPrefabsToUse;
        }

        private void UpdateMovingMaps()
        {
            if (movingMaps == null || movingMaps.Count == 0)
                return;

            foreach (var map in movingMaps.ToList()) // ToList()로 복사본 사용
            {
                if (map != null)
                {
                    MoveMap(map);
                }
                else
                {
                    // null 맵 제거
                    movingMaps.Remove(map);
                }
            }

            // 버퍼의 맵들도 이동
            foreach (var map in mapBuffer.ToList())
            {
                if (map != null)
                {
                    MoveMap(map);
                }
                else
                {
                    mapBuffer.Remove(map);
                }
            }
        }

        private void MoveMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint가 null입니다!");
                return;
            }

            // 맵이 끝점을 지나면 제거
            if (map.transform.position.z < endPoint.position.z)
            {
                if (spawnedMaps.Contains(map))
                {
                    spawnedMaps.Remove(map);
                    movingMaps.Remove(map);
                }
                else if (mapBuffer.Contains(map))
                {
                    mapBuffer.Remove(map);
                }
                
                Destroy(map);
                Debug.Log("맵이 EndPoint를 지나 제거됨");
            }
        }

        private float GetMaxZPosition()
        {
            float maxZ = float.MinValue;
            
            // 현재 맵들에서 최대 Z 찾기
            foreach (var map in movingMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            
            // 버퍼 맵들에서도 최대 Z 찾기
            foreach (var map in mapBuffer)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            
            return maxZ;
        }

        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: {spawnedMaps.Count}개 맵 정리");

            foreach (var map in spawnedMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            spawnedMaps.Clear();
            movingMaps.Clear();

            // 버퍼 맵들도 정리
            foreach (var map in mapBuffer)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            mapBuffer.Clear();

            Debug.Log("ClearAllMaps 완료");
        }

        #endregion

        #region Stage Transition

        public void StartStageTransition(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransition: {mainStageID}-{subStageID}, 게임시작: {isGameStart}");

            if (!enableTransition)
            {
                LoadStage(mainStageID, subStageID);
                return;
            }

            if (!isTransitioning)
            {
                StartCoroutine(TransitionCoroutine(mainStageID, subStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }

        public void StartStageTransitionOnlyFadeIn(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransitionOnlyFadeIn: {mainStageID}-{subStageID}, 게임시작: {isGameStart}");

            if (!enableTransition)
            {
                LoadStage(mainStageID, subStageID);
                return;
            }

            if (!isTransitioning)
            {
                StartCoroutine(TransitionWithFadeInCoroutine(mainStageID, subStageID, isGameStart));
            }
            else
            {
                Debug.LogWarning("이미 전환 중입니다!");
            }
        }

        private IEnumerator TransitionCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionCoroutine 시작: {mainStageID}-{subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (enableTransition && useGameSceneManagerTransition && Manager.GSM != null)
                {
                    // GameSceneManager의 전환 화면 사용
                    Manager.GSM.ShowTransitionScreen();
                    yield return new WaitForSeconds(0.5f); // 전환 화면 표시 시간

                    LoadStage(mainStageID, subStageID);

                    yield return new WaitForSeconds(0.1f); // 스테이지 로드 대기
                    Manager.GSM.HideTransitionScreen();
                }
                else
                {
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionCoroutine 완료");
        }

        private IEnumerator TransitionWithFadeInCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionWithFadeInCoroutine 시작: {mainStageID}-{subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (enableTransition && useGameSceneManagerTransition && Manager.GSM != null)
                {
                    // GameSceneManager의 전환 화면을 검은색으로 설정
                    Manager.GSM.SetTransitionText("스테이지 전환 중...");
                    Manager.GSM.ShowTransitionScreen();

                    LoadStage(mainStageID, subStageID);
                    yield return new WaitForSeconds(0.1f);

                    Manager.GSM.HideTransitionScreen();
                }
                else
                {
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionWithFadeInCoroutine 완료");
        }

        #endregion

        #region Stage Progression

        public void ClearCurrentStageAndNext()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"스테이지 클리어: {currentMainStage}-{currentSubStage}");

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }

            LoadStage(nextStage.mainStage, nextStage.subStage);
            Debug.Log($"다음 스테이지로 전환: {nextStage.mainStage}-{nextStage.subStage}");
        }

        public void ClearCurrentStageAndNextWithTransition()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"스테이지 클리어 (전환 화면 사용): {currentMainStage}-{currentSubStage}");

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어! 게임 클리어 처리");
                OnGameComplete();
                return;
            }

            StartStageTransition(nextStage.mainStage, nextStage.subStage, false);
            Debug.Log($"다음 스테이지로 전환 (전환 화면): {nextStage.mainStage}-{nextStage.subStage}");
        }

        private (int mainStage, int subStage, bool isGameComplete) CalculateNextStage(int currentMainStage, int currentSubStage)
        {
            int nextSubStage = currentSubStage + 1;

            if (nextSubStage > maxSubStages)
            {
                int nextMainStage = currentMainStage + 1;

                if (nextMainStage > maxMainStages)
                {
                    return (0, 0, true); // 게임 클리어
                }

                return (nextMainStage, 1, false);
            }
            else
            {
                return (currentMainStage, nextSubStage, false);
            }
        }

        private void OnGameComplete()
        {
            Debug.Log("게임 클리어!");

            if (Manager.GSM != null)
            {
                // 결과 화면으로 이동
            }
        }

        #endregion

        #region Public API

        public void OnStageButtonClick(int mainStageID, int subStageID = 1)
        {
            if (!isTransitioning)
            {
                StartStageTransition(mainStageID, subStageID, false);
            }
        }

        public void ForceStage(int mainStageID, int subStageID)
        {
            Debug.Log($"강제 스테이지 이동: {mainStageID}-{subStageID}");
            LoadStage(mainStageID, subStageID);
        }

        public void ForceStageWithTransition(int mainStageID, int subStageID)
        {
            Debug.Log($"강제 스테이지 이동 (전환 화면 사용): {mainStageID}-{subStageID}");
            StartStageTransition(mainStageID, subStageID, false);
        }

        public void OnStageCompleted()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            int score = Manager.Score.Score; // 현재 점수 가져오기

            Debug.Log($"스테이지 완료: {currentMainStage}-{currentSubStage}, 점수: {score}");

            if (dataManager != null)
            {
                dataManager.UpdateStageScore(currentMainStage, currentSubStage, score);
                dataManager.CompleteStage(currentMainStage, currentSubStage, Time.time);
                UnlockNextStage(currentMainStage, currentSubStage);
            }

            if (Manager.Game != null)
            {
                Manager.Game.SetGameClear();
            }
        }

        public void ResetStageProgress()
        {
            PlayerPrefs.SetInt("SelectedMainStage", 1);
            PlayerPrefs.SetInt("SelectedSubStage", 1);
            PlayerPrefs.Save();
            LoadStage(1, 1);
            Debug.Log("스테이지 진행 상태 초기화: 1-1");
        }

        public void ChangeStage(int newStageID)
        {
            Debug.Log($"ChangeStage 호출: 스테이지 {newStageID}로 변경");
            LoadStage(newStageID);
        }

        #endregion

        #region Utility Methods

        public StageData GetStageData(int stageID)
        {
            return stageDataList.Find(data => data.stageID == stageID);
        }

        public int GetCurrentSubStageID()
        {
            return PlayerPrefs.GetInt("SelectedSubStage", 1);
        }

        public SubStageData GetCurrentSubStageData()
        {
            if (currentStage == null) return null;

            int subStageID = GetCurrentSubStageID();
            return currentStage.subStages?.Find(s => s.subStageID == subStageID);
        }

        public bool IsTransitioning => isTransitioning;
        public bool IsMapTransitioning => isMapTransitioning;

        public void SetTransitionEnabled(bool enabled)
        {
            enableTransition = enabled;
        }

        public void SetUseGameSceneManagerTransition(bool use)
        {
            useGameSceneManagerTransition = use;
        }

        public void SetMapConnectionEnabled(bool enabled)
        {
            enableMapConnection = enabled;
        }

        public void SetMapTransitionEnabled(bool enabled)
        {
            enableMapTransition = enabled;
        }

        public void SetMapTransitionDuration(float duration)
        {
            mapTransitionDuration = Mathf.Max(0.1f, duration);
        }

        public int GetActiveMapCount()
        {
            return spawnedMaps.Count;
        }

        public int GetBufferMapCount()
        {
            return mapBuffer.Count;
        }

        private void UnlockNextStage(int currentMainStage, int currentSubStage)
        {
            if (dataManager == null) return;

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("모든 스테이지 클리어!");
                return;
            }

            dataManager.UnlockStage(nextStage.mainStage);
            dataManager.UnlockSubStage(nextStage.mainStage, nextStage.subStage);

            Debug.Log($"다음 스테이지 해금: {nextStage.mainStage}-{nextStage.subStage}");
        }

        private bool CanLoadStage(int mainStageID, int subStageID)
        {
            if (dataManager == null) return true;

            return dataManager.IsStageUnlocked(mainStageID) &&
                   dataManager.IsSubStageUnlocked(mainStageID, subStageID);
        }

        #endregion
    }
}
