# FadeUtil 사용법 가이드

## 📋 개요
`FadeUtil`은 Unity에서 페이드 인/아웃 효과를 쉽게 구현할 수 있는 유틸리티 클래스입니다.

## 🚀 주요 기능
- ✅ **싱글톤 패턴**: 전역에서 쉽게 접근 가능
- ✅ **자동 패널 생성**: 페이드 패널을 자동으로 생성
- ✅ **커스터마이징**: 색상, 지속시간, 애니메이션 커브 설정 가능
- ✅ **이벤트 시스템**: 페이드 완료 시 이벤트 발생
- ✅ **즉시 전환**: 즉시 페이드 인/아웃 가능
- ✅ **연속 전환**: 페이드 아웃 후 인 연속 실행

## 🛠️ 설정 방법

### 1. 기본 설정
```csharp
// FadeUtil 컴포넌트를 씬에 추가
// 또는 자동으로 생성되도록 설정
```

### 2. Inspector 설정
- **Enable Transition**: 페이드 효과 활성화/비활성화
- **Use Fade Transition**: 페이드 전환 사용 여부
- **Fade Panel**: 페이드 패널 (자동 생성 가능)
- **Fade Duration**: 페이드 지속 시간
- **Fade Curve**: 애니메이션 커브
- **Fade Color**: 페이드 색상
- **Auto Create Fade Panel**: 자동으로 페이드 패널 생성

## 📖 사용법

### 기본 사용법
```csharp
// 페이드 아웃
FadeUtil.Instance.FadeOut();

// 페이드 인
FadeUtil.Instance.FadeIn();

// 정적 메서드 사용
FadeUtil.StaticFadeOut();
FadeUtil.StaticFadeIn();
```

### 커스텀 설정
```csharp
// 커스텀 지속시간
FadeUtil.Instance.FadeOut(2f);

// 커스텀 색상
FadeUtil.Instance.SetFadeColor(Color.red);
FadeUtil.Instance.FadeOut();

// 커스텀 애니메이션 커브
AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
FadeUtil.Instance.FadeOut(1f, customCurve);
```

### 연속 전환
```csharp
// 페이드 아웃 후 인 (1초씩, 0.5초 대기)
FadeUtil.Instance.FadeOutThenIn(1f, 1f, 0.5f);
```

### 즉시 전환
```csharp
// 즉시 페이드 아웃 (알파값을 1로 설정)
FadeUtil.Instance.FadeOutImmediate();

// 즉시 페이드 인 (알파값을 0으로 설정)
FadeUtil.Instance.FadeInImmediate();
```

### 이벤트 구독
```csharp
private void Start()
{
    // 이벤트 구독
    FadeUtil.OnFadeOutCompleted += OnFadeOutCompleted;
    FadeUtil.OnFadeInCompleted += OnFadeInCompleted;
}

private void OnDestroy()
{
    // 이벤트 구독 해제
    FadeUtil.OnFadeOutCompleted -= OnFadeOutCompleted;
    FadeUtil.OnFadeInCompleted -= OnFadeInCompleted;
}

private void OnFadeOutCompleted()
{
    Debug.Log("페이드 아웃 완료!");
}

private void OnFadeInCompleted()
{
    Debug.Log("페이드 인 완료!");
}
```

## 🎮 UI 버튼에서 사용
```csharp
// UI 버튼의 OnClick 이벤트에 연결
public void OnFadeOutButtonClick()
{
    FadeUtil.Instance.FadeOut();
}

public void OnFadeInButtonClick()
{
    FadeUtil.Instance.FadeIn();
}
```

## 🔧 고급 설정

### 런타임 설정 변경
```csharp
// 전환 설정 업데이트
FadeUtil.Instance.SetTransitionSettings(
    enable: true,
    useFade: true,
    panel: customPanel,
    duration: 2f,
    curve: customCurve,
    color: Color.blue
);

// 개별 설정 변경
FadeUtil.Instance.SetFadeColor(Color.green);
FadeUtil.Instance.SetFadeDuration(3f);
```

### 상태 확인
```csharp
// 전환 중인지 확인
if (FadeUtil.Instance.IsTransitioning)
{
    Debug.Log("페이드 전환 중...");
}

// 페이드 아웃 상태 확인
if (FadeUtil.Instance.IsFadeOut)
{
    Debug.Log("페이드 아웃 상태");
}

// 페이드 인 상태 확인
if (FadeUtil.Instance.IsFadeIn)
{
    Debug.Log("페이드 인 상태");
}
```

## 🎯 실제 사용 예시

### 씬 전환 시 사용
```csharp
public void LoadNextScene()
{
    // 페이드 아웃 후 씬 로드
    FadeUtil.Instance.FadeOut(1f);
    
    // 페이드 아웃 완료 후 씬 로드
    FadeUtil.OnFadeOutCompleted += () => {
        SceneManager.LoadScene("NextScene");
        FadeUtil.Instance.FadeIn(1f);
    };
}
```

### 게임 오버 시 사용
```csharp
public void OnGameOver()
{
    // 빨간색으로 페이드 아웃
    FadeUtil.Instance.SetFadeColor(Color.red);
    FadeUtil.Instance.FadeOut(2f);
    
    FadeUtil.OnFadeOutCompleted += () => {
        ShowGameOverUI();
        FadeUtil.Instance.FadeIn(1f);
    };
}
```

## ⚠️ 주의사항

1. **중복 실행 방지**: `IsTransitioning` 상태를 확인하여 중복 실행을 방지합니다.
2. **이벤트 구독 해제**: `OnDestroy`에서 반드시 이벤트 구독을 해제하세요.
3. **Canvas 설정**: 페이드 패널은 자동으로 최상위 Canvas에 생성됩니다.
4. **DontDestroyOnLoad**: FadeUtil은 씬 전환 시에도 유지됩니다.

## 🐛 문제 해결

### 페이드 패널이 보이지 않는 경우
- Canvas의 Sort Order 확인
- 페이드 패널의 RectTransform 설정 확인
- Image 컴포넌트 존재 여부 확인

### 페이드 효과가 작동하지 않는 경우
- Enable Transition과 Use Fade Transition이 활성화되어 있는지 확인
- FadeUtil 인스턴스가 존재하는지 확인

### 성능 최적화
- 불필요한 이벤트 구독 해제
- 전환 중 중복 호출 방지
- 적절한 지속시간 설정 