using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "SceneChangerManagerSO", menuName = "Manager/SceneChangerManager")]
    public class SceneChangerManagerSO : SOSingleton<SceneChangerManagerSO>
    {
        public override void Init()
        {
            // 후에 필요시
        }
        public void LoadScene(string name) // 지정한 이름의 씬 로드
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("[SceneChangerManagerSO] 씬 이름이 비어있습니다.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(name))
            {
                Debug.LogError($"[SceneTransitionManagerSO] '{name}' 씬을 Build Settings에서 찾을 수 없습니다.");
                return;
            }
            SceneManager.LoadScene(name);
        }

        public void ReloadCurrentScene() // 현재 씬 새로고침
        {
            var current = SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(current))
            {
                Debug.LogError("[SceneTransitionManagerSO] 현재 씬 이름을 가져오지 못했습니다.");
                return;
            }
            SceneManager.LoadScene(current);
        }
    }
}
