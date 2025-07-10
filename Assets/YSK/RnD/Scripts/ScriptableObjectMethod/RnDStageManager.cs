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

        [Header("Map Connection System")]
        [SerializeField] private Transform endPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private float speed = 3f;
        [SerializeField] private float mapLength = 20;
        [SerializeField] private float mapConnectionDistance = 0.1f;
        [SerializeField] private float mapTransitionDuration = 1f;
        [SerializeField] private AnimationCurve mapTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private int mapRepeatCount = 2; // �� ���� �� ���� ��������

        // �� ���� ����
        private List<GameObject> oldMaps = new(); // ���� �ʵ� (���ŵ�)
        private List<GameObject> newMaps = new(); // ���� ������ �ʵ� (���� �ݺ�)
        private List<GameObject> currentStageMapPrefabs = new(); // ���� ���������� �� �����յ�
        private int currentMapIndex = 0; // ���� ���� ���� �� �ε���
        private bool isMapTransitioning = false;

        [Header("Transition Settings")]
        [SerializeField] private bool useGameSceneManagerTransition = true;
        [SerializeField] private bool enableTransition = true;

        private bool isTransitioning = false;

        // �̺�Ʈ
        public static System.Action<int, int> OnStageChanged;
        public static System.Action OnMapTransitionStarted;
        public static System.Action OnMapTransitionCompleted;

        #region Unity Lifecycle

        private void Awake()
        {
            // �ּ����� �ʱ�ȭ��
        }

        private void Start()
        {
            Debug.Log("=== StageManager Start ���� ===");
            InitializeComponents();
            //Manager.Game.onGameClear.AddListener(OnGameComplete); // ���� Ŭ���� �̺�Ʈ ������ ���
            Debug.Log("=== StageManager Start �Ϸ� ===");
        }

        private void Update()
        {
            if (!isMapTransitioning)
            {
                UpdateMapMovement();
                UpdateMapConnections();
               
            }
            
            // �����: F1 Ű�� �� ���� Ȯ��
            if (Input.GetKeyDown(KeyCode.F1))
            {
                DebugMapStatus();
            }
            
            // �����: F2 Ű�� ClearCurrentStageAndNext �׽�Ʈ
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Debug.Log("F2 Ű�� ClearCurrentStageAndNext �׽�Ʈ ����");
                ClearCurrentStageAndNext();
            }
            
            // �����: F3 Ű�� ���� �� ���� �׽�Ʈ
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Debug.Log("F3 Ű�� ���� �� ���� �׽�Ʈ ����");
                ProgressToNextMap();
            }
        }

        private void OnDestroy()
        {
            //Manager.Game.onGameClear.RemoveListener(OnGameComplete); // ���� Ŭ���� �̺�Ʈ ������ ����
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
                    Debug.LogWarning("StartPoint�� ã�� �� �����ϴ�!");
                }
            }

            if (endPoint == null)
            {
                endPoint = transform.Find("EndPoint");
                if (endPoint == null)
                {
                    Debug.LogWarning("EndPoint�� ã�� �� �����ϴ�!");
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
                    Debug.LogWarning("StageDataManager�� ã�� �� �����ϴ�!");
                }
            }
        }

        #endregion

        #region Map Connection System

        private void UpdateMapConnections()
        {
            // ��� �ʵ��� �ϳ��� ����Ʈ�� ���ļ� ����
            var allMaps = new List<GameObject>();
            allMaps.AddRange(oldMaps);
            allMaps.AddRange(newMaps);
            
            if (allMaps.Count < 2) return;

            // �ʵ��� Z ��ġ ������ ����
            var sortedMaps = allMaps.OrderBy(m => m.transform.position.z).ToList();

            for (int i = 0; i < sortedMaps.Count - 1; i++)
            {
                GameObject currentMap = sortedMaps[i];
                GameObject nextMap = sortedMaps[i + 1];

                if (currentMap == null || nextMap == null) continue;

                float distance = Mathf.Abs(nextMap.transform.position.z - currentMap.transform.position.z);
                
                // �� ������ �ʹ� ũ�� ����
                if (distance > mapLength + mapConnectionDistance)
                {
                    Vector3 newPosition = currentMap.transform.position;
                    newPosition.z += mapLength;
                    nextMap.transform.position = newPosition;
                    
                    Debug.Log($"�� ���� ����: {distance:F2} -> {mapLength:F2}");
                }
            }
        }

        private void UpdateMapMovement()
        {
            // ���� �ʵ� �̵�
            foreach (var map in oldMaps.ToList())
            {
                if (map != null)
                {
                    MoveOldMap(map);
                }
                else
                {
                    oldMaps.Remove(map);
                }
            }

            // �� �ʵ� �̵�
            foreach (var map in newMaps.ToList())
            {
                if (map != null)
                {
                    MoveNewMap(map);
                }
                else
                {
                    newMaps.Remove(map);
                }
            }
        }

        private void MoveOldMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint�� null�Դϴ�!");
                return;
            }

            // ���� ���� ������ ������ ����
            if (map.transform.position.z < endPoint.position.z)
            {
                oldMaps.Remove(map);
                Destroy(map);
                Debug.Log($"���� ���� EndPoint�� ���� ���ŵ�: {map.name}");
            }
        }

        private void MoveNewMap(GameObject map)
        {
            if (map == null) return;

            map.transform.position += Vector3.back * speed * Time.deltaTime;

            if (endPoint == null)
            {
                Debug.LogWarning("endPoint�� null�Դϴ�!");
                return;
            }

            // �� ���� ������ ������ �������� �̵� (���� �ݺ�)
            if (map.transform.position.z < endPoint.position.z)
            {
                float maxZ = GetMaxZPosition();
                if (maxZ == float.MinValue)
                {
                    maxZ = startPoint != null ? startPoint.position.z : 0f;
                }
                else
                {
                    maxZ += mapLength;
                }

                Vector3 newPosition = map.transform.position;
                newPosition.z = maxZ;
                map.transform.position = newPosition;

                Debug.Log($"�� ���� EndPoint�� ���� �������� �̵�: {map.name} - �� ��ġ: {newPosition}");
            }
        }

        #endregion

        #region Map Management

        private void LoadStage(int mainStageID, int subStageID = 1)
        {
            Debug.Log($"=== LoadStage ����: {mainStageID}-{subStageID} ===");

            // �� ���� ��ȯ ����
            StartCoroutine(ConnectMapTransition(mainStageID, subStageID));

            Debug.Log($"=== LoadStage �Ϸ�: {mainStageID}-{subStageID} ===");
        }

        private IEnumerator ConnectMapTransition(int mainStageID, int subStageID)
        {
            Debug.Log($"�� ���� ��ȯ ����: {mainStageID}-{subStageID}");
            isMapTransitioning = true;
            OnMapTransitionStarted?.Invoke();

            // 1. �� �������� ������ �غ�
            UpdatePlayerPrefs(mainStageID, subStageID);

            if (!ValidateStageData(mainStageID))
            {
                Debug.LogError($"�������� ������ ���� ����: {mainStageID}");
                isMapTransitioning = false;
                yield break;
            }

            // 2. �� �������� �����͸� ���� ����
            StageData newStage = stageDataList.Find(data => data.stageID == mainStageID);
            currentStage = newStage;

            // 3. ���� �� �ʵ��� oldMaps�� �̵� (���� ���)
            MoveNewMapsToOldMaps();

            // 4. �� �� ������ ��������
            List<GameObject> newMapPrefabs = GetMapPrefabsForSubStage(subStageID);

            if (newMapPrefabs == null || newMapPrefabs.Count == 0)
            {
                Debug.LogWarning($"�� �������� {mainStageID}�� �� �������� �����ϴ�!");
                isMapTransitioning = false;
                yield break;
            }

            // 5. �� �ʵ��� newMaps ����Ʈ�� ����
            yield return StartCoroutine(SpawnAndConnectNewMaps(newMapPrefabs));

            // 6. �̺�Ʈ �߻�
            OnStageChanged?.Invoke(mainStageID, subStageID);

            isMapTransitioning = false;
            OnMapTransitionCompleted?.Invoke();
            Debug.Log("�� ���� ��ȯ �Ϸ�");
        }

        private IEnumerator SpawnAndConnectNewMaps(List<GameObject> mapPrefabs)
        {
            Debug.Log($"�� �ʵ� ���� ����: {mapPrefabs.Count}�� ������");

            // ���� �������� �� ������ ����
            currentStageMapPrefabs.Clear();
            currentStageMapPrefabs.AddRange(mapPrefabs);
            currentMapIndex = 0;

            // ù ��° ���� mapRepeatCount��ŭ ����
            yield return StartCoroutine(SpawnCurrentMapWithRepeat());

            Debug.Log($"ù ��° �� ���� �Ϸ�: {newMaps.Count}��");
        }

        // ���� �� �ε����� ���� �ݺ� �����ϴ� �޼���
        private IEnumerator SpawnCurrentMapWithRepeat()
        {
            if (currentMapIndex >= currentStageMapPrefabs.Count)
            {
                Debug.LogWarning("�� �̻� ������ ���� �����ϴ�!");
                yield break;
            }

            GameObject currentMapPrefab = currentStageMapPrefabs[currentMapIndex];
            if (currentMapPrefab == null)
            {
                Debug.LogError($"�� ������ {currentMapIndex}�� null�Դϴ�!");
                yield break;
            }

            Debug.Log($"���� �� {currentMapIndex} ({currentMapPrefab.name})�� {mapRepeatCount}�� ����");

            // ��� �ʵ� �� ���� ���� Z ��ġ ã��
            float maxZ = GetMaxZPosition();
            if (maxZ == float.MinValue)
            {
                maxZ = startPoint != null ? startPoint.position.z : 0f;
            }
            else
            {
                maxZ += mapLength;
            }

            // ���� ���� mapRepeatCount��ŭ ����
            for (int i = 0; i < mapRepeatCount; i++)
            {
                GameObject map = Instantiate(currentMapPrefab);
                Vector3 spawnPosition = new Vector3(
                    startPoint != null ? startPoint.position.x : 0f,
                    startPoint != null ? startPoint.position.y : 0f,
                    maxZ + (mapLength * i)
                );

                map.transform.position = spawnPosition;
                newMaps.Add(map);

                Debug.Log($"�� ����: {map.name} (�ݺ� {i + 1}/{mapRepeatCount}) - ��ġ: {spawnPosition}");

                // �ε巯�� ���� ȿ�� (ù ��°��)
                if (i == 0)
                {
                    yield return StartCoroutine(FadeInMap(map));
                }
                else
                {
                    CanvasGroup cg = map.GetComponent<CanvasGroup>();
                    if (cg == null) cg = map.AddComponent<CanvasGroup>();
                    cg.alpha = 1f;
                }
            }

            Debug.Log($"�� {currentMapIndex} ���� �Ϸ�: {mapRepeatCount}��");
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

        private float GetMaxZPosition()
        {
            float maxZ = float.MinValue;
            
            // ���� �ʵ鿡�� �ִ� Z ã��
            foreach (var map in oldMaps)
            {
                if (map != null && map.transform.position.z > maxZ)
                    maxZ = map.transform.position.z;
            }
            
            // �� �ʵ鿡���� �ִ� Z ã��
            foreach (var map in newMaps)
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
                Debug.LogError("�������� ������ ����Ʈ�� ����ֽ��ϴ�!");
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

        private List<GameObject> GetMapPrefabsForSubStage(int subStageID)
        {
            // currentStage�� null�� �� �����Ƿ� �����ϰ� ó��
            if (currentStage == null)
            {
                Debug.LogWarning("currentStage�� null�Դϴ�. �⺻ �� �������� ����մϴ�.");
                return stageDataList.Count > 0 ? stageDataList[0].mapPrefabs : new List<GameObject>();
            }

            List<GameObject> mapPrefabsToUse = currentStage.mapPrefabs;

            if (currentStage.subStages != null && currentStage.subStages.Count > 0)
            {
                SubStageData subStageData = currentStage.subStages.Find(s => s.subStageID == subStageID);
                if (subStageData != null && subStageData.customMapPrefabs != null && subStageData.customMapPrefabs.Count > 0)
                {
                    mapPrefabsToUse = subStageData.customMapPrefabs;
                    Debug.Log($"Ŀ���� ���꽺������ {subStageID} �� ���: {mapPrefabsToUse.Count}��");
                }
                else
                {
                    Debug.Log($"���꽺������ {subStageID} Ŀ���� �� ����, �⺻ �� ���: {mapPrefabsToUse.Count}��");
                }
            }

            return mapPrefabsToUse;
        }

        public void ClearAllMaps()
        {
            Debug.Log($"ClearAllMaps ����: ���� �� {oldMaps.Count}��, �� �� {newMaps.Count}�� ����");

            foreach (var map in oldMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            oldMaps.Clear();

            foreach (var map in newMaps)
            {
                if (map != null)
                {
                    Destroy(map);
                }
            }
            newMaps.Clear();

            Debug.Log("ClearAllMaps �Ϸ�");
        }

        #endregion

        #region Stage Transition

        public void StartStageTransition(int mainStageID, int subStageID = 1, bool isGameStart = false)
        {
            Debug.Log($"StageManager.StartStageTransition: {mainStageID}-{subStageID}, ���ӽ���: {isGameStart}");

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
                Debug.LogWarning("�̹� ��ȯ ���Դϴ�!");
            }
        }

        private IEnumerator TransitionCoroutine(int mainStageID, int subStageID, bool isGameStart)
        {
            Debug.Log($"TransitionCoroutine ����: {mainStageID}-{subStageID}");
            isTransitioning = true;

            if (isGameStart)
            {
                LoadStage(mainStageID, subStageID);
            }
            else
            {
                if (enableTransition && useGameSceneManagerTransition && Manager.GSM != null)
                {
                    // GameSceneManager�� ��ȯ ȭ�� ���
                    Manager.GSM.ShowTransitionScreen();
                    yield return new WaitForSeconds(0.5f); // ��ȯ ȭ�� ǥ�� �ð�

                    LoadStage(mainStageID, subStageID);

                    yield return new WaitForSeconds(0.1f); // �������� �ε� ���
                    Manager.GSM.HideTransitionScreen();
                }
                else
                {
                    LoadStage(mainStageID, subStageID);
                }
            }

            isTransitioning = false;
            Debug.Log("TransitionCoroutine �Ϸ�");
        }

        #endregion

        #region Stage Progression

        public void ClearCurrentStageAndNext()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"=== �������� Ŭ���� ����: {currentMainStage}-{currentSubStage} ===");

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("��� �������� Ŭ����! ���� Ŭ���� ó��");
                OnGameComplete();
                return;
            }

            // �� ���� ��ȯ ����
            StartCoroutine(ConnectMapTransition(nextStage.mainStage, nextStage.subStage));
            
            Debug.Log($"���� ���������� ��ȯ: {nextStage.mainStage}-{nextStage.subStage}");
        }

        private (int mainStage, int subStage, bool isGameComplete) CalculateNextStage(int currentMainStage, int currentSubStage)
        {
            int nextSubStage = currentSubStage + 1;

            if (nextSubStage > maxSubStages)
            {
                int nextMainStage = currentMainStage + 1;

                if (nextMainStage > maxMainStages)
                {
                    return (0, 0, true); // ���� Ŭ����
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
            Debug.Log("���� Ŭ����!");

            if (Manager.GSM != null)
            {
                // ��� ȭ������ �̵�
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
            Debug.Log($"���� �������� �̵�: {mainStageID}-{subStageID}");
            LoadStage(mainStageID, subStageID);
        }

        public void OnStageCompleted()
        {
            int currentMainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int currentSubStage = PlayerPrefs.GetInt("SelectedSubStage", 1);

            Debug.Log($"=== �������� �Ϸ� ó�� ����: {currentMainStage}-{currentSubStage} ===");

            // ���� ��������
            int score = Manager.Score?.Score ?? 0;
            Debug.Log($"���� ����: {score}");

            // StageDataManager�� ���� ������ ó��
            if (dataManager != null)
            {
                dataManager.CompleteStageWithSave(currentMainStage, currentSubStage, score, Time.time);
                Debug.Log("StageDataManager�� ���� �Ϸ� ó�� �Ϸ�");
            }
            else
            {
                Debug.LogError("StageDataManager�� null�Դϴ�!");
            }

            // ���� Ŭ���� ó��
            if (Manager.Game != null)
            {
                Manager.Game.SetGameClear();
                Debug.Log("���� Ŭ���� ó�� �Ϸ�");
            }

            Debug.Log($"=== �������� �Ϸ� ó�� �Ϸ�: {currentMainStage}-{currentSubStage} ===");
        }

        public void ResetStageProgress()
        {
            ClearAllMaps();
             PlayerPrefs.SetInt("SelectedMainStage", 1);
             PlayerPrefs.SetInt("SelectedSubStage", 1);
            PlayerPrefs.Save();
            int mainStage = PlayerPrefs.GetInt("SelectedMainStage", 1);
            int subStage = PlayerPrefs.GetInt("SelectedSubStage", 1);
            Debug.Log("�������� ���� ���� �ʱ�ȭ: 1-1");
            StartCoroutine(ConnectMapTransition(mainStage,subStage));
            Debug.Log("�������� 1-1 �� ����.");
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

        public void SetMapTransitionDuration(float duration)
        {
            mapTransitionDuration = Mathf.Max(0.1f, duration);
        }

        public int GetActiveMapCount()
        {
            return oldMaps.Count + newMaps.Count; // �� ����Ʈ ��ģ ����
        }

        private void UnlockNextStage(int currentMainStage, int currentSubStage)
        {
            if (dataManager == null) return;

            var nextStage = CalculateNextStage(currentMainStage, currentSubStage);

            if (nextStage.isGameComplete)
            {
                Debug.Log("��� �������� Ŭ����!");
                return;
            }

            dataManager.UnlockStage(nextStage.mainStage);
            dataManager.UnlockSubStage(nextStage.mainStage, nextStage.subStage);

            Debug.Log($"���� �������� �ر�: {nextStage.mainStage}-{nextStage.subStage}");
        }

        private bool CanLoadStage(int mainStageID, int subStageID)
        {
            if (dataManager == null) return true;

            return dataManager.IsStageUnlocked(mainStageID) &&
                   dataManager.IsSubStageUnlocked(mainStageID, subStageID);
        }

        #endregion

        // ����׿� �޼��� �߰�
        public void DebugMapStatus()
        {
            Debug.Log("=== �� ���� ����� ===");
            Debug.Log($"���� �� ��: {oldMaps.Count}");
            Debug.Log($"�� �� ��: {newMaps.Count}");
            Debug.Log($"�� ��ȯ ��: {isMapTransitioning}");
            Debug.Log($"���� ��������: {currentStage?.stageID ?? -1}");
            Debug.Log($"���� ���꽺������: {GetCurrentSubStageID()}");
            Debug.Log($"���� �� �ε���: {currentMapIndex}/{currentStageMapPrefabs.Count}");
            Debug.Log($"�� �ݺ� ��: {mapRepeatCount}");
            
            Debug.Log("--- ���� �ʵ� (���� ����) ---");
            for (int i = 0; i < oldMaps.Count; i++)
            {
                if (oldMaps[i] != null)
                {
                    Debug.Log($"���� �� {i}: {oldMaps[i].name} - ��ġ: {oldMaps[i].transform.position}");
                }
            }
            
            Debug.Log("--- �� �ʵ� (���� �ݺ�) ---");
            for (int i = 0; i < newMaps.Count; i++)
            {
                if (newMaps[i] != null)
                {
                    Debug.Log($"�� �� {i}: {newMaps[i].name} - ��ġ: {newMaps[i].transform.position}");
                }
            }
            
            Debug.Log("=== �� ���� ����� �Ϸ� ===");
        }

        // ���� �� �ʵ��� oldMaps�� �̵��ϴ� �޼��� �߰�
        private void MoveNewMapsToOldMaps()
        {
            Debug.Log($"���� �� �ʵ��� oldMaps�� �̵�: {newMaps.Count}��");
            
            foreach (var map in newMaps)
            {
                if (map != null)
                {
                    oldMaps.Add(map);
                    Debug.Log($"���� oldMaps�� �̵�: {map.name}");
                }
            }
            
            newMaps.Clear();
            Debug.Log($"oldMaps ����: {oldMaps.Count}, newMaps ����: {newMaps.Count}");
        }

        // ���� ������ �����ϴ� �޼��� (�ܺο��� ȣ��)
        public void ProgressToNextMap()
        {
            Debug.Log($"���� ������ ���� ��û: ���� {currentMapIndex}, ��ü {currentStageMapPrefabs.Count}");

            if (currentMapIndex >= currentStageMapPrefabs.Count - 1)
            {
                Debug.Log("������ ���Դϴ�. �������� ��ȯ�� �����մϴ�.");
                // �������� ��ȯ
                ClearCurrentStageAndNext();
                return;
            }

            // ���� �ʵ��� oldMaps�� �̵� (���� ���)
            MoveNewMapsToOldMaps();

            // ���� �� �ε����� �̵�
            currentMapIndex++;

            // ���� �� ����
            StartCoroutine(SpawnCurrentMapWithRepeat());
        }
    }
}
