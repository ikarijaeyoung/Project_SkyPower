# AudioTestManager 사용법 가이드

## 📋 개요
`AudioTestManager`는 기존 `AudioManagerSO`와 `AudioManagerRunner`를 활용하여 모든 오디오를 UI를 통해 쉽게 테스트할 수 있는 시스템입니다.

## 🚀 주요 기능
- ✅ **자동 UI 생성**: 필요한 UI 컴포넌트를 자동으로 생성
- ✅ **전체 오디오 테스트**: BGM, SFX, UI 사운드 모두 테스트 가능
- ✅ **볼륨 조절**: 마스터, BGM, SFX 개별 볼륨 조절
- ✅ **뮤트 기능**: 전체 뮤트 토글
- ✅ **BGM 컨트롤**: BGM 재생/정지/다음/이전
- ✅ **키보드 단축키**: 빠른 조작을 위한 단축키
- ✅ **실시간 디버그**: 현재 오디오 상태 실시간 표시

## 🛠️ 설정 방법

### 1. 기본 설정
```csharp
// 1. 빈 GameObject 생성
GameObject audioTestManager = new GameObject("AudioTestManager");

// 2. AudioTestManager 컴포넌트 추가
AudioTestManager manager = audioTestManager.AddComponent<AudioTestManager>();

// 3. 자동으로 UI가 생성되고 오디오 테스트 가능!
```

### 2. 수동 설정
```csharp
// AudioManagerRunner 연결
manager.audioManagerRunner = FindObjectOfType<AudioManagerRunner>();

// AudioManagerSO 연결
manager.audioManagerSO = Resources.Load<AudioManagerSO>("AudioManagerSO");

// UI 컴포넌트 연결 (선택사항)
manager.buttonContainer = yourButtonContainer;
manager.masterVolumeSlider = yourMasterVolumeSlider;
manager.bgmVolumeSlider = yourBGMVolumeSlider;
manager.sfxVolumeSlider = yourSFXVolumeSlider;
manager.muteToggle = yourMuteToggle;
```

## 🎮 조작법

### 키보드 단축키
- **M**: 뮤트 토글
- **S**: 모든 사운드 정지
- **T**: 랜덤 SFX 테스트

### UI 조작
- **볼륨 슬라이더**: 각종 볼륨 조절
- **뮤트 토글**: 전체 뮤트
- **오디오 버튼**: 개별 오디오 재생
- **BGM 컨트롤**: BGM 재생/정지/다음/이전

## ⚙️ 설정 옵션

### AudioTestManager 설정
```csharp
[Header("Audio Manager")]
audioManagerRunner;     // AudioManagerRunner 참조
audioManagerSO;         // AudioManagerSO 참조

[Header("UI Settings")]
buttonContainer;        // 오디오 버튼 컨테이너
buttonPrefab;          // 버튼 프리팹 (선택사항)
scrollRect;            // 스크롤 영역

[Header("Volume Controls")]
masterVolumeSlider;    // 마스터 볼륨 슬라이더
bgmVolumeSlider;       // BGM 볼륨 슬라이더
sfxVolumeSlider;       // SFX 볼륨 슬라이더
muteToggle;           // 뮤트 토글

[Header("BGM Controls")]
bgmDropdown;          // BGM 선택 드롭다운
playBGMButton;        // BGM 재생 버튼
stopBGMButton;        // BGM 정지 버튼
nextBGMButton;        // 다음 BGM 버튼
prevBGMButton;        // 이전 BGM 버튼

[Header("Test Controls")]
muteKey = KeyCode.M;   // 뮤트 단축키
stopAllKey = KeyCode.S; // 전체 정지 단축키
testSFXKey = KeyCode.T; // SFX 테스트 단축키
```

## 🎵 오디오 데이터 구조

### 지원하는 오디오 타입
1. **BGM**: 배경음악 (루프 재생)
2. **SFX**: 효과음 (일회성 재생)
3. **UI SFX**: UI 효과음 (일회성 재생)

### 오디오 데이터베이스
- `audioDB`: AudioManagerSO의 기본 오디오 데이터베이스
- `defaultBGM`: 기본 BGM 오디오 데이터
- **자동 로드**: Resources 폴더의 모든 AudioDataBase 자동 검색 및 로드
  - BGMAudioDataBase (BGM 오디오)
  - InGameSFXAudioDataBase (인게임 효과음)
  - UISFXAudioDataBase (UI 효과음)

## 🔧 커스터마이징

### 커스텀 버튼 프리팹
```csharp
// 버튼 프리팹 설정
manager.buttonPrefab = yourCustomButtonPrefab;

// 버튼 프리팹에는 다음 컴포넌트가 필요:
// - Button
// - TextMeshProUGUI (자식 오브젝트)
```

### 런타임 오디오 리스트 갱신
```csharp
// 오디오 리스트 새로고침
manager.RefreshAudioList();
```

### 볼륨 프로그래밍 제어
```csharp
// 볼륨 설정
manager.SetMasterVolume(0.8f);
manager.SetBGMVolume(0.6f);
manager.SetSFXVolume(1.0f);

// 뮤트 설정
manager.SetMute(true);
```

## 🎨 UI 레이아웃

### 자동 생성되는 UI 구조
```
AudioTestManager
├── AudioScrollView (ScrollRect)
│   ├── Viewport (Mask)
│   │   └── Content (VerticalLayoutGroup)
│   │       ├── AudioButton 1
│   │       ├── AudioButton 2
│   │       └── ...
├── MasterVolume (Slider)
├── BGMVolume (Slider)
├── SFXVolume (Slider)
├── MuteToggle (Toggle)
└── DebugText (TextMeshProUGUI)
```

### 수동 UI 연결
```csharp
// Canvas에 UI 요소들을 미리 배치하고 연결
manager.buttonContainer = yourButtonContainer;
manager.masterVolumeSlider = yourMasterVolumeSlider;
manager.bgmVolumeSlider = yourBGMVolumeSlider;
manager.sfxVolumeSlider = yourSFXVolumeSlider;
manager.muteToggle = yourMuteToggle;
manager.debugText = yourDebugText;
```

## 🐛 문제 해결

### 오디오가 재생되지 않는 경우
1. **AudioManagerRunner 확인**: 씬에 AudioManagerRunner가 있는지 확인
2. **AudioManagerSO 확인**: AudioManagerSO가 올바르게 설정되었는지 확인
3. **AudioSource 확인**: BGM/SFX AudioSource가 있는지 확인
4. **볼륨 확인**: 볼륨이 0이 아닌지 확인

### UI가 생성되지 않는 경우
1. **Canvas 확인**: 씬에 Canvas가 있는지 확인
2. **EventSystem 확인**: EventSystem이 있는지 확인
3. **TextMeshPro 확인**: TextMeshPro 패키지가 설치되어 있는지 확인

### 성능 최적화
1. **버튼 개수 제한**: 너무 많은 오디오 버튼은 성능에 영향
2. **스크롤 최적화**: ScrollRect의 ContentSizeFitter 사용
3. **메모리 관리**: 사용하지 않는 오디오 데이터 정리

## 📊 디버그 정보

실시간으로 다음 정보를 확인할 수 있습니다:
- **총 오디오 수**: 로드된 오디오 데이터 개수
- **뮤트 상태**: 현재 뮤트 상태
- **BGM 상태**: BGM 재생 여부, 볼륨, 현재 재생 중인 곡
- **SFX 상태**: SFX 재생 여부, 볼륨
- **조작법**: 키보드 단축키 안내

## 🎮 확장 가능성

### 추가 기능 구현
1. **오디오 그룹**: 카테고리별 오디오 그룹화
2. **재생 목록**: BGM 재생 목록 기능
3. **오디오 믹서**: 고급 오디오 믹싱 기능
4. **이퀄라이저**: 실시간 이퀄라이저
5. **오디오 시각화**: 파형, 스펙트럼 표시

### 네트워크 연동
1. **멀티플레이어**: 여러 플레이어의 오디오 동기화
2. **음성 채팅**: 실시간 음성 채팅 기능
3. **오디오 스트리밍**: 실시간 오디오 스트리밍

## 📝 예제 코드

### 기본 사용 예제
```csharp
using UnityEngine;
using YSK;

public class AudioTestExample : MonoBehaviour
{
    private AudioTestManager audioTestManager;
    
    void Start()
    {
        // 오디오 테스트 매니저 생성
        GameObject managerObj = new GameObject("AudioTestManager");
        audioTestManager = managerObj.AddComponent<AudioTestManager>();
        
        // 자동으로 UI가 생성되고 오디오 테스트 가능!
    }
    
    void Update()
    {
        // 추가 기능 구현
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 오디오 리스트 새로고침
            audioTestManager.RefreshAudioList();
        }
    }
}
```

### 커스텀 UI 연결 예제
```csharp
public class CustomAudioTest : MonoBehaviour
{
    [SerializeField] private AudioTestManager audioTestManager;
    [SerializeField] private Transform customButtonContainer;
    [SerializeField] private Slider customMasterVolume;
    [SerializeField] private Toggle customMuteToggle;
    
    void Start()
    {
        // 커스텀 UI 연결
        audioTestManager.buttonContainer = customButtonContainer;
        audioTestManager.masterVolumeSlider = customMasterVolume;
        audioTestManager.muteToggle = customMuteToggle;
        
        // 오디오 테스트 시작
        audioTestManager.RefreshAudioList();
    }
}
```

이 시스템을 사용하여 `AudioManagerSO`의 모든 오디오를 쉽게 테스트할 수 있습니다! 

## 🔍 문제 진단

### 1. 현재 상황 확인
화면에 표시되는 디버그 정보를 확인해보세요:
- `BGM AudioSource: 있음/없음`
- `SFX AudioSource: 있음/없음`
- `AudioManagerSO: 있음/없음`

### 2. 가능한 원인들

#### 원인 1: AudioManagerSO의 데이터가 비어있음
```csharp
// AudioManagerSO에서 다음을 확인:
- defaultBGM이 null이 아닌지
- defaultBGM.clipSource가 null이 아닌지
- audioDB.audioList에 실제 오디오 클립이 있는지
```

#### 원인 2: AudioSource 설정 문제
```csharp
// AudioSource에서 확인할 것들:
- Play On Awake가 false인지
- Mute가 false인지
- Volume이 0이 아닌지
- AudioClip이 할당되어 있는지
```

#### 원인 3: 오디오 클립 자체 문제
```csharp
// 오디오 클립에서 확인할 것들:
- 실제 오디오 파일이 있는지
- 오디오 파일이 손상되지 않았는지
- 오디오 포맷이 Unity에서 지원하는지
```

## 🔧 해결 방법

### 1. SimpleAudioTest에 디버그 정보 추가
`SimpleAudioTest.cs`의 `TestBGM()` 메서드를 다음과 같이 수정하세요:

```csharp
<code_block_to_apply_changes_from>
private void TestBGM()
{
    Debug.Log("=== BGM 테스트 시작 ===");
    
    // AudioManagerSO 확인
    if (audioManagerSO == null)
    {
        Debug.LogError("AudioManagerSO가 null입니다!");
        return;
    }
    Debug.Log($"AudioManagerSO 찾음: {audioManagerSO.name}");
    
    // defaultBGM 확인
    if (audioManagerSO.defaultBGM == null)
    {
        Debug.LogError("defaultBGM이 null입니다!");
        return;
    }
    Debug.Log($"defaultBGM 찾음: {audioManagerSO.defaultBGM.clipName}");
    
    // clipSource 확인
    if (audioManagerSO.defaultBGM.clipSource == null)
    {
        Debug.LogError("defaultBGM.clipSource가 null입니다!");
        return;
    }
    Debug.Log($"clipSource 찾음: {audioManagerSO.defaultBGM.clipSource.name}");
    
    // AudioManagerRunner 확인
    if (audioManagerRunner == null)
    {
        Debug.LogError("AudioManagerRunner가 null입니다!");
        return;
    }
    Debug.Log($"AudioManagerRunner 찾음: {audioManagerRunner.name}");
    
    // bgmSource 확인
    if (audioManagerRunner.bgmSource == null)
    {
        Debug.LogError("bgmSource가 null입니다!");
        return;
    }
    Debug.Log($"bgmSource 찾음: {audioManagerRunner.bgmSource.name}");
    
    // 오디오 재생
    audioManagerRunner.bgmSource.clip = audioManagerSO.defaultBGM.clipSource;
    audioManagerRunner.bgmSource.volume = audioManagerSO.defaultBGM.volume;
    audioManagerRunner.bgmSource.loop = audioManagerSO.defaultBGM.loop;
    audioManagerRunner.bgmSource.mute = isMuted;
    
    Debug.Log($"오디오 설정 완료:");
    Debug.Log($"  - Clip: {audioManagerRunner.bgmSource.clip?.name}");
    Debug.Log($"  - Volume: {audioManagerRunner.bgmSource.volume}");
    Debug.Log($"  - Loop: {audioManagerRunner.bgmSource.loop}");
    Debug.Log($"  - Mute: {audioManagerRunner.bgmSource.mute}");
    
    audioManagerRunner.bgmSource.Play();
    
    Debug.Log($"BGM 재생 시작: {audioManagerSO.defaultBGM.clipName}");
    Debug.Log("=== BGM 테스트 완료 ===");
}
```

### 2. 수동 테스트
Inspector에서 SimpleAudioTest 컴포넌트를 선택하고:
1. **Context Menu**에서 **BGM 테스트** 클릭
2. Console 창에서 상세한 디버그 정보 확인

### 3. AudioSource 직접 확인
AudioManagerRunner 오브젝트의 자식 AudioSource들을 직접 확인:
1. **BGM AudioSource** 선택
2. Inspector에서:
   - **AudioClip**이 할당되어 있는지 확인
   - **Volume**이 0이 아닌지 확인
   - **Mute**가 체크되지 않았는지 확인
   - **Play On Awake**가 체크되어 있는지 확인

### 4. AudioManagerSO 확인
AudioManagerSO 에셋을 선택하고:
1. **defaultBGM** 필드에 AudioData가 할당되어 있는지 확인
2. **defaultBGM.clipSource**에 실제 오디오 클립이 할당되어 있는지 확인
3. **audioDB.audioList**에 오디오 데이터가 있는지 확인

## 🎨 빠른 해결 방법

### 1. 테스트용 오디오 클립 생성
```csharp
// SimpleAudioTest에 테스트용 오디오 클립 생성 메서드 추가
[ContextMenu("테스트 오디오 클립 생성")]
public void CreateTestAudioClip()
{
    // 간단한 사인파 오디오 클립 생성
    int sampleRate = 44100;
    float duration = 1f;
    int samples = (int)(sampleRate * duration);
    
    AudioClip testClip = AudioClip.Create("TestClip", samples, 1, sampleRate, false);
    float[] data = new float[samples];
    
    for (int i = 0; i < samples; i++)
    {
        data[i] = Mathf.Sin(2f * Mathf.PI * 440f * i / sampleRate) * 0.5f;
    }
    
    testClip.SetData(data, 0);
    
    // AudioSource에 직접 할당
    if (audioManagerRunner != null && audioManagerRunner.bgmSource != null)
    {
        audioManagerRunner.bgmSource.clip = testClip;
        audioManagerRunner.bgmSource.volume = 0.5f;
        audioManagerRunner.bgmSource.loop = true;
        audioManagerRunner.bgmSource.Play();
        
        Debug.Log("테스트 오디오 클립 재생 시작!");
    }
}
```

이렇게 하면 문제의 원인을 정확히 파악하고 해결할 수 있습니다! 🎵 