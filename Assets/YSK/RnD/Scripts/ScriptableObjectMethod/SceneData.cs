using UnityEngine;
using System.Collections.Generic;
using YSK;

namespace YSK
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "YSK/Scene Data")]
    public class SceneData : ScriptableObject
    {
        // UI 메서드 열거형
        public enum UIMethodType
        {
            None,                   // UI 생성 안함
            CreateMainMenuUI,       // 메인 메뉴 UI
            CreateBaseStageUI,      // 게임 스테이지 UI
            CreateEndlessStageUI,   // 무한 스테이지 UI
            CreateStoreUI,          // 상점 UI
            CreatePartyUI,          // 파티 UI
            CreateMainStageSelectUI, // 메인 스테이지 선택 UI
            CreateSubStageSelectUI,  // 서브 스테이지 선택 UI
            CreatePopupUI,          // 팝업 UI
            CreateResultUI,         // 결과 UI
            CreateLoadingUI,        // 로딩 UI
            CreateSettingsUI,       // 설정 UI
            CreatePauseUI,          // 일시정지 UI
            CreateGameOverUI        // 게임오버 UI
        }
        
        [System.Serializable]
        public class SceneInfo
        {
            [Header("Scene Info")]
            [Tooltip("Unity Build Settings의 씬 이름")]
            public string sceneName;
            
            [Tooltip("게임 내에서 표시될 씬 이름")]
            public string displayName;
            
            [Header("Scene Settings")]
            [Tooltip("Additive 로딩 여부")]
            public bool isAdditive = false;
            
            [Tooltip("로딩 화면 표시 여부")]
            public bool requiresLoadingScreen = true;
            
            [Tooltip("최소 로딩 시간 (초)")]
            [Range(0.1f, 5f)]
            public float minLoadingTime = 1f;
            
            [Header("Description")]
            [TextArea(2, 4)]
            public string description = "";
        }
        
        [Header("Scene List")]
        public List<SceneInfo> scenes = new List<SceneInfo>();
        
        // 기본 메서드들
        public SceneInfo GetSceneInfo(string sceneName)
        {
            return scenes.Find(s => s.sceneName == sceneName);
        }
        
        public bool HasScene(string sceneName)
        {
            return scenes.Exists(s => s.sceneName == sceneName);
        }
        
        public void AddScene(SceneInfo newScene)
        {
            if (!HasScene(newScene.sceneName))
            {
                scenes.Add(newScene);
                Debug.Log($"새 씬 추가됨: {newScene.sceneName} ({newScene.displayName})");
            }
            else
            {
                Debug.LogWarning($"씬 '{newScene.sceneName}'이 이미 존재합니다!");
            }
        }
        
        public bool RemoveScene(string sceneName)
        {
            SceneInfo sceneToRemove = scenes.Find(s => s.sceneName == sceneName);
            if (sceneToRemove != null)
            {
                scenes.Remove(sceneToRemove);
                Debug.Log($"씬 제거됨: {sceneName}");
                return true;
            }
            else
            {
                Debug.LogWarning($"씬 '{sceneName}'을 찾을 수 없습니다!");
                return false;
            }
        }
    }
} 