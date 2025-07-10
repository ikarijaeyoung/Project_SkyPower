using KYG_skyPower;
using System.Collections.Generic;
using UnityEngine;

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
        public StageEnemyData stageEnemyData;
    }

    public class StageDataManager : Singleton<StageDataManager>
    {
        [Header("Stage Data")]
        [SerializeField] private List<StageData> stageDataList = new List<StageData>();
        
        [Header("Runtime Data")]
        [SerializeField] public List<StageRuntimeData> runtimeData = new List<StageRuntimeData>();
        
        // JSON 저장 키 제거
        // private const string STAGE_DATA_KEY = "StageRuntimeData"; // 삭제
        
        public static System.Action OnStageDataChanged;
        
        protected override void Awake() => base.Awake();
        
        private void Start() { Init(); }
        
        public override void Init()
        {
            InitializeStageDataList();
            InitializeRuntimeData();
            
            // JSON 로드 제거 - 런타임에서만 관리
            // LoadRuntimeData(); // 삭제
            
            // 초기화 완료 후 이벤트 발생
            OnStageDataChanged?.Invoke();
        }
        
        /// <summary>
        /// StageData 스크립터블 오브젝트들을 자동으로 찾아서 설정
        /// </summary>

        public void SyncRuntimeDataWithStageInfo()
        {
            GameData saveData = Manager.Game.CurrentSave;
            for (int i = 0; i < saveData.stageInfo.Length; i++)
            {
                if(i>Manager.SDM.runtimeData.Count*5-1)
                {
                    Debug.Log("runtimeData 이상의 맵 데이터 세이브 정보가 있음");
                    return;
                }
                int worldIndex = i / 5;
                int stageIndex = i % 5;
                //Debug.Log($"{i}  {worldIndex} {stageIndex}");
                runtimeData[worldIndex].subStages[stageIndex].bestScore = saveData.stageInfo[i].score;
                runtimeData[worldIndex].subStages[stageIndex].isUnlocked = saveData.stageInfo[i].unlock;
                runtimeData[worldIndex].subStages[stageIndex].isCompleted = saveData.stageInfo[i].isClear;
            }
        }


        private void InitializeStageDataList()
        {
            if (stageDataList == null || stageDataList.Count == 0)
            {
                // Resources 폴더에서 StageData 스크립터블 오브젝트들을 찾기
                StageData[] foundStageData = Resources.FindObjectsOfTypeAll<StageData>();

                if (foundStageData.Length > 0)
                {
                    stageDataList = new List<StageData>(foundStageData);
                    stageDataList.Sort((a, b) => a.stageID.CompareTo(b.stageID)); // ID 순으로 정렬
                    Debug.Log($"자동으로 {stageDataList.Count}개의 StageData를 찾았습니다.");
                }
                else
                {
                    Debug.LogWarning("StageData 스크립터블 오브젝트를 찾을 수 없습니다! Resources 폴더에 StageData 파일들이 있는지 확인해주세요.");
                }
            }
        }

        private void InitializeRuntimeData()
        {
            //runtimeData.Clear();

            if (stageDataList == null || stageDataList.Count == 0)
            {
                Debug.LogWarning("StageDataList가 비어있어서 런타임 데이터를 초기화할 수 없습니다.");
                return;
            }

            foreach (var stageData in stageDataList)
            {
                var runtimeStage = new StageRuntimeData
                {
                    stageID = stageData.stageID,
                    // 1-1 스테이지만 기본 해금, 나머지는 잠금
                    isUnlocked = (stageData.stageID == 1),
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
                        // 1-1 스테이지만 기본 해금, 나머지는 잠금
                        isUnlocked = (stageData.stageID == 1 && subStageData.subStageID == 1),
                        bestScore = subStageData.subStageScore,
                        isCompleted = false,
                        completionTime = 0f,
                        stageEnemyData = subStageData.stageEnemyData
                    };

                    runtimeStage.subStages.Add(runtimeSubStage);
                }

                runtimeData.Add(runtimeStage);
            }

            Debug.Log($"런타임 데이터 초기화 완료: {runtimeData.Count}개 스테이지");
        }

        /// <summary>
        /// 런타임 데이터를 PlayerPrefs에 저장 (JSON 제거)
        /// </summary>
        public void SaveRuntimeData()
        {
            // JSON 저장 제거 - 런타임에서만 관리하므로 저장 불필요
            // string json = JsonUtility.ToJson(new StageDataWrapper { stages = runtimeData });
            // PlayerPrefs.SetString(STAGE_DATA_KEY, json);
            // PlayerPrefs.Save();
            
            Debug.Log("런타임 데이터는 메모리에서만 관리됩니다 (JSON 저장 제거)");
        }
        
        /// <summary>
        /// PlayerPrefs에서 런타임 데이터 로드 (JSON 제거)
        /// </summary>
        public void LoadRuntimeData()
        {
            // JSON 로드 제거 - 항상 초기값 사용
            // if (PlayerPrefs.HasKey(STAGE_DATA_KEY))
            // {
            //     string json = PlayerPrefs.GetString(STAGE_DATA_KEY);
            //     var wrapper = JsonUtility.FromJson<StageDataWrapper>(json);
            //     SyncRuntimeDataWithStageData(wrapper.stages);
            // }
            
            Debug.Log("런타임 데이터는 메모리에서만 관리됩니다 (JSON 로드 제거)");
        }

        /// <summary>
        /// 저장된 런타임 데이터와 현재 StageDataList를 동기화
        /// </summary>
        private void SyncRuntimeDataWithStageData(List<StageRuntimeData> savedData)
        {
            if (savedData == null)
            {
                Debug.LogWarning("저장된 데이터가 null입니다.");
                return;
            }

            // 기존 런타임 데이터를 저장된 데이터로 교체
            runtimeData = new List<StageRuntimeData>();

            foreach (var stageData in stageDataList)
            {
                // 저장된 데이터에서 해당 스테이지 찾기
                var savedStage = savedData.Find(s => s.stageID == stageData.stageID);

                if (savedStage != null)
                {
                    // 저장된 데이터 사용
                    var runtimeStage = new StageRuntimeData
                    {
                        stageID = savedStage.stageID,
                        isUnlocked = savedStage.isUnlocked,
                        bestScore = savedStage.bestScore,
                        isCompleted = savedStage.isCompleted,
                        completionTime = savedStage.completionTime,
                        subStages = new List<SubStageRuntimeData>()
                    };

                    // 서브 스테이지 동기화
                    foreach (var subStageData in stageData.subStages)
                    {
                        var savedSubStage = savedStage.subStages.Find(s => s.subStageID == subStageData.subStageID);

                        if (savedSubStage != null)
                        {
                            runtimeStage.subStages.Add(savedSubStage);
                        }
                        else
                        {
                            // 새로운 서브 스테이지 추가
                            var newSubStage = new SubStageRuntimeData
                            {
                                subStageID = subStageData.subStageID,
                                isUnlocked = (stageData.stageID == 1 && subStageData.subStageID == 1),
                                bestScore = subStageData.subStageScore,
                                isCompleted = false,
                                completionTime = 0f
                            };
                            runtimeStage.subStages.Add(newSubStage);
                        }
                    }

                    runtimeData.Add(runtimeStage);
                }
                else
                {
                    // 새로운 스테이지 추가
                    var newStage = new StageRuntimeData
                    {
                        stageID = stageData.stageID,
                        isUnlocked = (stageData.stageID == 1),
                        bestScore = 0,
                        isCompleted = false,
                        completionTime = 0f,
                        subStages = new List<SubStageRuntimeData>()
                    };

                    foreach (var subStageData in stageData.subStages)
                    {
                        var newSubStage = new SubStageRuntimeData
                        {
                            subStageID = subStageData.subStageID,
                            isUnlocked = (stageData.stageID == 1 && subStageData.subStageID == 1),
                            bestScore = subStageData.subStageScore,
                            isCompleted = false,
                            completionTime = 0f
                        };
                        newStage.subStages.Add(newSubStage);
                    }

                    runtimeData.Add(newStage);
                }
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
                // SaveRuntimeData(); // 제거
                OnStageDataChanged?.Invoke(); // UI 업데이트 이벤트 발생
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
                    // SaveRuntimeData(); // 제거
                    OnStageDataChanged?.Invoke(); // UI 업데이트 이벤트 발생
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
                    // SaveRuntimeData(); // 제거
                    OnStageDataChanged?.Invoke(); // UI 업데이트 이벤트 발생
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

                    // SaveRuntimeData(); // 제거
                    OnStageDataChanged?.Invoke(); // UI 업데이트 이벤트 발생
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

        /// <summary>
        /// 전체 스테이지 데이터 리스트 가져오기
        /// </summary>
        public List<StageData> GetStageDataList()
        {
            return stageDataList;
        }

        /// <summary>
        /// 특정 스테이지 데이터 가져오기
        /// </summary>
        public StageData GetStageData(int stageID)
        {
            return stageDataList?.Find(s => s.stageID == stageID);
        }

        /// <summary>
        /// 런타임 데이터 초기화 (테스트용)
        /// </summary>
        public void ResetAllProgress()
        {
            // PlayerPrefs.DeleteKey(STAGE_DATA_KEY); // 삭제
            InitializeRuntimeData();
            OnStageDataChanged?.Invoke();
            Debug.Log("모든 스테이지 진행도가 초기화되었습니다.");
        }

        /// <summary>
        /// RuntimeData를 세이브 파일과 완전히 동기화하고 저장 (JSON 제거)
        /// </summary>
        public void SyncAndSaveRuntimeData()
        {
            if (Manager.Game != null && Manager.Game.saveFiles != null && 
                Manager.Game.currentSaveIndex >= 0 && 
                Manager.Game.currentSaveIndex < Manager.Game.saveFiles.Length)
            {
                GameData saveData = Manager.Game.CurrentSave;
                
                // RuntimeData를 세이브 파일과 동기화
                SyncRuntimeDataWithStageInfo();
                
                // JSON 저장 제거 - 런타임에서만 관리
                // SaveRuntimeData();
                
                Debug.Log("RuntimeData 동기화 완료 (JSON 저장 제거)");
            }
            else
            {
                Debug.LogWarning("Manager.Game 또는 세이브 파일이 유효하지 않습니다!");
            }
        }

        /// <summary>
        /// 스테이지 완료 시 모든 데이터를 동기화하고 저장 (JSON 제거)
        /// </summary>
        public void CompleteStageWithSave(int stageID, int subStageID, int score, float completionTime)
        {
            // 1. 점수 업데이트
            //UpdateStageScore(stageID, subStageID, score);
            
            // 2. 스테이지 완료 처리
            CompleteStage(stageID, subStageID, completionTime);
            
            // 3. 다음 스테이지 언락
            var nextStage = CalculateNextStage(stageID, subStageID);
            if (!nextStage.isGameComplete)
            {
                UnlockStage(nextStage.mainStage);
                UnlockSubStage(nextStage.mainStage, nextStage.subStage);
            }
            
            // 4. UI 업데이트
            OnStageDataChanged?.Invoke();
            
            Debug.Log($"스테이지 {stageID}-{subStageID} 완료 처리 완료 (JSON 저장 제거)");
        }

        /// <summary>
        /// 다음 스테이지 계산
        /// </summary>
        private (int mainStage, int subStage, bool isGameComplete) CalculateNextStage(int currentMainStage, int currentSubStage)
        {
            int nextSubStage = currentSubStage + 1;
            
            if (nextSubStage > 5) // 최대 서브스테이지 수
            {
                int nextMainStage = currentMainStage + 1;
                
                if (nextMainStage > 4) // 최대 메인스테이지 수
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


#if UNITY_EDITOR
        /// <summary>
        /// 인스펙터에서 값이 변경될 때 호출
        /// </summary>
        private void OnValidate()
        {
            // 런타임 중에만 실행
            if (Application.isPlaying && runtimeData != null && runtimeData.Count > 0)
            {
                // 약간의 지연을 두어 인스펙터 변경이 완료된 후 이벤트 발생
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    OnStageDataChanged?.Invoke();
                    Debug.Log("StageDataManager 인스펙터 Runtime Data 변경 감지 - UI 업데이트");
                };
            }
        }

        /// <summary>
        /// 디버그용 - 현재 스테이지 상태 출력
        /// </summary>
        public void DebugStageStatus()
        {
            Debug.Log("=== 스테이지 상태 디버그 ===");
            foreach (var stage in runtimeData)
            {
                Debug.Log($"스테이지 {stage.stageID}: 해금={stage.isUnlocked}, 완료={stage.isCompleted}, 점수={stage.bestScore}");
                foreach (var subStage in stage.subStages)
                {
                    Debug.Log($"  서브스테이지 {subStage.subStageID}: 해금={subStage.isUnlocked}, 완료={subStage.isCompleted}, 점수={subStage.bestScore}");
                }
            }
        }
#endif
    }

    // JSON 직렬화 래퍼 클래스 제거
    // [System.Serializable]
    // public class StageDataWrapper
    // {
    //     public List<StageRuntimeData> stages;
    // }
}