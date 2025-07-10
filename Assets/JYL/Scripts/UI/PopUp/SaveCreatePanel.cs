using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using KYG_skyPower;

namespace JYL
{
    public class SaveCreatePanel : BaseUI
    {
        [SerializeField] CharacterInit characterInit;
        [SerializeField] EquipController equipController;
        [SerializeField] int maxInputCount = 8;
        private TMP_InputField inputField;
        private Image bgImage;
        private TMP_Text warningText;
        private bool correctInput = false;
        private Color normalColor = Color.white;
        private Color warningColor = Color.red;
        private Color correctColor = Color.green;

        private void OnEnable()
        {
            equipController = GetComponent<EquipController>();
            characterInit = GetComponent<CharacterInit>();
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
            if (correctInput)
            {
                int index = Manager.Game.currentSaveIndex;
                characterInit.InitCharacterInfo();
                // TODO: ��� �迭 ���� �ֱ�
                equipController.Init();
                equipController.CreateEquipInfo(); // SO�� �����迭 ����
                equipController.SaveFileInit(); // SO������ ����
                Manager.Save.GameSave(Manager.Game.CurrentSave, index,inputField.text);
                Manager.Game.ResetSaveRef();
                SceneManager.LoadSceneAsync("bMainScene_JYL");
            }
            else
            {
                warningText.gameObject.SetActive(true);
                warningText.color = warningColor;
                warningText.text = $"�̸��� �Է����ּ��� !!!";
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
                warningText.text = $"�̸��� �ִ� {maxInputCount}���ڱ��� �����մϴ�.";
                bgImage.color = warningColor;
                correctInput = false;
            }
            else
            {
                warningText.gameObject.SetActive(true);
                warningText.color = correctColor;
                warningText.text = "����� �� �ִ� �̸��Դϴ�";
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