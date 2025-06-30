using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JYL
{
    public class SaveCreatePanel : BaseUI
    {
        [SerializeField] int maxInputCount = 8;
        private TMP_InputField inputField;
        private Image bgImage;
        private TMP_Text warningText;
        private bool correctInput = false;
        private Color normalColor = Color.white;
        private Color warningColor = Color.red;
        private Color correctColor = Color.green;

        private void Start()
        {
        }
        private void OnEnable()
        {
            bgImage = GetUI<Image>("SaveInput");
            warningText = GetUI<TMP_Text>("SaveWarningText");
            inputField = GetUI<TMP_InputField>("SaveInput");

            inputField.onEndEdit.AddListener(OnInputEnd);
            inputField.onValueChanged.AddListener(OnInputChanged);

            GetEvent("SaveConfirmBtn").Click += OnStartClick;

            warningText.gameObject.SetActive(false);

        }
        private void OnDisable()
        {
            inputField.onEndEdit.RemoveListener(OnInputEnd);
            inputField.onValueChanged.RemoveListener(OnInputChanged);

            GetEvent("SaveConfirmBtn").Click -= OnStartClick;
        }
        private void OnStartClick(PointerEventData eventData)
        {
            // 게임매니저에 현재 선택한 세이브 파일을 지정함
            // 씬매니저를 통해 씬 전환
            // GameManager.Instance.selectSaveFile = 지정 함수 또는 직접 지정
            // GameSceneManager
            if (correctInput)
            {
                // 다음 씬으로 전환 및 세이브 파일 생성
                // SaveManager.Instance.PlayerSave;
                // GameSceneManager.Instance.세이브파일 로드(최신화)
                // GameSceneManager.Instance.SceneChange();
                SceneManager.LoadSceneAsync("bMainScene_JYL");
            }
            else
            {
                warningText.gameObject.SetActive(true);
                warningText.color = warningColor;
                warningText.text = $"이름을 입력해주세요 !!!";
                bgImage.color = warningColor;
            }
        }
        private void OnInputChanged(string text)
        {
            if (text.Length == 0)
            {
                warningText.gameObject.SetActive(false);
                bgImage.color = normalColor;
                correctInput = false;
            }
            else if (text.Length > maxInputCount)
            {
                warningText.gameObject.SetActive(true);
                warningText.color = warningColor;
                warningText.text = $"이름은 최대 {maxInputCount}글자까지 가능합니다.";
                bgImage.color = warningColor;
                correctInput = false;
            }
            else
            {
                warningText.gameObject.SetActive(true);
                warningText.color = correctColor;
                warningText.text = "사용할 수 있는 이름입니다";
                bgImage.color = normalColor;
                correctInput = true;
            }
        }
        private void OnInputEnd(string text)
        {
            if (text.Length < maxInputCount && text.Length > 0)
            {
                correctInput = true;
            }
        }

    }
}