//// RnD/Scripts/ScriptableObjectMethod/StageSelectUI.cs
//using UnityEngine;
//using UnityEngine.UI;
//using YSK;
//using KYG_skyPower;
//using JYL;
//using TMPro;
//using System.Collections;
//using System.Net.NetworkInformation;

//namespace YSK
//{
//    public class StageSelectUI : BaseUI
//    {
//        [Header("Stage Data")]
//        [SerializeField] private StageDataManager dataManager;

//        [Header("UI Button Names")]
//        [SerializeField] private string mainMenuButtonName = "MainMenuButton";
//        [SerializeField] private string mainStageSelectButtonName = "MainStageSelectButton";
//        [SerializeField] private string testSceneButtonName = "TestSceneButton";

//        [Header("Stage Button Names")]
//        [SerializeField] private string stage1_1ButtonName = "Stage1_1Button";
//        [SerializeField] private string stage1_2ButtonName = "Stage1_2Button";
//        [SerializeField] private string stage2_1ButtonName = "Stage2_1Button";
//        [SerializeField] private string stage2_2ButtonName = "Stage2_2Button";
//        [SerializeField] private string stage3_1ButtonName = "Stage3_1Button";

//        [Header("Score Display Names")]
//        [SerializeField] private string stage1_1ScoreTextName = "Stage1_1ScoreText";
//        [SerializeField] private string stage1_2ScoreTextName = "Stage1_2ScoreText";
//        [SerializeField] private string stage2_1ScoreTextName = "Stage2_1ScoreText";
//        [SerializeField] private string stage2_2ScoreTextName = "Stage2_2ScoreText";
//        [SerializeField] private string stage3_1ScoreTextName = "Stage3_1ScoreText";

//        [Header("Debug")]
//        [SerializeField] private bool forceUIUpdate = false;

//        private void Start()
//        {
//            InitializeDataManager();
//            ConnectButtons();
            
//            // ������ �ε��� ��ٸ� �� UI ������Ʈ
//            StartCoroutine(UpdateUIAfterDataLoad());
            
//            // StageDataManager �̺�Ʈ ����
//            if (StageDataManager.OnStageDataChanged != null)
//            {
//                StageDataManager.OnStageDataChanged += UpdateUI;
//            }
//        }

//        private void OnDestroy()
//        {
//            // �̺�Ʈ ���� ����
//            if (StageDataManager.OnStageDataChanged != null)
//            {
//                StageDataManager.OnStageDataChanged -= UpdateUI;
//            }
//        }

//        private System.Collections.IEnumerator UpdateUIAfterDataLoad()
//        {
//            // StageDataManager�� �ʱ�ȭ�� ������ ���
//            yield return new WaitForEndOfFrame();
            
//            // �� ������ �� ����Ͽ� �����Ͱ� ������ �ε�ǵ��� ��
//            yield return new WaitForEndOfFrame();
            
//            Debug.Log("=== ������ �ε� �Ϸ� �� UI ������Ʈ ���� ===");
//            UpdateUI();
//        }

//        private void InitializeDataManager()
//        {
//            if (dataManager == null)
//            {
//                dataManager = FindObjectOfType<StageDataManager>();
//                if (dataManager == null)
//                {
//                    Debug.LogWarning("StageDataManager�� ã�� �� �����ϴ�!");
//                }
//            }
//        }

//        private void ConnectButtons()
//        {
//            Debug.Log("=== ConnectButtons ���� ===");
            
            
//            // �޴� ��ư�� ����
//            ConnectMenuButton(mainMenuButtonName, "bMainScene_JYL");
//            ConnectMenuButton(mainStageSelectButtonName, "cStoreScene_JYL");
//            ConnectMenuButton(testSceneButtonName, "aTitleScene_JYL");

//            // �������� ��ư�� ����
//            ConnectStageButton(stage1_1ButtonName, 1, 1);
//            ConnectStageButton(stage1_2ButtonName, 1, 2);
//            ConnectStageButton(stage2_1ButtonName, 2, 1);
//            ConnectStageButton(stage2_2ButtonName, 2, 2);
//            ConnectStageButton(stage3_1ButtonName, 3, 1);
            
//            Debug.Log("=== ConnectButtons �Ϸ� ===");
//        }

//        private void ConnectMenuButton(string buttonName, string sceneName)
//        {
//            Debug.Log($"ConnectMenuButton �õ�: {buttonName}");
            
//            // 1. ���ӿ�����Ʈ ���� ã��
//            GameObject buttonGO = GetUI(buttonName);
//            if (buttonGO == null)
//            {
//                Debug.LogError($"���ӿ�����Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"���ӿ�����Ʈ ã��: {buttonGO.name}");
            
//            // 2. Button ������Ʈ ã��
//            Button button = buttonGO.GetComponent<Button>();
//            if (button == null)
//            {
//                Debug.LogError($"Button ������Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"Button ������Ʈ ã��: {button.name}");
            
//            button.onClick.RemoveAllListeners();
//            button.onClick.AddListener(() => {
//                Debug.Log($"{buttonName} ��ư Ŭ����! �� {sceneName} �ε�");
//                LoadScene(sceneName);
//            });
//            Debug.Log($"{buttonName} �޴� ��ư ���� �Ϸ�");
//        }

//        private void ConnectStageButton(string buttonName, int mainStage, int subStage)
//        {
//            Debug.Log($"ConnectStageButton �õ�: {buttonName}");
            
//            // 1. ���ӿ�����Ʈ ���� ã��
//            GameObject buttonGO = GetUI(buttonName);
//            if (buttonGO == null)
//            {
//                Debug.LogError($"���ӿ�����Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"���ӿ�����Ʈ ã��: {buttonGO.name}");
            
//            // 2. Button ������Ʈ ã��
//            Button button = buttonGO.GetComponent<Button>();
//            if (button == null)
//            {
//                Debug.LogError($"Button ������Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"Button ������Ʈ ã��: {button.name}");
            
//            button.onClick.RemoveAllListeners();
//            button.onClick.AddListener(() => {
//                Debug.Log($"{buttonName} ��ư Ŭ����! �������� {mainStage}-{subStage} �ε�");

//                // �ӽ÷� �ر� ���� Ȯ�� �ǳʶٱ� (�׽�Ʈ��)
//                Debug.Log($"�������� {mainStage}-{subStage} �ε� ����...");
//                LoadStage(mainStage, subStage);
//            });
//            Debug.Log($"{buttonName} �������� ��ư ���� �Ϸ�");
//        }

//        private void UpdateUI()
//        {
//            if (dataManager == null) 
//            {
//                Debug.LogError("dataManager�� null�Դϴ�!");
//                return;
//            }

//            Debug.Log("=== UpdateUI ���� ===");
//            Debug.Log($"dataManager ����: {(dataManager != null ? "ã��" : "�� ã��")}");

//            // �������� ��ư ���� ������Ʈ
//            UpdateStageButtonState(stage1_1ButtonName,  stage1_1ScoreTextName, 1, 1);
//            UpdateStageButtonState(stage1_2ButtonName,  stage1_2ScoreTextName, 1, 2);
//            UpdateStageButtonState(stage2_1ButtonName,  stage2_1ScoreTextName, 2, 1);
//            UpdateStageButtonState(stage2_2ButtonName,  stage2_2ScoreTextName, 2, 2);
//            UpdateStageButtonState(stage3_1ButtonName,  stage3_1ScoreTextName, 3, 1);
            
//            Debug.Log("=== UpdateUI �Ϸ� ===");
//        }

//        private void UpdateStageButtonState(string buttonName, string scoreTextName, int mainStage, int subStage)
//        {
//            Debug.Log($"UpdateStageButtonState �õ�: {buttonName} (�������� {mainStage}-{subStage})");
            
//            // 1. ���ӿ�����Ʈ ���� ã��
//            GameObject buttonGO = GetUI(buttonName);
//            if (buttonGO == null)
//            {
//                Debug.LogError($"���ӿ�����Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"���ӿ�����Ʈ ã��: {buttonGO.name}");
            
//            // 2. Button ������Ʈ ã��
//            Button button = buttonGO.GetComponent<Button>();
//            if (button == null)
//            {
//                Debug.LogError($"Button ������Ʈ�� ã�� �� ����: {buttonName}");
//                return;
//            }
            
//            Debug.Log($"Button ������Ʈ ã��: {button.name}");

//            bool isUnlocked = CanPlayStage(mainStage, subStage);
//            Debug.Log($"�������� {mainStage}-{subStage} �ر� ����: {isUnlocked}");

//            // ��ư Ȱ��ȭ/��Ȱ��ȭ - ������ ����
//            button.interactable = isUnlocked;
//            Debug.Log($"��ư {buttonName} interactable ����: {isUnlocked} (���� ��: {button.interactable})");

//            // ��ư ���� ���� - �� ��Ȯ�ϰ� ����
//            ColorBlock colors = button.colors;
//            if (isUnlocked)
//            {
//                colors.normalColor = Color.white;
//                colors.highlightedColor = Color.white;
//                colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
//                colors.selectedColor = Color.white;
//                colors.disabledColor = Color.white;
//                Debug.Log($"��ư {buttonName} ����: ��� (�رݵ�) - Ŭ�� ����");
//            }
//            else
//            {
//                colors.normalColor = Color.gray;
//                colors.highlightedColor = Color.gray;
//                colors.pressedColor = Color.gray;
//                colors.selectedColor = Color.gray;
//                colors.disabledColor = Color.gray;
//                Debug.Log($"��ư {buttonName} ����: ȸ�� (��ݵ�) - Ŭ�� �Ұ�");
//            }
//            button.colors = colors;

//            // ��ư�� Image ������Ʈ�� Ȯ��
//            Image buttonImage = buttonGO.GetComponent<Image>();
//            if (buttonImage != null)
//            {
//                if (isUnlocked)
//                {
//                    buttonImage.color = Color.white;
//                    Debug.Log($"��ư �̹��� ����: ������� ����");
//                }
//                else
//                {
//                    buttonImage.color = Color.gray;
//                    Debug.Log($"��ư �̹��� ����: ȸ������ ����");
//                }
//            }

//            // ���� ǥ��

//                UpdateScoreDisplay(scoreTextName, mainStage, subStage);
            
            
//            Debug.Log($"UpdateStageButtonState �Ϸ�: {buttonName} (interactable: {button.interactable})");
//        }

//        private void UpdateScoreDisplay(string scoreTextName, int mainStage, int subStage)
//        {
//            if (dataManager == null) return;

//            TextMeshProUGUI scoreText = GetUI<TextMeshProUGUI>(scoreTextName);
//            if (scoreText != null)
//            {
//                int score = dataManager.GetStageScore(mainStage, subStage);
//                if (score >= 0)
//                {
//                    scoreText.text = $"�ְ� ����: {score}";
//                    scoreText.gameObject.SetActive(true);
//                    Debug.Log($"�������� {mainStage}-{subStage} ���� ǥ��: {score}");
//                }
//                else
//                {
//                    scoreText.gameObject.SetActive(false);
//                    Debug.Log($"�������� {mainStage}-{subStage} ���� ����");
//                }
//            }
//            else
//            {
//                Debug.LogWarning($"���� �ؽ�Ʈ ������Ʈ�� ã�� �� ����: {scoreTextName}");
//            }
//        }

//        private bool CanPlayStage(int mainStage, int subStage)
//        {
//            if (dataManager == null) 
//            {
//                Debug.LogWarning("dataManager�� null�Դϴ�!");
//                return false;
//            }

//            bool stageUnlocked = dataManager.IsStageUnlocked(mainStage);
//            bool subStageUnlocked = dataManager.IsSubStageUnlocked(mainStage, subStage);
            
//            Debug.Log($"�������� {mainStage}-{subStage} Ȯ��:");
//            Debug.Log($"  - ���� �������� {mainStage} �ر�: {stageUnlocked}");
//            Debug.Log($"  - ���� �������� {subStage} �ر�: {subStageUnlocked}");
            
//            bool canPlay = stageUnlocked && subStageUnlocked;
//            Debug.Log($"  - ���� ���: {canPlay}");
            
//            return canPlay;
//        }

//        private void ShowLockMessage(int mainStage, int subStage)
//        {
//            if (dataManager.IsSubStageUnlocked(mainStage, subStage))
//            {
//                Debug.Log($"�������� {mainStage}-{subStage}�� ���� �رݵ��� �ʾҽ��ϴ�!");
//            }
            
//        }

//        private void LoadStage(int mainStage, int subStage)
//        {
//            Debug.Log($"LoadStage ȣ��: {mainStage}-{subStage}");
            
//            if (GameSceneManager.Instance != null)
//            {
//                Debug.Log($"GameSceneManager.Instance ã��, �� �ε� ����");
//                GameSceneManager.Instance.LoadGameSceneWithStage("dStageScene_JYL", mainStage, subStage);
//            }
//            else
//            {
//                Debug.LogError("GameSceneManager.Instance�� null�Դϴ�!");
//                Debug.LogError("���� GameSceneManager ������Ʈ�� �ִ��� Ȯ�����ּ���!");
//            }
//        }

//        private void LoadScene(string sceneName)
//        {
//            if (GameSceneManager.Instance != null)
//            {
//                GameSceneManager.Instance.LoadGameScene(sceneName);
//            }
//            else
//            {
//                Debug.LogError("GameSceneManager.Instance�� null�Դϴ�!");
//            }
//        }

//        // �ܺο��� UI ������Ʈ�� ��û�� �� ���
//        public void RefreshUI()
//        {
//            UpdateUI();
//        }

//        private void ClearStage()
//        {
            
//        }

//        private void Update()
//        {
//            // �ν����Ϳ��� üũ�ڽ��� ����ϸ� UI ������Ʈ
//            if (forceUIUpdate)
//            {
//                UpdateUI();
//                forceUIUpdate = false;
//                Debug.Log("���� UI ������Ʈ ����");
//            }
//        }
//    }
//}