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
        //[SerializeField] private AudioManagerRunner audioManagerRunner;
        [SerializeField] private AudioManager audioManagerSO;
        
        [Header("Test Controls")]
        [SerializeField] private KeyCode testBGMKey = KeyCode.B;
        [SerializeField] private KeyCode testSFXKey = KeyCode.S;
        [SerializeField] private KeyCode randomBGMKey = KeyCode.R;
        [SerializeField] private KeyCode nextBGMKey = KeyCode.N;
        [SerializeField] private KeyCode stopAllKey = KeyCode.X;
        [SerializeField] private KeyCode muteKey = KeyCode.M;
        [SerializeField] private KeyCode CheckKey = KeyCode.Z;
   
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private bool isMuted = false;
        private int currentBGMIndex = 0;
        private List<AudioData> bgmList = new List<AudioData>();
        
        void Start()
        {
            FindAudioComponents();
            //LoadSoundList();
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
            // AudioManagerRunner 제거 (KYG 시스템만 사용)
            // if (audioManagerRunner == null)
            // {
            //     audioManagerRunner = FindObjectOfType<AudioManagerRunner>();
            // }
            
            // AudioManagerSO 찾기
            if (audioManagerSO == null)
            {
                audioManagerSO = FindObjectOfType<AudioManager>();
                if (audioManagerSO == null)
                {
                    audioManagerSO = Resources.Load<AudioManager>("AudioManagerSO");
                }
            }
            
            //// KYG 시스템 초기화 확인
            //if (KYG_skyPower.AudioManageO.Instance == null)
            //{
            //    Debug.LogError("KYG AudioManagerSO.Instance가 null입니다!");
            //}
            //else
            //{
            //    Debug.Log($"KYG AudioManagerSO 초기화 완료: {KYG_skyPower.AudioManager.Instance.name}");
                
            //    // AudioManagerSO 초기화
            //    KYG_skyPower.AudioManager.Instance.Init();
                
            //    // BGMSource 확인
            //    GameObject bgmSource = GameObject.Find("BGMSource");
            //    if (bgmSource != null)
            //    {
            //        Debug.Log($"BGMSource 찾음: {bgmSource.name}");
            //    }
            //    else
            //    {
            //        Debug.LogWarning("BGMSource를 찾을 수 없습니다!");
            //    }
            //}
        }
        
        //private void LoadSoundList()
        //{
        //    if (audioManagerSO?.audioDB?.audioList != null)
        //    {
        //        bgmList = audioManagerSO.audioDB.audioList.FindAll(a => a != null);
        //        Debug.Log($"총 {bgmList.Count}개의 BGM을 찾았습니다.");
                
        //        // 각 BGM 데이터 상세 확인
        //        for (int i = 0; i < bgmList.Count; i++)
        //        {
        //            var bgm = bgmList[i];
        //            Debug.Log($"BGM {i + 1}: {bgm.clipName} - 클립: {bgm.clipSource?.name ?? "없음"} - 볼륨: {bgm.volume}");
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("audioManagerSO 또는 audioDB가 null입니다!");
        //    }
        //}



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

            if (Input.GetKeyDown(CheckKey))
            {
                //LoadSoundList();
            }
        }
        
        #endregion
        
        #region Audio Testing
        
        private void TestBGM()
        {
            Debug.Log("=== BGM 테스트 시작 ===");
            
            // KYG 시스템 확인
            if (KYG_skyPower.AudioManager.Instance == null)
            {
                Debug.LogError("AudioManagerSO.Instance가 null입니다!");
                return;
            }
            
            Debug.Log($"AudioManagerSO 찾음: {KYG_skyPower.AudioManager.Instance.name}");
            
            // BGM 리스트 확인
            if (bgmList.Count == 0)
            {
                Debug.LogWarning("재생 가능한 BGM이 없습니다!");
                return;
            }
            
            // 첫 번째 BGM 재생
            var firstBGM = bgmList[0];
            if (firstBGM?.clipSource == null)
            {
                Debug.LogError("첫 번째 BGM의 clipSource가 null입니다!");
                return;
            }
            
            //Debug.Log($"BGM 재생 시도: {firstBGM.clipName}");
            //KYG_skyPower.AudioManager.Sound.PlayBGM(firstBGM.clipName);
            
            Debug.Log("=== BGM 테스트 완료 ===");
        }
        
        private void TestSFX()
        {
            Debug.Log("=== SFX 테스트 시작 ===");
            
            //if (KYG_skyPower.AudioManager.Instance != null)
            //{
            //    // SFX 재생
            //    KYG_skyPower.AudioManager.Sound.PlaySFXOneShot("Click");
            //    Debug.Log("SFX 재생: Click");
            //}
            //else
            //{
            //    Debug.LogError("AudioManagerSO.Instance가 null입니다!");
            //}
        }
        
        private void StopAllAudio()
        {
            //// KYG 시스템 사용
            //if (KYG_skyPower.AudioManager.Instance != null)
            //{
            //    KYG_skyPower.AudioManager.Sound.StopBGM();
            //    Debug.Log("모든 오디오 정지");
            //}
            //else
            //{
            //    Debug.LogError("AudioManagerSO.Instance가 null입니다!");
            //}
        }
        
        private void ToggleMute()
        {
            isMuted = !isMuted;
            
            // KYG 시스템의 BGMSource에 직접 접근하여 뮤트 설정
            if (KYG_skyPower.AudioManager.Instance != null)
            {
                // BGMSource 찾기
                GameObject bgmSource = GameObject.Find("BGMSource");
                if (bgmSource != null)
                {
                    AudioSource bgmAudioSource = bgmSource.GetComponent<AudioSource>();
                    if (bgmAudioSource != null)
                    {
                        bgmAudioSource.mute = isMuted;
                    }
                }
                
                // SFXPool 찾기
                GameObject sfxPool = GameObject.Find("SFXPool");
                if (sfxPool != null)
                {
                    AudioSource[] sfxSources = sfxPool.GetComponents<AudioSource>();
                    foreach (var source in sfxSources)
                    {
                        source.mute = isMuted;
                    }
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
            if (bgmData?.clipSource == null)
            {
                Debug.LogWarning("BGM 데이터가 없습니다!");
                return;
            }
            
            //// KYG 시스템 사용 (AudioManagerRunner 제거)
            //if (KYG_skyPower.AudioManager.Instance != null)
            //{
            //    KYG_skyPower.AudioManager.Sound.PlayBGM(bgmData.clipName);
            //    Debug.Log($"BGM 재생: {bgmData.clipName} (인덱스: {currentBGMIndex + 1}/{bgmList.Count})");
            //}
            //else
            //{
            //    Debug.LogError("AudioManagerSO.Instance가 null입니다!");
            //}
        }
        
        #endregion
        
        #region Debug Info
        
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 400, 250));
            GUILayout.Label("=== 간단한 오디오 테스트 (KYG 시스템) ===");
            GUILayout.Label($"BGM 개수: {bgmList.Count}");
            GUILayout.Label($"현재 BGM: {currentBGMIndex + 1}/{bgmList.Count}");
            
            // KYG 시스템의 BGMSource 상태 확인
            GameObject bgmSource = GameObject.Find("BGMSource");
            if (bgmSource != null)
            {
                AudioSource bgmAudioSource = bgmSource.GetComponent<AudioSource>();
                if (bgmAudioSource != null)
                {
                    GUILayout.Label($"BGM 재생 중: {bgmAudioSource.isPlaying}");
                    GUILayout.Label($"BGM 클립: {bgmAudioSource.clip?.name ?? "없음"}");
                    GUILayout.Label($"BGM 볼륨: {bgmAudioSource.volume:F2}");
                    GUILayout.Label($"BGM 뮤트: {bgmAudioSource.mute}");
                }
            }
            else
            {
                GUILayout.Label("BGMSource를 찾을 수 없습니다!");
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