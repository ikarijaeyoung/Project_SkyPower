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
            
            [Header("Scene Type")]
            [Tooltip("씬의 카테고리 분류")]
            public SceneCategory sceneCategory = SceneCategory.Game;
            
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
        
        public enum SceneCategory
        {
            Bootstrap,
            Menu,
            Game,
            UI,
            Test
        }
        
        [System.Serializable]
        public class CategoryDefaults
        {
            public bool isAdditive;
            public bool requiresLoadingScreen;
            public float minLoadingTime;
            public string description;
        }
        
        [Header("Category Default Settings")]
        public CategoryDefaults bootstrapDefaults = new CategoryDefaults
        {
            isAdditive = false,
            requiresLoadingScreen = false,
            minLoadingTime = 0.5f,
            description = "게임 초기화 씬"
        };
        
        public CategoryDefaults menuDefaults = new CategoryDefaults
        {
            isAdditive = false,
            requiresLoadingScreen = true,
            minLoadingTime = 1.0f,
            description = "메뉴 관련 씬"
        };
        
        public CategoryDefaults gameDefaults = new CategoryDefaults
        {
            isAdditive = false,
            requiresLoadingScreen = true,
            minLoadingTime = 1.5f,
            description = "게임플레이 씬"
        };
        
        public CategoryDefaults uiDefaults = new CategoryDefaults
        {
            isAdditive = false,
            requiresLoadingScreen = false,
            minLoadingTime = 0.3f,
            description = "UI 전용 씬"
        };
        
        public CategoryDefaults testDefaults = new CategoryDefaults
        {
            isAdditive = false,
            requiresLoadingScreen = false,
            minLoadingTime = 0.1f,
            description = "테스트용 씬"
        };
        
        [Header("Scene List")]
        public List<SceneInfo> scenes = new List<SceneInfo>();
        
        // 카테고리별 기본 설정을 가져오는 메서드
        public CategoryDefaults GetCategoryDefaults(SceneCategory category)
        {
            switch (category)
            {
                case SceneCategory.Bootstrap: return bootstrapDefaults;
                case SceneCategory.Menu: return menuDefaults;
                case SceneCategory.Game: return gameDefaults;
                case SceneCategory.UI: return uiDefaults;
                case SceneCategory.Test: return testDefaults;
                default: return gameDefaults;
            }
        }
        
        // 씬 정보를 가져올 때 카테고리 기본값 적용
        public SceneInfo GetSceneInfoWithDefaults(string sceneName)
        {
            SceneInfo sceneInfo = scenes.Find(s => s.sceneName == sceneName);
            if (sceneInfo != null)
            {
                ApplyCategoryDefaults(sceneInfo);
            }
            return sceneInfo;
        }
        
        private void ApplyCategoryDefaults(SceneInfo sceneInfo)
        {
            CategoryDefaults defaults = GetCategoryDefaults(sceneInfo.sceneCategory);
            
            // 다른 설정들도 필요에 따라 기본값 적용
            if (sceneInfo.minLoadingTime <= 0)
            {
                sceneInfo.minLoadingTime = defaults.minLoadingTime;
            }
        }
        
        // 기본 메서드들
        public SceneInfo GetSceneInfo(string sceneName)
        {
            return GetSceneInfoWithDefaults(sceneName);
        }
        
        public bool HasScene(string sceneName)
        {
            return scenes.Exists(s => s.sceneName == sceneName);
        }
        
        public List<SceneInfo> GetScenesByCategory(SceneCategory category)
        {
            return scenes.FindAll(s => s.sceneCategory == category);
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