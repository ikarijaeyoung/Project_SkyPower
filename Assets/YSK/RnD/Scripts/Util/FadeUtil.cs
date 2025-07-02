using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

namespace YSK
{
    /// <summary>
    /// 페이드 효과를 관리하는 유틸리티 클래스
    /// </summary>
    public class FadeUtil : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private bool enableTransition = true;
        [SerializeField] private bool useFadeTransition = true;
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Color fadeColor = Color.black;
        //[Header("Fade In/Out Settings")]
        //[Tooltip("패이드 아웃 페이드인 설정")]
        //[SerializeField] private float fadeInDuration = 1f;
        //[SerializeField] private float fadeOutDuration = 1f;
        //[SerializeField] private float waitTime = 0.1f;

        [Header("Auto Setup")]
        [SerializeField] private bool autoCreateFadePanel = true;
        [SerializeField] private int canvasSortOrder = 999;
        
        // 싱글톤 패턴
        public static FadeUtil Instance { get; private set; }
        
        // 이벤트
        public static System.Action OnFadeOutCompleted;
        public static System.Action OnFadeInCompleted;
        
        // 프로퍼티
        public bool IsTransitioning { get; private set; }
        public bool IsFadeOut => fadePanel != null && fadePanel.alpha >= 1f;
        public bool IsFadeIn => fadePanel != null && fadePanel.alpha <= 0f;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeFadePanel();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (fadePanel != null)
            {
                // 초기 상태 설정
                fadePanel.alpha = 0f;
                fadePanel.gameObject.SetActive(false);
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// 페이드 아웃 효과 실행
        /// </summary>
        /// <param name="duration">페이드 시간 (기본값: 설정된 시간 사용)</param>
        /// <param name="curve">애니메이션 커브 (기본값: 설정된 커브 사용)</param>
        public void FadeOut(float? duration = null, AnimationCurve curve = null)
        {
            if (!enableTransition || !useFadeTransition)
            {
                Debug.Log("페이드 효과가 비활성화되어 있습니다.");
                return;
            }
            
            if (IsTransitioning)
            {
                Debug.LogWarning("이미 전환 중입니다!");
                return;
            }
            
            StartCoroutine(FadeOutCoroutine(duration ?? fadeDuration, curve ?? fadeCurve));
        }
        
        /// <summary>
        /// 페이드 인 효과 실행
        /// </summary>
        /// <param name="duration">페이드 시간 (기본값: 설정된 시간 사용)</param>
        /// <param name="curve">애니메이션 커브 (기본값: 설정된 커브 사용)</param>
        public void FadeIn(float? duration = null, AnimationCurve curve = null)
        {
            if (!enableTransition || !useFadeTransition)
            {
                Debug.Log("페이드 효과가 비활성화되어 있습니다.");
                return;
            }
            
            if (IsTransitioning)
            {
                Debug.LogWarning("이미 전환 중입니다!");
                return;
            }
            
            StartCoroutine(FadeInCoroutine(duration ?? fadeDuration, curve ?? fadeCurve));
        }
        
        /// <summary>
        /// 페이드 아웃 후 페이드 인 실행
        /// </summary>
        /// <param name="fadeOutDuration">페이드 아웃 시간</param>
        /// <param name="fadeInDuration">페이드 인 시간</param>
        /// <param name="waitTime">중간 대기 시간</param>
        public void FadeOutThenIn(float fadeOutDuration = 1f, float fadeInDuration = 1f, float waitTime = 0.1f)
        {
            StartCoroutine(FadeOutThenInCoroutine(fadeOutDuration, fadeInDuration, waitTime));
        }
        
        /// <summary>
        /// 즉시 페이드 아웃 (알파값을 1로 설정)
        /// </summary>
        public void FadeOutImmediate()
        {
            if (fadePanel != null)
            {
                fadePanel.alpha = 1f;
                fadePanel.gameObject.SetActive(true);
                OnFadeOutCompleted?.Invoke();
            }
        }
        
        /// <summary>
        /// 즉시 페이드 인 (알파값을 0으로 설정)
        /// </summary>
        public void FadeInImmediate()
        {
            if (fadePanel != null)
            {
                fadePanel.alpha = 0f;
                fadePanel.gameObject.SetActive(false);
                OnFadeInCompleted?.Invoke();
            }
        }
        
        /// <summary>
        /// 전환 설정 업데이트
        /// </summary>
        public void SetTransitionSettings(bool enable, bool useFade, CanvasGroup panel, float duration, AnimationCurve curve, Color color)
        {
            enableTransition = enable;
            useFadeTransition = useFade;
            fadePanel = panel;
            fadeDuration = duration;
            fadeCurve = curve;
            fadeColor = color;
            
            if (fadePanel != null)
            {
                fadePanel.gameObject.SetActive(true);
                fadePanel.alpha = 0f;
                
                // Image 컴포넌트 색상 설정
                Image fadeImage = fadePanel.GetComponent<Image>();
                if (fadeImage != null)
                {
                    fadeImage.color = fadeColor;
                }
                else
                {
                    Debug.LogWarning("페이드 패널에 Image 컴포넌트가 없습니다!");
                }
            }
        }
        
        /// <summary>
        /// 페이드 패널 색상 변경
        /// </summary>
        public void SetFadeColor(Color color)
        {
            fadeColor = color;
            if (fadePanel != null)
            {
                Image fadeImage = fadePanel.GetComponent<Image>();
                if (fadeImage != null)
                {
                    fadeImage.color = color;
                }
            }
        }
        
        /// <summary>
        /// 페이드 지속 시간 변경
        /// </summary>
        public void SetFadeDuration(float duration)
        {
            fadeDuration = Mathf.Max(0.1f, duration);
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeFadePanel()
        {
            if (fadePanel == null && autoCreateFadePanel)
            {
                CreateFadePanel();
            }
        }
        
        private void CreateFadePanel()
        {
            // Canvas 찾기 또는 생성
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("FadeCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = canvasSortOrder;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                DontDestroyOnLoad(canvasObj);
            }
            
            // 페이드 패널 생성
            GameObject fadeObj = new GameObject("FadePanel");
            fadeObj.transform.SetParent(canvas.transform, false);
            
            // Image 컴포넌트 추가
            Image fadeImage = fadeObj.AddComponent<Image>();
            fadeImage.color = fadeColor;
            
            // RectTransform 설정
            RectTransform rectTransform = fadeObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // CanvasGroup 추가
            fadePanel = fadeObj.AddComponent<CanvasGroup>();
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
            
            Debug.Log("페이드 패널이 자동으로 생성되었습니다.");
        }
        
        private IEnumerator FadeOutCoroutine(float duration, AnimationCurve curve)
        {
            if (fadePanel == null)
            {
                Debug.LogError("페이드 패널이 null입니다!");
                yield break;
            }
            
            IsTransitioning = true;
            fadePanel.gameObject.SetActive(true);
            
            float elapsed = 0f;
            float startAlpha = fadePanel.alpha;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                fadePanel.alpha = Mathf.Lerp(startAlpha, 1f, curve.Evaluate(t));
                yield return null;
            }
            
            fadePanel.alpha = 1f;
            IsTransitioning = false;
            OnFadeOutCompleted?.Invoke();
        }
        
        private IEnumerator FadeInCoroutine(float duration, AnimationCurve curve)
        {
            if (fadePanel == null)
            {
                Debug.LogError("페이드 패널이 null입니다!");
                yield break;
            }
            
            IsTransitioning = true;
            fadePanel.gameObject.SetActive(true);
            
            float elapsed = 0f;
            float startAlpha = fadePanel.alpha;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                fadePanel.alpha = Mathf.Lerp(startAlpha, 0f, curve.Evaluate(t));
                yield return null;
            }
            
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
            IsTransitioning = false;
            OnFadeInCompleted?.Invoke();
        }
        
        private IEnumerator FadeOutThenInCoroutine(float fadeOutDuration, float fadeInDuration, float waitTime)
        {
            // 페이드 아웃
            yield return StartCoroutine(FadeOutCoroutine(fadeOutDuration, fadeCurve));
            
            // 중간 대기
            yield return new WaitForSeconds(waitTime);
            
            // 페이드 인
            yield return StartCoroutine(FadeInCoroutine(fadeInDuration, fadeCurve));
        }
        
        #endregion
        
        #region Static Utility Methods
        
        /// <summary>
        /// 정적 메서드로 페이드 아웃 실행
        /// </summary>
        public static void StaticFadeOut(float duration = 1f)
        {
            if (Instance != null)
            {
                Instance.FadeOut(duration);
            }
            else
            {
                Debug.LogError("FadeUtil 인스턴스가 없습니다!");
            }
        }
        
        /// <summary>
        /// 정적 메서드로 페이드 인 실행
        /// </summary>
        public static void StaticFadeIn(float duration = 1f)
        {
            if (Instance != null)
            {
                Instance.FadeIn(duration);
            }
            else
            {
                Debug.LogError("FadeUtil 인스턴스가 없습니다!");
            }
        }
        
        #endregion
    }
}