using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        
    }

    private void Start()
    {
        InitializeComponents();
        
        // GameStateManager 이벤트 구독
        if (gameStateManager != null)
        {
            GameStateManager.OnStageChanged += OnStageChanged;
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
        }
        
        // 자동 스테이지 로드 제거 - GameStateManager가 제어하도록 함
        // LoadStage(selectedstageID);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (gameStateManager != null)
        {
            GameStateManager.OnStageChanged -= OnStageChanged;
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }

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
        Debug.Log($"OnGameStateChanged: {newState}");
        
        switch (newState)
        {
            case GameState.Playing:
                // 이미 스테이지가 로드되어 있지 않은 경우에만 로드
                if (spawnedMaps.Count == 0 && gameStateManager != null)
                {
                    Debug.Log("게임 시작: 스테이지 로드");
                    LoadStage(gameStateManager.CurrentStageID);
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
            gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager == null)
            {
                Debug.LogWarning("GameStateManager를 찾을 수 없습니다!");
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
            Debug.LogError("StageTransition 컴포넌트를 자식 오브젝트에서 찾을 수 없습니다!");
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
                Debug.LogError("StartPoint를 찾을 수 없습니다!");
            }
        }
        
        if (endPoint == null)
        {
            endPoint = transform.Find("EndPoint");
            if (endPoint == null)
            {
                Debug.LogError("EndPoint를 찾을 수 없습니다!");
            }
        }
    }

    private void Update()
    {
        UpdateMovingMaps();

        CheckInput();

    }


    private void CheckInput()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) HandleKey(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleKey(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleKey(3);
        }
    }

    private void HandleKey(int keyNumber)
    {
        if (stageControl == null)
        {
            Debug.LogError("StageTransition이 초기화되지 않았습니다!");
            return;
        }

        switch (keyNumber)
        {
            case 1:
                Debug.Log("1번 키: 첫 번째 스테이지 로드");
                stageControl.StartStageTransition(1, false);
                break;

            case 2:
                Debug.Log("2번 키: 두 번째 스테이지 로드");
                stageControl.StartStageTransition(2, false);
                break;

            case 3:
                Debug.Log("3번 키: 세 번째 스테이지 로드");
                stageControl.StartStageTransition(3, false);
                break;

            default:
                Debug.LogWarning("알 수 없는 키 입력");
                break;
        }
    }


    private void LoadStage(int stageID)
    {
        Debug.Log($"LoadStage 호출: 스테이지 {stageID}");
        
        // 기존 맵 정리
        ClearAllMaps();
        
        currentStage = stageDataList.Find(data => data.stageID == stageID);

        if (currentStage == null)
        {
            Debug.LogError($"Stage ID {stageID} not found!");
            return;
        }

        Debug.Log($"스테이지 {stageID} 데이터 로드 완료, 맵 생성 시작");
        SpawnMaps();
    }



    private void SpawnMaps()
    {
        Debug.Log($"SpawnMaps 시작: 기존 맵 {spawnedMaps.Count}개 정리");
        spawnedMaps.Clear();
        movingMaps.Clear();

        Debug.Log($"새 맵 생성 시작: {currentStage.mapPrefabs.Count}개 프리팹");
        for (int i = 0; i < currentStage.mapPrefabs.Count; i++)
        {
            GameObject map = Instantiate(currentStage.mapPrefabs[i]);
            // 맵을 일렬로 배치 (예: Z축 기준)
            map.transform.position = startPoint.position + Vector3.back * (mapLength * i);
            spawnedMaps.Add(map);
            movingMaps.Add(map);
            Debug.Log($"맵 {i + 1} 생성 완료: {map.name}");
        }
        
        Debug.Log($"SpawnMaps 완료: 총 {spawnedMaps.Count}개 맵 생성");
    }





    private void MoveMap(GameObject map)
    {
        map.transform.position += Vector3.back * speed * Time.deltaTime;

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
            if (map.transform.position.z > maxZ)
                maxZ = map.transform.position.z;
        }
        return maxZ;
    }

    private void UpdateMovingMaps()
    {
        foreach (var map in movingMaps)
        {
            MoveMap(map);
        }
    }

    public void ChangeStage(int newStageID)
    {
        Debug.Log($"ChangeStage 호출: 스테이지 {newStageID}로 변경");
        
        // LoadStage에서 이미 ClearAllMaps를 호출하므로 여기서는 제거
        // 새로운 스테이지 데이터 로드 및 맵 배치
        LoadStage(newStageID);

        // UI 텍스트 업데이트
        GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
        if (gameSceneManager != null)
        {
            gameSceneManager.UpdateStageText(newStageID);
        }

        // 3. 플레이어/카메라 위치 초기화
        // player.transform.position = ...;
        // camera.transform.position = ...;

        // 4. UI 및 게임 상태 초기화
        // UpdateUI();
        // SetGameState(GameState.Ready);
    }

    /// <summary>
    /// 특정 스테이지 ID의 StageData를 반환합니다.
    /// </summary>
    /// <param name="stageID">찾을 스테이지 ID</param>
    /// <returns>해당 스테이지 데이터 또는 null</returns>
    public StageData GetStageData(int stageID)
    {
        return stageDataList.Find(data => data.stageID == stageID);
    }

    /// <summary>
    /// 현재 스테이지 데이터를 반환합니다.
    /// </summary>
    /// <returns>현재 스테이지 데이터</returns>
    public StageData GetCurrentStageData()
    {
        return currentStage;
    }

    /// <summary>
    /// 현재 생성된 맵들의 리스트를 반환합니다.
    /// </summary>
    /// <returns>현재 맵들의 리스트</returns>
    public List<GameObject> GetSpawnedMaps()
    {
        return new List<GameObject>(spawnedMaps);
    }

    /// <summary>
    /// 현재 이동 중인 맵들의 리스트를 반환합니다.
    /// </summary>
    /// <returns>현재 이동 중인 맵들의 리스트</returns>
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

}
