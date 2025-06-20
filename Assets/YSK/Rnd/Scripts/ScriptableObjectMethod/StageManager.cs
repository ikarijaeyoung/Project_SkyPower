using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    [Header ("StageData")]
    [SerializeField] private List<StageData> stageDataList; // 모든 스테이지의 데이터
    private  StageData  currentStage;
    private List<GameObject> MapPrefabs;
    [SerializeField] int selectedstageID; // Test용 Stage ID의 경우 외부 선택에 의해서 전해지게 되는 값으로 설정해야함.

    [Header ("MoveInfo")]
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform startPoint;
    [SerializeField] private float speed = 3f;

    private List<GameObject> spawnedMaps = new(); // 프리팹을 이용한 Stage Map 생성

    private Queue<GameObject> mapQueue = new(); // 이동순서를 위한 큐
    private List<GameObject> movingMaps = new(); // 현재 이동중인 맵.

    private void Awake()
    {


    }

    private void Start()
    {
        LoadStage(selectedstageID);
       
    }

    private void Update()
    {
        UpdateMovingMaps();
    }


    private void LoadStage(int stageID)
    {
        currentStage = stageDataList.Find(data => data.stageID == stageID);

        if (currentStage == null)
        {
            Debug.LogError($"Stage ID {stageID} not found!");
            return;
        }

        // 초기 배치를 어떻게 할것인가?

        SpawnMaps();

    }



    private void SpawnMaps()
    {
        spawnedMaps.Clear();
        mapQueue.Clear();
        movingMaps.Clear();

        for (int i = 0; i < currentStage.mapPrefabs.Count; i++)
        {
            GameObject map = Instantiate(currentStage.mapPrefabs[i]);

            if (i == 0)
            {
                map.transform.position = Vector3.zero;
                movingMaps.Add(map);
            }
            else if (i == 1)
            {
                map.transform.position = startPoint.position;
                movingMaps.Add(map);
            }
            else
            {
                map.transform.position = startPoint.position + Vector3.back * (10f * (i - 1));
                mapQueue.Enqueue(map);
            }

            spawnedMaps.Add(map);
        }

    }



    private void MoveMap(GameObject map, int index)
    {
        map.transform.position = Vector3.MoveTowards(
            map.transform.position,
            endPoint.position,
            speed * Time.deltaTime
        );

        // 도착 판정
        if (Vector3.Distance(map.transform.position, endPoint.position) < 0.01f)
        {
            // 해당 맵을 다시 startPoint로 보내고 큐 뒤로
            map.transform.position = startPoint.position;
            mapQueue.Enqueue(map);

            // 다음 맵을 꺼내 이동 리스트에 넣기
            if (mapQueue.Count > 0)
            {
                movingMaps[index] = mapQueue.Dequeue();
            }
        }
    }


    private void UpdateMovingMaps()
    {
        for (int i = 0; i < movingMaps.Count; i++)
        {
            MoveMap(movingMaps[i], i);
        }
    }
}
