using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using KYG_skyPower;
using TMPro;

namespace YSK
{
    /// <summary>
    /// AudioManagerSO를 테스트할 수 있는 UI 기반 테스트 매니저
    /// </summary>
    public class AudioTestManager : MonoBehaviour
    {
        [Header("Audio Manager")]
        [SerializeField] private AudioManagerRunner audioManagerRunner;
        [SerializeField] private AudioManagerSO audioManagerSO;
        
        [Header("UI Settings")]
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private ScrollRect scrollRect;
        
        [Header("Volume Controls")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle muteToggle;
        
        [Header("BGM Controls")]
        [SerializeField] private Dropdown bgmDropdown;
        [SerializeField] private Button playBGMButton;
        [SerializeField] private Button stopBGMButton;
        [SerializeField] private Button nextBGMButton;
        [SerializeField] private Button prevBGMButton;
        
        [Header("Test Controls")]
        [SerializeField] private KeyCode muteKey = KeyCode.M;
        [SerializeField] private KeyCode stopAllKey = KeyCode.S;
        [SerializeField] private KeyCode testSFXKey = KeyCode.T;
        
        [Header("Debug Info")]
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private bool showDebugInfo = true;
        
        // Private variables
        private List<Button> audioButtons = new List<Button>();
        private List<AudioData> allAudioData = new List<AudioData>();
        private int currentBGMIndex = 0;
        private bool isMuted = false;
        
        void Start()
        {
            InitializeComponents();
            SetupUI();
            LoadAllAudioData();
            CreateAudioButtons();
            SetupVolumeControls();
            SetupBGMControls();
            
            Debug.Log("=== 오디오 테스트 매니저 시작 ===");
            Debug.Log("조작법:");
            Debug.Log("- M: 뮤트 토글");
            Debug.Log("- S: 모든 사운드 정지");
            Debug.Log("- T: 랜덤 SFX 테스트");
        }
        
        void Update()
        {
            HandleInput();
            UpdateDebugInfo();
        }
        
        #region Initialization
        
        private void InitializeComponents()
        {
            // AudioManagerRunner 찾기
            if (audioManagerRunner == null)
            {
                audioManagerRunner = FindObjectOfType<AudioManagerRunner>();
                if (audioManagerRunner == null)
                {
                    Debug.LogWarning("AudioManagerRunner를 찾을 수 없습니다!");
                }
            }
            
            // AudioManagerSO 찾기
            if (audioManagerSO == null)
            {
                audioManagerSO = FindObjectOfType<AudioManagerSO>();
                if (audioManagerSO == null)
                {
                    // Resources에서 찾기
                    audioManagerSO = Resources.Load<AudioManagerSO>("AudioManagerSO");
                    if (audioManagerSO == null)
                    {
                        Debug.LogWarning("AudioManagerSO를 찾을 수 없습니다!");
                    }
                }
            }
            
            // UI 컴포넌트 자동 생성
            CreateMissingUIComponents();
        }
        
        private void CreateMissingUIComponents()
        {
            // Button Container가 없으면 생성
            if (buttonContainer == null)
            {
                GameObject container = new GameObject("AudioButtonContainer");
                container.transform.SetParent(transform);
                buttonContainer = container.transform;
                
                // ScrollRect 추가
                if (scrollRect == null)
                {
                    GameObject scrollView = new GameObject("AudioScrollView");
                    scrollView.transform.SetParent(transform);
                    scrollRect = scrollView.AddComponent<ScrollRect>();
                    
                    // Viewport 생성
                    GameObject viewport = new GameObject("Viewport");
                    viewport.transform.SetParent(scrollView.transform);
                    viewport.AddComponent<RectTransform>();
                    viewport.AddComponent<Mask>();
                    viewport.AddComponent<Image>();
                    
                    // Content 생성
                    GameObject content = new GameObject("Content");
                    content.transform.SetParent(viewport.transform);
                    content.AddComponent<RectTransform>();
                    content.AddComponent<VerticalLayoutGroup>();
                    content.AddComponent<ContentSizeFitter>();
                    
                    // ScrollRect 설정
                    scrollRect.viewport = viewport.GetComponent<RectTransform>();
                    scrollRect.content = content.GetComponent<RectTransform>();
                    scrollRect.vertical = true;
                    scrollRect.horizontal = false;
                    
                    buttonContainer = content.transform;
                }
            }
        }
        
        private void SetupUI()
        {
            // 볼륨 컨트롤 설정
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
                masterVolumeSlider.value = 1f;
            }
            
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
                bgmVolumeSlider.value = 1f;
            }
            
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                sfxVolumeSlider.value = 1f;
            }
            
            if (muteToggle != null)
            {
                muteToggle.onValueChanged.AddListener(OnMuteToggled);
                muteToggle.isOn = false;
            }
        }
        
        #endregion
        
        #region Audio Data Loading
        
        private void LoadAllAudioData()
        {
            allAudioData.Clear();
            
            if (audioManagerSO == null)
            {
                Debug.LogError("AudioManagerSO가 null입니다!");
                return;
            }
            
            // AudioManagerSO의 기본 오디오 데이터베이스에서 로드
            if (audioManagerSO.audioDB != null && audioManagerSO.audioDB.audioList != null)
            {
                foreach (var audioData in audioManagerSO.audioDB.audioList)
                {
                    if (audioData != null)
                    {
                        allAudioData.Add(audioData);
                    }
                }
            }
            
            // 기본 BGM 추가
            if (audioManagerSO.defaultBGM != null)
            {
                allAudioData.Add(audioManagerSO.defaultBGM);
            }
            
            // 추가 오디오 데이터베이스들 로드 (Resources에서 찾기)
            LoadAdditionalAudioDatabases();
            
            Debug.Log($"총 {allAudioData.Count}개의 오디오 데이터를 로드했습니다.");
        }
        
        private void LoadAdditionalAudioDatabases()
        {
            // Resources 폴더에서 모든 AudioDataBase 찾기
            AudioDataBase[] allAudioDatabases = Resources.FindObjectsOfTypeAll<AudioDataBase>();
            
            foreach (var audioDB in allAudioDatabases)
            {
                if (audioDB != null && audioDB.audioList != null)
                {
                    foreach (var audioData in audioDB.audioList)
                    {
                        if (audioData != null && !allAudioData.Contains(audioData))
                        {
                            allAudioData.Add(audioData);
                        }
                    }
                }
            }
            
            Debug.Log($"추가 오디오 데이터베이스 {allAudioDatabases.Length}개에서 오디오 로드 완료");
        }
        
        #endregion
        
        #region UI Button Creation
        
        private void CreateAudioButtons()
        {
            // 기존 버튼들 제거
            ClearAudioButtons();
            
            // 각 오디오 데이터에 대해 버튼 생성
            foreach (var audioData in allAudioData)
            {
                CreateAudioButton(audioData);
            }
            
            Debug.Log($"{audioButtons.Count}개의 오디오 버튼을 생성했습니다.");
        }
        
        private void CreateAudioButton(AudioData audioData)
        {
            GameObject buttonObj;
            
            if (buttonPrefab != null)
            {
                buttonObj = Instantiate(buttonPrefab, buttonContainer);
            }
            else
            {
                buttonObj = CreateDefaultButton();
            }
            
            Button button = buttonObj.GetComponent<Button>();
            if (button == null)
            {
                button = buttonObj.AddComponent<Button>();
            }
            
            // 버튼 텍스트 설정
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                GameObject textObj = new GameObject("ButtonText");
                textObj.transform.SetParent(buttonObj.transform);
                buttonText = textObj.AddComponent<TextMeshProUGUI>();
                
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
            }
            
            string buttonLabel = $"{audioData.clipName}\n볼륨: {audioData.volume:F2}";
            if (audioData.loop)
            {
                buttonLabel += " (루프)";
            }
            buttonText.text = buttonLabel;
            
            // 버튼 클릭 이벤트 설정
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => PlayAudioData(audioData));
            
            audioButtons.Add(button);
        }
        
        private GameObject CreateDefaultButton()
        {
            GameObject buttonObj = new GameObject("AudioButton");
            buttonObj.transform.SetParent(buttonContainer);
            
            // Image 컴포넌트 추가
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Button 컴포넌트 추가
            Button button = buttonObj.AddComponent<Button>();
            
            // RectTransform 설정
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 50);
            
            return buttonObj;
        }
        
        private void ClearAudioButtons()
        {
            foreach (var button in audioButtons)
            {
                if (button != null)
                {
                    DestroyImmediate(button.gameObject);
                }
            }
            audioButtons.Clear();
        }
        
        #endregion
        
        #region Volume Controls
        
        private void SetupVolumeControls()
        {
            // 마스터 볼륨 슬라이더가 없으면 생성
            if (masterVolumeSlider == null)
            {
                masterVolumeSlider = CreateVolumeSlider("MasterVolume", "마스터 볼륨");
            }
            
            // BGM 볼륨 슬라이더가 없으면 생성
            if (bgmVolumeSlider == null)
            {
                bgmVolumeSlider = CreateVolumeSlider("BGMVolume", "BGM 볼륨");
            }
            
            // SFX 볼륨 슬라이더가 없으면 생성
            if (sfxVolumeSlider == null)
            {
                sfxVolumeSlider = CreateVolumeSlider("SFXVolume", "SFX 볼륨");
            }
            
            // 뮤트 토글이 없으면 생성
            if (muteToggle == null)
            {
                muteToggle = CreateMuteToggle();
            }
        }
        
        private Slider CreateVolumeSlider(string name, string label)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(transform);
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            
            // 배경 이미지
            GameObject background = new GameObject("Background");
            background.transform.SetParent(sliderObj.transform);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.gray;
            
            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform);
            
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform);
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = Color.blue;
            
            // Handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(sliderObj.transform);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;
            
            // RectTransform 설정
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(200, 20);
            
            // Slider 설정
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            
            return slider;
        }
        
        private Toggle CreateMuteToggle()
        {
            GameObject toggleObj = new GameObject("MuteToggle");
            toggleObj.transform.SetParent(transform);
            
            Toggle toggle = toggleObj.AddComponent<Toggle>();
            
            // Background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(toggleObj.transform);
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = Color.gray;
            
            // Checkmark
            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform);
            Image checkImage = checkmark.AddComponent<Image>();
            checkImage.color = Color.white;
            
            // Label
            GameObject label = new GameObject("Label");
            label.transform.SetParent(toggleObj.transform);
            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "뮤트";
            labelText.color = Color.white;
            
            // Toggle 설정
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkImage;
            
            return toggle;
        }
        
        private void OnMasterVolumeChanged(float volume)
        {
            if (audioManagerRunner != null && audioManagerRunner.bgmSource != null)
            {
                audioManagerRunner.bgmSource.volume = volume * (isMuted ? 0f : 1f);
            }
            if (audioManagerRunner != null && audioManagerRunner.sfxSource != null)
            {
                audioManagerRunner.sfxSource.volume = volume * (isMuted ? 0f : 1f);
            }
        }
        
        private void OnBGMVolumeChanged(float volume)
        {
            if (audioManagerRunner != null && audioManagerRunner.bgmSource != null)
            {
                audioManagerRunner.bgmSource.volume = volume * (isMuted ? 0f : 1f);
            }
        }
        
        private void OnSFXVolumeChanged(float volume)
        {
            if (audioManagerRunner != null && audioManagerRunner.sfxSource != null)
            {
                audioManagerRunner.sfxSource.volume = volume * (isMuted ? 0f : 1f);
            }
        }
        
        private void OnMuteToggled(bool isMuted)
        {
            this.isMuted = isMuted;
            
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
            
            Debug.Log($"뮤트 상태: {(isMuted ? "켜짐" : "꺼짐")}");
        }
        
        #endregion
        
        #region BGM Controls
        
        private void SetupBGMControls()
        {
            // BGM 드롭다운 설정
            if (bgmDropdown != null)
            {
                bgmDropdown.ClearOptions();
                List<string> bgmOptions = new List<string>();
                
                foreach (var audioData in allAudioData)
                {
                    if (audioData.loop) // BGM은 보통 루프
                    {
                        bgmOptions.Add(audioData.clipName);
                    }
                }
                
                bgmDropdown.AddOptions(bgmOptions);
                bgmDropdown.onValueChanged.AddListener(OnBGMSelected);
            }
            
            // BGM 컨트롤 버튼들
            if (playBGMButton != null)
            {
                playBGMButton.onClick.AddListener(PlayCurrentBGM);
            }
            
            if (stopBGMButton != null)
            {
                stopBGMButton.onClick.AddListener(StopBGM);
            }
            
            if (nextBGMButton != null)
            {
                nextBGMButton.onClick.AddListener(PlayNextBGM);
            }
            
            if (prevBGMButton != null)
            {
                prevBGMButton.onClick.AddListener(PlayPrevBGM);
            }
        }
        
        private void OnBGMSelected(int index)
        {
            currentBGMIndex = index;
            PlayCurrentBGM();
        }
        
        private void PlayCurrentBGM()
        {
            if (bgmDropdown != null && bgmDropdown.options.Count > 0)
            {
                string selectedBGM = bgmDropdown.options[currentBGMIndex].text;
                AudioData bgmData = allAudioData.Find(a => a.clipName == selectedBGM);
                
                if (bgmData != null)
                {
                    PlayBGM(bgmData);
                }
            }
        }
        
        private void PlayNextBGM()
        {
            if (bgmDropdown != null && bgmDropdown.options.Count > 0)
            {
                currentBGMIndex = (currentBGMIndex + 1) % bgmDropdown.options.Count;
                bgmDropdown.value = currentBGMIndex;
                PlayCurrentBGM();
            }
        }
        
        private void PlayPrevBGM()
        {
            if (bgmDropdown != null && bgmDropdown.options.Count > 0)
            {
                currentBGMIndex = (currentBGMIndex - 1 + bgmDropdown.options.Count) % bgmDropdown.options.Count;
                bgmDropdown.value = currentBGMIndex;
                PlayCurrentBGM();
            }
        }
        
        private void StopBGM()
        {
            if (audioManagerRunner != null && audioManagerRunner.bgmSource != null)
            {
                audioManagerRunner.bgmSource.Stop();
                Debug.Log("BGM 정지");
            }
        }
        
        #endregion
        
        #region Audio Playback
        
        private void PlayAudioData(AudioData audioData)
        {
            if (audioData == null || audioData.clipSource == null)
            {
                Debug.LogWarning($"오디오 데이터가 null입니다!");
                return;
            }
            
            if (audioData.loop)
            {
                // BGM으로 재생
                PlayBGM(audioData);
            }
            else
            {
                // SFX로 재생
                PlaySFX(audioData);
            }
            
            Debug.Log($"오디오 재생: {audioData.clipName} (볼륨: {audioData.volume:F2}, 루프: {audioData.loop})");
        }
        
        private void PlayBGM(AudioData bgmData)
        {
            if (audioManagerRunner != null && audioManagerRunner.bgmSource != null)
            {
                audioManagerRunner.bgmSource.clip = bgmData.clipSource;
                audioManagerRunner.bgmSource.volume = bgmData.volume * (isMuted ? 0f : 1f);
                audioManagerRunner.bgmSource.loop = bgmData.loop;
                audioManagerRunner.bgmSource.Play();
                
                Debug.Log($"BGM 재생: {bgmData.clipName}");
            }
        }
        
        private void PlaySFX(AudioData sfxData)
        {
            if (audioManagerRunner != null && audioManagerRunner.sfxSource != null)
            {
                audioManagerRunner.sfxSource.clip = sfxData.clipSource;
                audioManagerRunner.sfxSource.volume = sfxData.volume * (isMuted ? 0f : 1f);
                audioManagerRunner.sfxSource.loop = sfxData.loop;
                audioManagerRunner.sfxSource.Play();
                
                Debug.Log($"SFX 재생: {sfxData.clipName}");
            }
        }
        
        private void PlayRandomSFX()
        {
            List<AudioData> sfxList = allAudioData.FindAll(a => !a.loop);
            
            if (sfxList.Count > 0)
            {
                AudioData randomSFX = sfxList[Random.Range(0, sfxList.Count)];
                PlaySFX(randomSFX);
            }
        }
        
        #endregion
        
        #region Input Handling
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(muteKey))
            {
                ToggleMute();
            }
            
            if (Input.GetKeyDown(stopAllKey))
            {
                StopAllAudio();
            }
            
            if (Input.GetKeyDown(testSFXKey))
            {
                PlayRandomSFX();
            }
        }
        
        private void ToggleMute()
        {
            isMuted = !isMuted;
            
            if (muteToggle != null)
            {
                muteToggle.isOn = isMuted;
            }
            
            OnMuteToggled(isMuted);
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
        
        #endregion
        
        #region Debug Info
        
        private void UpdateDebugInfo()
        {
            if (!showDebugInfo || debugText == null) return;
            
            string debugInfo = $"=== 오디오 테스트 매니저 ===\n";
            debugInfo += $"총 오디오 수: {allAudioData.Count}\n";
            debugInfo += $"뮤트 상태: {(isMuted ? "켜짐" : "꺼짐")}\n";
            
            if (audioManagerRunner != null)
            {
                if (audioManagerRunner.bgmSource != null)
                {
                    debugInfo += $"BGM 재생 중: {audioManagerRunner.bgmSource.isPlaying}\n";
                    debugInfo += $"BGM 볼륨: {audioManagerRunner.bgmSource.volume:F2}\n";
                    if (audioManagerRunner.bgmSource.clip != null)
                    {
                        debugInfo += $"현재 BGM: {audioManagerRunner.bgmSource.clip.name}\n";
                    }
                }
                
                if (audioManagerRunner.sfxSource != null)
                {
                    debugInfo += $"SFX 재생 중: {audioManagerRunner.sfxSource.isPlaying}\n";
                    debugInfo += $"SFX 볼륨: {audioManagerRunner.sfxSource.volume:F2}\n";
                }
            }
            
            debugInfo += $"\n조작법:\n";
            debugInfo += $"M: 뮤트 토글\n";
            debugInfo += $"S: 모든 사운드 정지\n";
            debugInfo += $"T: 랜덤 SFX 테스트\n";
            
            debugText.text = debugInfo;
        }
        
        #endregion
        
        #region Public Methods
        
        public void RefreshAudioList()
        {
            LoadAllAudioData();
            CreateAudioButtons();
        }
        
        public void SetMasterVolume(float volume)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = volume;
            }
        }
        
        public void SetBGMVolume(float volume)
        {
            if (bgmVolumeSlider != null)
            {
                bgmVolumeSlider.value = volume;
            }
        }
        
        public void SetSFXVolume(float volume)
        {
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = volume;
            }
        }
        
        public void SetMute(bool mute)
        {
            if (muteToggle != null)
            {
                muteToggle.isOn = mute;
            }
        }
        
        #endregion
    }
} 