using UnityEngine;
using YSK;

namespace YSK
{
    /// <summary>
    /// AudioTestManager 사용 예제
    /// </summary>
    public class AudioTestExample : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool createAudioTestManagerOnStart = true;
        [SerializeField] private bool useCustomUI = false;
        
        [Header("Custom UI References")]
        [SerializeField] private Transform customButtonContainer;
        [SerializeField] private UnityEngine.UI.Slider customMasterVolume;
        [SerializeField] private UnityEngine.UI.Slider customBGMVolume;
        [SerializeField] private UnityEngine.UI.Slider customSFXVolume;
        [SerializeField] private UnityEngine.UI.Toggle customMuteToggle;
        [SerializeField] private TMPro.TextMeshProUGUI customDebugText;
        
        private AudioTestManager audioTestManager;
        
        void Start()
        {
            if (createAudioTestManagerOnStart)
            {
                CreateAudioTestManager();
            }
        }
        
        void Update()
        {
            HandleTestInput();
        }
        
        #region AudioTestManager Creation
        
        private void CreateAudioTestManager()
        {
            Debug.Log("=== AudioTestManager 생성 시작 ===");
            
            // AudioTestManager GameObject 생성
            GameObject managerObj = new GameObject("AudioTestManager");
            audioTestManager = managerObj.AddComponent<AudioTestManager>();
            
            // 커스텀 UI 사용 여부에 따라 설정
            if (useCustomUI)
            {
                SetupCustomUI();
            }
            
            Debug.Log("=== AudioTestManager 생성 완료 ===");
        }
        
        private void SetupCustomUI()
        {
            if (audioTestManager == null) return;
            
            Debug.Log("커스텀 UI 설정 중...");
            
            // 커스텀 UI 컴포넌트 연결
            if (customButtonContainer != null)
            {
                // 리플렉션을 통해 private 필드에 접근
                var buttonContainerField = typeof(AudioTestManager).GetField("buttonContainer", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                buttonContainerField?.SetValue(audioTestManager, customButtonContainer);
            }
            
            if (customMasterVolume != null)
            {
                var masterVolumeField = typeof(AudioTestManager).GetField("masterVolumeSlider", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                masterVolumeField?.SetValue(audioTestManager, customMasterVolume);
            }
            
            if (customBGMVolume != null)
            {
                var bgmVolumeField = typeof(AudioTestManager).GetField("bgmVolumeSlider", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                bgmVolumeField?.SetValue(audioTestManager, customBGMVolume);
            }
            
            if (customSFXVolume != null)
            {
                var sfxVolumeField = typeof(AudioTestManager).GetField("sfxVolumeSlider", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                sfxVolumeField?.SetValue(audioTestManager, customSFXVolume);
            }
            
            if (customMuteToggle != null)
            {
                var muteToggleField = typeof(AudioTestManager).GetField("muteToggle", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                muteToggleField?.SetValue(audioTestManager, customMuteToggle);
            }
            
            if (customDebugText != null)
            {
                var debugTextField = typeof(AudioTestManager).GetField("debugText", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                debugTextField?.SetValue(audioTestManager, customDebugText);
            }
            
            Debug.Log("커스텀 UI 설정 완료");
        }
        
        #endregion
        
        #region Test Input Handling
        
        private void HandleTestInput()
        {
            // R: 오디오 리스트 새로고침
            if (Input.GetKeyDown(KeyCode.R))
            {
                RefreshAudioList();
            }
            
            // F: 오디오 테스트 매니저 재생성
            if (Input.GetKeyDown(KeyCode.F))
            {
                RecreateAudioTestManager();
            }
            
            // V: 볼륨 테스트
            if (Input.GetKeyDown(KeyCode.V))
            {
                TestVolumeControls();
            }
            
            // B: BGM 테스트
            if (Input.GetKeyDown(KeyCode.B))
            {
                TestBGMControls();
            }
        }
        
        private void RefreshAudioList()
        {
            if (audioTestManager != null)
            {
                audioTestManager.RefreshAudioList();
                Debug.Log("오디오 리스트 새로고침 완료");
            }
            else
            {
                Debug.LogWarning("AudioTestManager가 null입니다!");
            }
        }
        
        private void RecreateAudioTestManager()
        {
            Debug.Log("AudioTestManager 재생성 중...");
            
            // 기존 매니저 제거
            if (audioTestManager != null)
            {
                DestroyImmediate(audioTestManager.gameObject);
                audioTestManager = null;
            }
            
            // 새로 생성
            CreateAudioTestManager();
        }
        
        private void TestVolumeControls()
        {
            if (audioTestManager == null) return;
            
            Debug.Log("=== 볼륨 컨트롤 테스트 ===");
            
            // 볼륨 설정 테스트
            audioTestManager.SetMasterVolume(0.8f);
            audioTestManager.SetBGMVolume(0.6f);
            audioTestManager.SetSFXVolume(1.0f);
            
            Debug.Log("볼륨 설정 완료: 마스터=0.8, BGM=0.6, SFX=1.0");
            
            // 2초 후 원래대로 복원
            StartCoroutine(RestoreVolumeAfterDelay(2f));
        }
        
        private System.Collections.IEnumerator RestoreVolumeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (audioTestManager != null)
            {
                audioTestManager.SetMasterVolume(1.0f);
                audioTestManager.SetBGMVolume(1.0f);
                audioTestManager.SetSFXVolume(1.0f);
                Debug.Log("볼륨 원래대로 복원 완료");
            }
        }
        
        private void TestBGMControls()
        {
            if (audioTestManager == null) return;
            
            Debug.Log("=== BGM 컨트롤 테스트 ===");
            
            // 뮤트 토글 테스트
            audioTestManager.SetMute(true);
            Debug.Log("뮤트 켜짐");
            
            // 1초 후 뮤트 해제
            StartCoroutine(UnmuteAfterDelay(1f));
        }
        
        private System.Collections.IEnumerator UnmuteAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (audioTestManager != null)
            {
                audioTestManager.SetMute(false);
                Debug.Log("뮤트 해제됨");
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// 오디오 테스트 매니저 수동 생성
        /// </summary>
        [ContextMenu("Create AudioTestManager")]
        public void CreateAudioTestManagerManual()
        {
            CreateAudioTestManager();
        }
        
        /// <summary>
        /// 오디오 리스트 수동 새로고침
        /// </summary>
        [ContextMenu("Refresh Audio List")]
        public void RefreshAudioListManual()
        {
            RefreshAudioList();
        }
        
        /// <summary>
        /// 볼륨 테스트 실행
        /// </summary>
        [ContextMenu("Test Volume Controls")]
        public void TestVolumeControlsManual()
        {
            TestVolumeControls();
        }
        
        /// <summary>
        /// BGM 테스트 실행
        /// </summary>
        [ContextMenu("Test BGM Controls")]
        public void TestBGMControlsManual()
        {
            TestBGMControls();
        }
        
        #endregion
        
        #region Debug Info
        
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, Screen.height - 200, 300, 190));
            GUILayout.Label("=== AudioTestManager 예제 ===");
            
            if (audioTestManager != null)
            {
                GUILayout.Label("✅ AudioTestManager 활성화됨");
            }
            else
            {
                GUILayout.Label("❌ AudioTestManager 비활성화");
            }
            
            GUILayout.Space(10);
            GUILayout.Label("키보드 단축키:");
            GUILayout.Label("R: 오디오 리스트 새로고침");
            GUILayout.Label("F: 매니저 재생성");
            GUILayout.Label("V: 볼륨 테스트");
            GUILayout.Label("B: BGM 테스트");
            GUILayout.Label("M: 뮤트 토글 (AudioTestManager)");
            GUILayout.Label("S: 모든 사운드 정지");
            GUILayout.Label("T: 랜덤 SFX 테스트");
            
            GUILayout.EndArea();
        }
        
        #endregion
    }
} 