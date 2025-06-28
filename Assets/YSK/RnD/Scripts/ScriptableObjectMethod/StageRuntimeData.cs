using System.Collections.Generic;
using UnityEngine;
using YSK;

namespace YSK
{
    [System.Serializable]
    public class StageRuntimeData
    {
        public int stageID;
        public bool isUnlocked;
        public int bestScore;
        public bool isCompleted;
        public float completionTime;
        
        public List<SubStageRuntimeData> subStages = new List<SubStageRuntimeData>();
    }
    
    [System.Serializable]
    public class SubStageRuntimeData
    {
        public int subStageID;
        public bool isUnlocked;
        public int bestScore;
        public bool isCompleted;
        public float completionTime;
    }
    
    public class StageDataManager : MonoBehaviour
    {
        [Header("Stage Data")]
        [SerializeField] private List<StageData> stageDataList;
        
        [Header("Runtime Data")]
        [SerializeField] private List<StageRuntimeData> runtimeData = new List<StageRuntimeData>();
        
        // 저장 키
        private const string STAGE_DATA_KEY = "StageRuntimeData";
        
        private void Start()
        {
            InitializeRuntimeData();
            LoadRuntimeData();
        }
        
        private void InitializeRuntimeData()
        {
            runtimeData.Clear();
            
            foreach (var stageData in stageDataList)
            {
                var runtimeStage = new StageRuntimeData
                {
                    stageID = stageData.stageID,
                    isUnlocked = stageData.mainStageLock == false, // 초기값은 ScriptableObject의 lock 상태
                    bestScore = 0,
                    isCompleted = false,
                    completionTime = 0f
                };
                
                // 서브 스테이지 데이터 초기화
                foreach (var subStageData in stageData.subStages)
                {
                    var runtimeSubStage = new SubStageRuntimeData
                    {
                        subStageID = subStageData.subStageID,
                        isUnlocked = subStageData.subStageLock == false,
                        bestScore = subStageData.subStageScore,
                        isCompleted = false,
                        completionTime = 0f
                    };
                    
                    runtimeStage.subStages.Add(runtimeSubStage);
                }
                
                runtimeData.Add(runtimeStage);
            }
        }
        
        /// <summary>
        /// 런타임 데이터를 PlayerPrefs에 저장
        /// </summary>
        public void SaveRuntimeData()
        {
            string json = JsonUtility.ToJson(new StageDataWrapper { stages = runtimeData });
            PlayerPrefs.SetString(STAGE_DATA_KEY, json);
            PlayerPrefs.Save();
            Debug.Log("스테이지 런타임 데이터 저장 완료");
        }
        
        /// <summary>
        /// PlayerPrefs에서 런타임 데이터 로드
        /// </summary>
        public void LoadRuntimeData()
        {
            if (PlayerPrefs.HasKey(STAGE_DATA_KEY))
            {
                string json = PlayerPrefs.GetString(STAGE_DATA_KEY);
                var wrapper = JsonUtility.FromJson<StageDataWrapper>(json);
                runtimeData = wrapper.stages;
                Debug.Log("스테이지 런타임 데이터 로드 완료");
            }
            else
            {
                Debug.Log("저장된 스테이지 데이터가 없어서 초기값 사용");
            }
        }
        
        /// <summary>
        /// 스테이지 해금
        /// </summary>
        public void UnlockStage(int stageID)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null && !stage.isUnlocked)
            {
                stage.isUnlocked = true;
                SaveRuntimeData();
                Debug.Log($"스테이지 {stageID} 해금 완료");
            }
        }
        
        /// <summary>
        /// 서브 스테이지 해금
        /// </summary>
        public void UnlockSubStage(int stageID, int subStageID)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null)
            {
                var subStage = stage.subStages.Find(s => s.subStageID == subStageID);
                if (subStage != null && !subStage.isUnlocked)
                {
                    subStage.isUnlocked = true;
                    SaveRuntimeData();
                    Debug.Log($"서브 스테이지 {stageID}-{subStageID} 해금 완료");
                }
            }
        }
        
        /// <summary>
        /// 스테이지 점수 업데이트
        /// </summary>
        public void UpdateStageScore(int stageID, int subStageID, int score)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null)
            {
                var subStage = stage.subStages.Find(s => s.subStageID == subStageID);
                if (subStage != null && score > subStage.bestScore)
                {
                    subStage.bestScore = score;
                    SaveRuntimeData();
                    Debug.Log($"스테이지 {stageID}-{subStageID} 최고 점수 업데이트: {score}");
                }
            }
        }
        
        /// <summary>
        /// 스테이지 완료 처리
        /// </summary>
        public void CompleteStage(int stageID, int subStageID, float completionTime)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null)
            {
                var subStage = stage.subStages.Find(s => s.subStageID == subStageID);
                if (subStage != null)
                {
                    subStage.isCompleted = true;
                    subStage.completionTime = completionTime;
                    
                    // 모든 서브 스테이지가 완료되면 메인 스테이지도 완료
                    bool allSubStagesCompleted = stage.subStages.TrueForAll(s => s.isCompleted);
                    if (allSubStagesCompleted)
                    {
                        stage.isCompleted = true;
                    }
                    
                    SaveRuntimeData();
                    Debug.Log($"스테이지 {stageID}-{subStageID} 완료 처리");
                }
            }
        }
        
        /// <summary>
        /// 스테이지 해금 상태 확인
        /// </summary>
        public bool IsStageUnlocked(int stageID)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            return stage?.isUnlocked ?? false;
        }
        
        /// <summary>
        /// 서브 스테이지 해금 상태 확인
        /// </summary>
        public bool IsSubStageUnlocked(int stageID, int subStageID)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null)
            {
                var subStage = stage.subStages.Find(s => s.subStageID == subStageID);
                return subStage?.isUnlocked ?? false;
            }
            return false;
        }
        
        /// <summary>
        /// 스테이지 점수 가져오기
        /// </summary>
        public int GetStageScore(int stageID, int subStageID)
        {
            var stage = runtimeData.Find(s => s.stageID == stageID);
            if (stage != null)
            {
                var subStage = stage.subStages.Find(s => s.subStageID == subStageID);
                return subStage?.bestScore ?? 0;
            }
            return 0;
        }
    }
    
    // JSON 직렬화를 위한 래퍼 클래스
    [System.Serializable]
    public class StageDataWrapper
    {
        public List<StageRuntimeData> stages;
    }
} 