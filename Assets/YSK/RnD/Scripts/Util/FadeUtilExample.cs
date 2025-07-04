using UnityEngine;
using UnityEngine.UI;
using YSK;

namespace YSK
{
    /// <summary>
    /// FadeUtil 사용법 예제
    /// </summary>
    public class FadeUtilExample : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private KeyCode fadeOutKey = KeyCode.F1;
        [SerializeField] private KeyCode fadeInKey = KeyCode.F2;
        [SerializeField] private KeyCode fadeOutThenInKey = KeyCode.F3;
        [SerializeField] private KeyCode immediateFadeOutKey = KeyCode.F4;
        [SerializeField] private KeyCode immediateFadeInKey = KeyCode.F5;
        
        [Header("Custom Settings")]
        [SerializeField] private float customDuration = 2f;
        [SerializeField] private Color customColor = Color.red;
        
        private void Start()
        {
            // FadeUtil 이벤트 구독
            FadeUtil.OnFadeOutCompleted += OnFadeOutCompleted;
            FadeUtil.OnFadeInCompleted += OnFadeInCompleted;
            
            Debug.Log("FadeUtil 예제가 시작되었습니다.");
            Debug.Log($"F1: 페이드 아웃, F2: 페이드 인, F3: 페이드 아웃 후 인, F4: 즉시 페이드 아웃, F5: 즉시 페이드 인");
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            FadeUtil.OnFadeOutCompleted -= OnFadeOutCompleted;
            FadeUtil.OnFadeInCompleted -= OnFadeInCompleted;
        }
        
        private void Update()
        {
            // 키보드 입력으로 테스트
            if (Input.GetKeyDown(fadeOutKey))
            {
                TestFadeOut();
            }
            
            if (Input.GetKeyDown(fadeInKey))
            {
                TestFadeIn();
            }
            
            if (Input.GetKeyDown(fadeOutThenInKey))
            {
                TestFadeOutThenIn();
            }
            
            if (Input.GetKeyDown(immediateFadeOutKey))
            {
                TestImmediateFadeOut();
            }
            
            if (Input.GetKeyDown(immediateFadeInKey))
            {
                TestImmediateFadeIn();
            }
        }
        
        #region Test Methods
        
        /// <summary>
        /// 기본 페이드 아웃 테스트
        /// </summary>
        private void TestFadeOut()
        {
            Debug.Log("=== 페이드 아웃 테스트 ===");
            
            // 방법 1: 인스턴스 직접 사용
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.FadeOut();
            }
            
            // 방법 2: 정적 메서드 사용
            // FadeUtil.StaticFadeOut();
            
            // 방법 3: 커스텀 설정으로 사용
            // FadeUtil.Instance.FadeOut(customDuration);
        }
        
        /// <summary>
        /// 기본 페이드 인 테스트
        /// </summary>
        private void TestFadeIn()
        {
            Debug.Log("=== 페이드 인 테스트 ===");
            
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.FadeIn();
            }
        }
        
        /// <summary>
        /// 페이드 아웃 후 인 테스트
        /// </summary>
        private void TestFadeOutThenIn()
        {
            Debug.Log("=== 페이드 아웃 후 인 테스트 ===");
            
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.FadeOutThenIn(1f, 1f, 0.5f);
            }
        }
        
        /// <summary>
        /// 즉시 페이드 아웃 테스트
        /// </summary>
        private void TestImmediateFadeOut()
        {
            Debug.Log("=== 즉시 페이드 아웃 테스트 ===");
            
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.FadeOutImmediate();
            }
        }
        
        /// <summary>
        /// 즉시 페이드 인 테스트
        /// </summary>
        private void TestImmediateFadeIn()
        {
            Debug.Log("=== 즉시 페이드 인 테스트 ===");
            
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.FadeInImmediate();
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnFadeOutCompleted()
        {
            Debug.Log("페이드 아웃이 완료되었습니다!");
        }
        
        private void OnFadeInCompleted()
        {
            Debug.Log("페이드 인이 완료되었습니다!");
        }
        
        #endregion
        
        #region Public Methods (UI 버튼용)
        
        /// <summary>
        /// UI 버튼에서 호출할 수 있는 페이드 아웃 메서드
        /// </summary>
        public void OnFadeOutButtonClick()
        {
            TestFadeOut();
        }
        
        /// <summary>
        /// UI 버튼에서 호출할 수 있는 페이드 인 메서드
        /// </summary>
        public void OnFadeInButtonClick()
        {
            TestFadeIn();
        }
        
        /// <summary>
        /// UI 버튼에서 호출할 수 있는 페이드 아웃 후 인 메서드
        /// </summary>
        public void OnFadeOutThenInButtonClick()
        {
            TestFadeOutThenIn();
        }
        
        /// <summary>
        /// 커스텀 색상으로 페이드 아웃
        /// </summary>
        public void OnCustomColorFadeOutButtonClick()
        {
            if (FadeUtil.Instance != null)
            {
                FadeUtil.Instance.SetFadeColor(customColor);
                FadeUtil.Instance.FadeOut(customDuration);
            }
        }
        
        #endregion
    }
} 