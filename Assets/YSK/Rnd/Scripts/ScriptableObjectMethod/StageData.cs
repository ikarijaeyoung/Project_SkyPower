using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YSK;

namespace YSK
{
    [CreateAssetMenu(menuName = "Stage/StageData")]
    public class StageData : ScriptableObject
    {
        [Header("Basic Info")]
        public int stageID;
        public string stageName;
        public string sceneName;
        public float duration;
        public bool mainStageLock;
        
        [Header("Map Settings")]
        public List<GameObject> mapPrefabs;
        
        [Header("Sub Stage Settings")]
        [Tooltip("서브 스테이지별 설정 (현재는 모든 서브 스테이지가 같은 맵 사용)")]
        public List<SubStageData> subStages = new List<SubStageData>();

    }
    
    [System.Serializable]
    public class SubStageData
    {
        [Header("Sub Stage Info")]
        public int subStageID;
        public string subStageName;
        public bool subStageLock;
        public int subStageScore = 0;
        public StageEnemyData stageEnemyData;

        [Header("Map Override")]
        [Tooltip("이 서브 스테이지 전용 맵 프리팹 (null이면 기본 맵 사용)")]
        public List<GameObject> customMapPrefabs;


    }
}
