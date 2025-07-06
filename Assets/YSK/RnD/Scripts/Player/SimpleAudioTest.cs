using UnityEngine;
using KYG_skyPower;
using System.Collections.Generic;

namespace YSK
{
    /// <summary>
    /// 간단한 오디오 테스트 시스템
    /// </summary>
    public class SimpleAudioTest : MonoBehaviour
    {
        [Header("Audio Manager")]
        [SerializeField] private AudioManagerRunner audioManagerRunner;
        [SerializeField] private AudioManagerSO audioManagerSO;
        
        [Header("Test Controls")]
        [SerializeField] private KeyCode testBGMKey = KeyCode.B;
        [SerializeField] private KeyCode testSFXKey = KeyCode.S;
        [SerializeField] private KeyCode randomBGMKey = KeyCode.R;
        [SerializeField] private KeyCode nextBGMKey = KeyCode.N;
        [SerializeField] private KeyCode stopAllKey = KeyCode.X;
        [SerializeField] private KeyCode muteKey = KeyCode.M;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private bool isMuted = false;
        private int currentBGMIndex = 0;
        private List<AudioData> bgmList = new List<AudioData>();
        
        void Start()
        {
            FindAudioComponents();
            LoadBGMList();
            Debug.Log("=== 간단한 오디오 테스트 시작 ===");
            Debug.Log("B: 기본 BGM 테스트");
            Debug.Log("R: 랜덤 BGM 재생");
            Debug.Log("N: 다음 BGM 재생");
            Debug.Log("S: SFX 테스트");
            Debug.Log("X: 모든 사운드 정지");
            Debug.Log("M: 뮤트 토글");
        }
        
        void Update()
        {
            HandleInput();
        }
        
        #region Initialization
        
        private void FindAudioComponents()
        {
            // AudioManagerRunner 찾기
            if (audioManagerRunner == null)
            {
                audioManagerRunner = FindObjectOfType<AudioManagerRunner>();
            }
            
            // AudioManagerSO 찾기
            if (audioManagerSO == null)
            {
                audioManagerSO = FindObjectOfType<AudioManagerSO>();
                if (audioManagerSO == null)
                {
                    audioManagerSO = Resources.Load<AudioManagerSO>("AudioManagerSO");
                }
            }
            
            if (audioManagerRunner == null)
            {
                Debug.LogWarning("AudioManagerRunner를 찾을 수 없습니다!");
            }
            
            if (audioManagerSO == null)
            {
                Debug.LogWarning("AudioManagerSO를 찾을 수 없습니다!");
            }
        }
        
        private void LoadBGMList()
        {
            if (audioManagerSO?.audioDB?.audioList != null)
            {
                bgmList = audioManagerSO.audioDB.audioList.FindAll(a => a != null && a.loop);
                Debug.Log($"총 {bgmList.Count}개의 BGM을 찾았습니다.");
            }
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(testBGMKey))
            {
                TestBGM();
            }
            
            if (Input.GetKeyDown(randomBGMKey))
            {
                PlayRandomBGM();
            }
            
            if (Input.GetKeyDown(nextBGMKey))
            {
                PlayNextBGM();
            }
            
            if (Input.GetKeyDown(testSFXKey))
            {
                TestSFX();
            }
            
            if (Input.GetKeyDown(stopAllKey))
            {
                StopAllAudio();
            }
            
            if (Input.GetKeyDown(muteKey))
            {
                ToggleMute();
            }
        }
        
        #endregion
        
        #region Audio Testing
        
        private void TestBGM()
        {
            if (audioManagerSO?.defaultBGM != null)
            {
                PlayBGM(audioManagerSO.defaultBGM);
            }
        }
        
        private void TestSFX()
        {
            Debug.Log("=== SFX 테스트 시작 ===");
            
            if (audioManagerSO == null || audioManagerSO.audioDB == null || audioManagerSO.audioDB.audioList == null)
            {
                Debug.LogWarning("SFX를 찾을 수 없습니다!");
                return;
            }
            
            // 첫 번째 SFX 찾기 (루프가 아닌 것)
            AudioData sfxData = audioManagerSO.audioDB.audioList.Find(a => a != null && !a.loop);
            
            if (sfxData == null)
            {
                Debug.LogWarning("재생 가능한 SFX를 찾을 수 없습니다!");
                return;
            }
            
            if (audioManagerRunner != null && audioManagerRunner.sfxSource != null)
            {
                // 기존 재생 중단
                audioManagerRunner.sfxSource.Stop();
                
                // 새로 재생
                audioManagerRunner.sfxSource.clip = sfxData.clipSource;
                audioManagerRunner.sfxSource.volume = sfxData.volume;
                audioManagerRunner.sfxSource.loop = sfxData.loop;
                audioManagerRunner.sfxSource.mute = isMuted;
                audioManagerRunner.sfxSource.Play();
                
                Debug.Log($"SFX 재생: {sfxData.clipName}");
            }
            else
            {
                Debug.LogWarning("SFX AudioSource를 찾을 수 없습니다!");
            }
        }
        
        private void StopAllAudio()
        {
            if (audioManagerRunner != null)
            {
                if (audioManagerRunner.bgmSource != null)
                {
                    audioManagerRunner.bgmSource.Stop();
                }
                if (audioManagerRunner.sfxSource != null)
                {
                    audioManagerRunner.sfxSource.Stop();
                }
            }
            
            Debug.Log("모든 오디오 정지");
        }
        
        private void ToggleMute()
        {
            isMuted = !isMuted;
            
            if (audioManagerRunner != null)
            {
                if (audioManagerRunner.bgmSource != null)
                {
                    audioManagerRunner.bgmSource.mute = isMuted;
                }
                if (audioManagerRunner.sfxSource != null)
                {
                    audioManagerRunner.sfxSource.mute = isMuted;
                }
            }
            
            Debug.Log($"뮤트: {(isMuted ? "켜짐" : "꺼짐")}");
        }
        
        private void PlayRandomBGM()
        {
            if (bgmList.Count == 0)
            {
                Debug.LogWarning("재생 가능한 BGM이 없습니다!");
                return;
            }
            
            int randomIndex = Random.Range(0, bgmList.Count);
            PlayBGM(bgmList[randomIndex]);
            currentBGMIndex = randomIndex;
        }

        private void PlayNextBGM()
        {
            if (bgmList.Count == 0)
            {
                Debug.LogWarning("재생 가능한 BGM이 없습니다!");
                return;
            }
            
            currentBGMIndex = (currentBGMIndex + 1) % bgmList.Count;
            PlayBGM(bgmList[currentBGMIndex]);
        }

        private void PlayBGM(AudioData bgmData)
        {
            if (audioManagerRunner?.bgmSource == null)
            {
                Debug.LogWarning("BGM AudioSource를 찾을 수 없습니다!");
                return;
            }
            
            if (bgmData?.clipSource == null)
            {
                Debug.LogWarning("BGM 데이터가 없습니다!");
                return;
            }
            
            // 기존 재생 중단
            audioManagerRunner.bgmSource.Stop();
            
            // 새 BGM 재생
            audioManagerRunner.bgmSource.clip = bgmData.clipSource;
            audioManagerRunner.bgmSource.volume = bgmData.volume;
            audioManagerRunner.bgmSource.loop = bgmData.loop;
            audioManagerRunner.bgmSource.mute = isMuted;
            audioManagerRunner.bgmSource.Play();
            
            Debug.Log($"BGM 재생: {bgmData.clipName} (인덱스: {currentBGMIndex + 1}/{bgmList.Count})");
        }
        
        #endregion
        
        #region Debug Info
        
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 400, 250));
            GUILayout.Label("=== 간단한 오디오 테스트 ===");
            GUILayout.Label($"BGM 개수: {bgmList.Count}");
            GUILayout.Label($"현재 BGM: {currentBGMIndex + 1}/{bgmList.Count}");
            
            if (audioManagerRunner?.bgmSource != null)
            {
                GUILayout.Label($"BGM 재생 중: {audioManagerRunner.bgmSource.isPlaying}");
                GUILayout.Label($"BGM 클립: {audioManagerRunner.bgmSource.clip?.name ?? "없음"}");
                GUILayout.Label($"BGM 볼륨: {audioManagerRunner.bgmSource.volume:F2}");
            }
            
            GUILayout.Space(10);
            GUILayout.Label("조작법:");
            GUILayout.Label("B: 기본 BGM");
            GUILayout.Label("R: 랜덤 BGM");
            GUILayout.Label("N: 다음 BGM");
            GUILayout.Label("S: SFX 테스트");
            GUILayout.Label("X: 모든 사운드 정지");
            GUILayout.Label("M: 뮤트 토글");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Public Methods
        
        [ContextMenu("BGM 테스트")]
        public void TestBGMManual()
        {
            TestBGM();
        }
        
        [ContextMenu("SFX 테스트")]
        public void TestSFXManual()
        {
            TestSFX();
        }
        
        [ContextMenu("모든 사운드 정지")]
        public void StopAllAudioManual()
        {
            StopAllAudio();
        }
        
        [ContextMenu("뮤트 토글")]
        public void ToggleMuteManual()
        {
            ToggleMute();
        }
        
        #endregion
    }
} 