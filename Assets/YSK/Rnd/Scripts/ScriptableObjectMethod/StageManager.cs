using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YSK;
using System.Collections;

namespace YSK
{
    public class StageManager : MonoBehaviour
    {
        [Header("StageData")]
        [SerializeField] private List<StageData> stageDataList; // 모든 스테이지의 데이터
        private StageData currentStage;
        private List<GameObject> MapPrefabs;
        [SerializeField] int selectedstageID; // Test용 Stage ID의 경우 외부 선택에 의해서 전해지게 되는 값으로 설정해야함.

        [Header("MoveInfo")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;

        [Header("References")]
        [SerializeField] private GameStateManager gameStateManager;

        private List<GameObject> spawnedMaps = new(); // 프리팹을 이용한 Stage Map 생성

        private List<GameObject> movingMaps = new(); // 현재 이동중인 맵.
        private StageTransition stageControl;

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
            StartCoroutine(WaitForGameStateManager());
            
            Debug.Log("=== StageManager Start 완료 ===");
        }

        private IEnumerator WaitForGameStateManager()
        {
            Debug.Log("=== WaitForGameStateManager 시작 ===");
            
            int waitCount = 0;
            // GameStateManager가 준비될 때까지 대기
            while (GameStateManager.Instance == null)
            {
                waitCount++;
                if (waitCount % 60 == 0) // 1초마다 로그
                {
                    Debug.Log($"GameStateManager 대기 중... ({waitCount}프레임)");
                }
                yield return null;
            }
            
            Debug.Log($"GameStateManager 발견! ({waitCount}프레임 대기)");
            
            // 이벤트 구독
            GameStateManager.OnStageChanged += OnStageChanged;
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
            
            Debug.Log("StageManager 이벤트 구독 완료");
            
            // GameStateManager가 이미 Playing 상태라면 강제로 맵 생성 트리거
            if (GameStateManager.Instance.CurrentGameState == GameState.Playing)
            {
                Debug.Log("GameStateManager가 이미 Playing 상태입니다. 맵 생성 트리거");
                OnGameStateChanged(GameState.Playing);
            }
            
            Debug.Log("=== WaitForGameStateManager 완료 ===");
        }

        private void Update()
        {
            UpdateMovingMaps();
            CheckInput();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            GameStateManager.OnStageChanged -= OnStageChanged;
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }

        #endregion

        /// <summary>
        /// GameStateManager에서 스테이지 변경 이벤트를 받았을 때 호출됩니다.
        /// </summary>
        private void OnStageChanged(int newStageID)
        {
            Debug.Log($"GameStateManager에서 스테이지 변경 요청: {newStageID}");
            LoadStage(newStageID);
        }

        /// <summary>
        /// GameStateManager에서 게임 상태 변경 이벤트를 받았을 때 호출됩니다.
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"=== OnGameStateChanged 호출: {newState} ===");

            switch (newState)
            {
                case GameState.Playing:
                    Debug.Log($"Playing 상태 처리 시작 - spawnedMaps.Count: {spawnedMaps.Count}");
                    
                    // 이미 스테이지가 로드되어 있지 않은 경우에만 로드
                    if (spawnedMaps.Count == 0)
                    {
                        Debug.Log("게임 시작: 스테이지 로드");
                        // PlayerPrefs에서 메인 스테이지 정보 가져오기
                        int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
                        Debug.Log($"PlayerPrefs에서 가져온 메인 스테이지: {mainStageID}");
                        LoadStage(mainStageID);
                    }
                    else
                    {
                        Debug.Log($"게임 시작: 이미 스테이지가 로드됨 (맵 개수: {spawnedMaps.Count})");
                    }
                    break;

                case GameState.MainMenu:
                case GameState.StageSelect:
                    // 메인메뉴나 스테이지 선택 시에는 기존 스테이지 정리
                    Debug.Log("메인메뉴/스테이지 선택: 기존 맵 정리");
                    ClearAllMaps();
                    break;
            }
            
            Debug.Log($"=== OnGameStateChanged 완료: {newState} ===");
        }

        /// <summary>
        /// 필요한 컴포넌트들을 초기화합니다.
        /// </summary>
        private void InitializeComponents()
        {
            FindStageTransition();
            FindTransformPoints();
            FindGameStateManager();
        }

        /// <summary>
        /// GameStateManager를 찾습니다.
        /// </summary>
        private void FindGameStateManager()
        {
            if (gameStateManager == null)
            {
                gameStateManager = GameStateManager.Instance;
                if (gameStateManager == null)
                {
                    Debug.LogWarning("GameStateManager를 찾을 수 없습니다! (씬 전환 중이거나 아직 초기화되지 않았을 수 있습니다)");
                }
            }
        }

        /// <summary>
        /// 자식 오브젝트에서 StageTransition을 찾습니다.
        /// </summary>
        private void FindStageTransition()
        {
            stageControl = GetComponentInChildren<StageTransition>();

            if (stageControl == null)
            {
                Debug.LogWarning("StageTransition 컴포넌트를 자식 오브젝트에서 찾을 수 없습니다! (해당 씬에서만 사용되는 컴포넌트입니다)");
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

        private void CheckInput()
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) HandleKey(1);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleKey(2);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleKey(3);
                else if (Input.GetKeyDown(KeyCode.Alpha4)) HandleKey(4);
                else if (Input.GetKeyDown(KeyCode.Alpha5)) HandleKey(5);
            }
        }

        private void HandleKey(int keyNumber)
        {
            switch (keyNumber)
            {
                case 1:
                    Debug.Log("1번 키: 스테이지 클리어 및 다음 스테이지로");
                    ClearCurrentStageAndNext();
                    break;
                case 2:
                    Debug.Log("2번 키: 강제 맵 생성 테스트");
                    ForceTestMapGeneration();
                    break;
                case 3:
                    Debug.Log("3번 키: 스테이지 진행 상태 초기화");
                    ResetStageProgress();
                    break;
                case 4:
                    Debug.Log("4번 키: 2-3으로 강제 이동");
                    ForceStage(2, 3);
                    break;
                case 5:
                    Debug.Log("5번 키: 현재 상태 정보 출력");
                    DebugCurrentState();
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

            // PlayerPrefs에서 서브 스테이지 정보 가져오기
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
                    Debug.Log($"서브 스테이지 {subStageID} 전용 맵 사용");
                }
                else
                {
                    Debug.Log($"서브 스테이지 {subStageID}는 기본 맵 사용");
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
            Debug.Log($"ChangeStage 호출: 스테이지를 {newStageID}로 변경");

            // LoadStage에서 이미 ClearAllMaps를 호출하므로 여기서는 생략
            // 기존 스테이지와 새 스테이지의 맵을 교체
            LoadStage(newStageID);

            // PlayerPrefs에서 메인 스테이지와 서브 스테이지 정보 가져오기
            int mainStageID = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStageID = PlayerPrefs.GetInt("SelectedSubStage", 1);

            // UI 텍스트 업데이트 - 메인-서브 형태로 표시
            if (UIFactory.Instance != null)
            {
                UIFactory.Instance.UpdateStageText($"{mainStageID}-{subStageID}");
            }
            else
            {
                Debug.LogWarning("UIFactory.Instance가 null입니다. UIFactory를 찾아보겠습니다.");
                UIFactory uiFactory = FindObjectOfType<UIFactory>();
                if (uiFactory != null)
                {
                    uiFactory.UpdateStageText($"{mainStageID}-{subStageID}");
                }
                else
                {
                    Debug.LogWarning("UIFactory를 찾을 수 없습니다! UI 텍스트 업데이트를 건너뜁니다.");
                }
            }

            // 3. 플레이어/카메라 위치 초기화
            // player.transform.position = ...;
            // camera.transform.position = ...;

            // 4. UI 및 게임 상태 초기화
            // UpdateUI();
            // SetGameState(GameState.Ready);
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
        /// 현재 스테이지의 데이터를 반환합니다.
        /// </summary>
        /// <returns>현재 스테이지의 데이터</returns>
        public StageData GetCurrentStageData()
        {
            return currentStage;
        }

        /// <summary>
        /// 현재 생성된 맵 오브젝트 리스트를 반환합니다.
        /// </summary>
        /// <returns>생성된 맵 리스트</returns>
        public List<GameObject> GetSpawnedMaps()
        {
            return new List<GameObject>(spawnedMaps);
        }

        /// <summary>
        /// 현재 이동 중인 맵 오브젝트 리스트를 반환합니다.
        /// </summary>
        /// <returns>이동 중인 맵 리스트</returns>
        public List<GameObject> GetMovingMaps()
        {
            return new List<GameObject>(movingMaps);
        }

        /// <summary>
        /// 특정 맵을 제거합니다.
        /// </summary>
        /// <param name="map">제거할 맵 오브젝트</param>
        public void RemoveMap(GameObject map)
        {
            if (spawnedMaps.Contains(map))
            {
                spawnedMaps.Remove(map);
                movingMaps.Remove(map);
                Destroy(map);
            }
        }

        /// <summary>
        /// 모든 맵을 제거합니다.
        /// </summary>
        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps 시작: {spawnedMaps.Count}개 맵 제거");

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
        /// 맵 이동을 일시정지합니다.
        /// </summary>
        public void PauseMapMovement()
        {
            enabled = false;
        }

        /// <summary>
        /// 맵 이동을 재개합니다.
        /// </summary>
        public void ResumeMapMovement()
        {
            enabled = true;
        }

        /// <summary>
        /// 현재 선택된 메인 스테이지 ID를 반환합니다.
        /// </summary>
        /// <returns>메인 스테이지 ID</returns>
        public int GetCurrentMainStageID()
        {
            return PlayerPrefs.GetInt("SelectedMainStage", 1);
        }

        /// <summary>
        /// 현재 선택된 서브 스테이지 ID를 반환합니다.
        /// </summary>
        /// <returns>서브 스테이지 ID</returns>
        public int GetCurrentSubStageID()
        {
            return PlayerPrefs.GetInt("SelectedSubStage", 1);
        }

        /// <summary>
        /// 현재 서브 스테이지의 데이터를 반환합니다.
        /// </summary>
        /// <returns>서브 스테이지 데이터 또는 null</returns>
        public SubStageData GetCurrentSubStageData()
        {
            if (currentStage == null) return null;

            int subStageID = GetCurrentSubStageID();
            return currentStage.subStages?.Find(s => s.subStageID == subStageID);
        }

        /// <summary>
        /// 현재 스테이지의 난이도를 반환합니다.
        /// </summary>
        /// <returns>난이도 (1-5)</returns>
        public float GetCurrentDifficulty()
        {
            if (currentStage == null) return 1f;

            int subStageID = GetCurrentSubStageID();
            SubStageData subStageData = GetCurrentSubStageData();

            // 서브 스테이지별 커스텀 난이도가 있으면 사용
            if (subStageData != null && subStageData.customDifficulty > 0)
            {
                return subStageData.customDifficulty;
            }

            // 기본 난이도 + 서브 스테이지별 증가량
            return currentStage.baseDifficulty + (subStageID - 1) * currentStage.difficultyIncreasePerSubStage;
        }

        /// <summary>
        /// GameStateManager 참조를 설정합니다.
        /// </summary>
        /// <param name="newGameStateManager">설정할 GameStateManager 인스턴스</param>
        public void SetGameStateManager(GameStateManager newGameStateManager)
        {
            gameStateManager = newGameStateManager;

            if (gameStateManager != null)
            {
                Debug.Log("StageManager에 GameStateManager 참조 설정 완료");
                
                // 이벤트 구독 (중복 방지)
                GameStateManager.OnStageChanged -= OnStageChanged;
                GameStateManager.OnGameStateChanged -= OnGameStateChanged;
                GameStateManager.OnStageChanged += OnStageChanged;
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
                
                Debug.Log("StageManager 이벤트 구독 완료 (SetGameStateManager에서)");
            }
            else
            {
                Debug.LogWarning("StageManager에서 GameStateManager 참조가 null로 설정되었습니다.");
            }
        }

        /// <summary>
        /// 현재 설정된 GameStateManager 참조를 반환합니다.
        /// </summary>
        /// <returns>GameStateManager 인스턴스 또는 null</returns>
        public GameStateManager GetGameStateManager()
        {
            return gameStateManager;
        }

        /// <summary>
        /// StageTransition 참조를 설정합니다.
        /// </summary>
        /// <param name="newStageTransition">설정할 StageTransition 인스턴스</param>
        public void SetStageTransition(StageTransition newStageTransition)
        {
            stageControl = newStageTransition;

            if (stageControl != null)
            {
                Debug.Log("StageManager에 StageTransition 참조 설정 완료");
            }
            else
            {
                Debug.LogWarning("StageManager에서 StageTransition 참조가 null로 설정되었습니다.");
            }
        }

        /// <summary>
        /// 현재 설정된 StageTransition 참조를 반환합니다.
        /// </summary>
        /// <returns>StageTransition 인스턴스 또는 null</returns>
        public StageTransition GetStageTransition()
        {
            return stageControl;
        }

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
            
            // 다음 스테이지로 이동
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
            // 서브 스테이지 최대값 (5개)
            const int MAX_SUB_STAGES = 5;
            
            // 다음 서브 스테이지 계산
            int nextSubStage = currentSubStage + 1;
            
            // 서브 스테이지가 최대값을 넘으면 다음 메인 스테이지로
            if (nextSubStage > MAX_SUB_STAGES)
            {
                int nextMainStage = currentMainStage + 1;
                
                // 메인 스테이지 최대값 (4개)
                const int MAX_MAIN_STAGES = 4;
                
                // 메인 스테이지가 최대값을 넘으면 게임 클리어
                if (nextMainStage > MAX_MAIN_STAGES)
                {
                    return (0, 0, true); // 게임 클리어
                }
                
                // 다음 메인 스테이지의 첫 번째 서브 스테이지로
                return (nextMainStage, 1, false);
            }
            else
            {
                // 같은 메인 스테이지의 다음 서브 스테이지로
                return (currentMainStage, nextSubStage, false);
            }
        }

        /// <summary>
        /// 특정 메인 스테이지의 최대 서브 스테이지 수를 반환합니다.
        /// </summary>
        private int GetMaxSubStageCount(int mainStageID)
        {
            // StageData에서 실제 서브 스테이지 수를 가져오거나, 기본값 5 사용
            StageData stageData = stageDataList?.Find(data => data.stageID == mainStageID);
            if (stageData != null && stageData.subStages != null)
            {
                return stageData.subStages.Count;
            }
            return 5; // 기본값
        }

        /// <summary>
        /// 전체 메인 스테이지 수를 반환합니다.
        /// </summary>
        private int GetMaxMainStageCount()
        {
            return stageDataList != null ? stageDataList.Count : 4; // 기본값
        }

        /// <summary>
        /// 게임 클리어 시 호출됩니다.
        /// </summary>
        private void OnGameComplete()
        {
            Debug.Log("게임 클리어!");
            
            // GameStateManager에 게임 클리어 알림
            if (gameStateManager != null)
            {
                gameStateManager.SetGameState(GameState.StageComplete);
            }
            
            // 결과 화면으로 이동
            if (GameSceneManager.Instance != null)
            {
                //GameSceneManager.Instance.LoadResultScene(1000, true); // 예시 점수
            }
        }

        /// <summary>
        /// 스테이지 진행을 테스트하는 메서드입니다.
        /// </summary>
        public void TestStageProgression()
        {
            Debug.Log("=== 스테이지 진행 테스트 ===");
            
            // 1-1부터 시작해서 모든 스테이지 진행 테스트
            for (int mainStage = 1; mainStage <= 4; mainStage++)
            {
                for (int subStage = 1; subStage <= 5; subStage++)
                {
                    var nextStage = CalculateNextStage(mainStage, subStage);
                    
                    if (nextStage.isGameComplete)
                    {
                        Debug.Log($"{mainStage}-{subStage} → 게임 클리어!");
                        break;
                    }
                    else
                    {
                        Debug.Log($"{mainStage}-{subStage} → {nextStage.mainStage}-{nextStage.subStage}");
                    }
                }
            }
            
            Debug.Log("=== 스테이지 진행 테스트 완료 ===");
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
            Debug.Log($"GameStateManager: {(GameStateManager.Instance != null ? "존재" : "null")}");
            Debug.Log($"PlayerPrefs - Main: {PlayerPrefs.GetInt("SelectedMainStage", 1)}, Sub: {PlayerPrefs.GetInt("SelectedSubStage", 1)}");
            Debug.Log("=== 상태 정보 완료 ===");
        }
    }
}