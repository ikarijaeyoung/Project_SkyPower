using UnityEngine;
using System.Collections.Generic;

namespace YSK
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "YSK/Scene Data")]
    public class SceneData : ScriptableObject
    {
        [System.Serializable]
        public class SceneInfo
        {
            [Header("씬 이름 (Build Settings와 동일하게!)")]
            [Tooltip("Unity Build Settings의 씬 이름 (예: MainMenu, RnDBaseStageScene 등)")]
            public string sceneName;

            [Header("표시용 이름/설명 (선택)")]
            public string displayName;
            [TextArea(2, 4)]
            public string description;

            [Header("로딩 설정")]
            [Tooltip("이 씬에서 로딩 화면을 표시할지 여부")]
            public bool requiresLoadingScreen = true;

            [Tooltip("최소 로딩 시간 (초)")]
            [Range(0.1f, 5f)]
            public float minLoadingTime = 1f;
        }

        [Header("씬 목록")]
        public List<SceneInfo> scenes = new List<SceneInfo>();

        // 씬 이름으로 정보 반환
        public SceneInfo GetSceneInfo(string sceneName)
        {
            return scenes.Find(s => s.sceneName == sceneName);
        }

        // 씬 존재 여부
        public bool HasScene(string sceneName)
        {
            return scenes.Exists(s => s.sceneName == sceneName);
        }
    }
} 