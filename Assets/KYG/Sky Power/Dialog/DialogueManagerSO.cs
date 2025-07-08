using KYG_skyPower;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KYG_skyPower
{
    [DefaultExecutionOrder(-100)]
    public class DialogueManager : MonoBehaviour
    {
        // === 싱글톤 구현부 ===
        private static DialogueManager instance;
        [Header("Prefab")]
        public GameObject dialogueUIPrefab;
        public static DialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DialogueManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject(nameof(DialogueManager));
                        instance = obj.AddComponent<DialogueManager>();
                        // 필요시: DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
        }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject); // ★ 중요!
            AutoFindOrSpawnUI();
        }

        // === 기존 필드 ===
        [Header("UI")]
        public GameObject dialoguePanel;
        public Image speakerImage;
        public TMP_Text speakerNameText;
        public TMP_Text dialogueText;
        public Button nextButton;

        [Header("Data")]
        public DialogDB dialogDB;
        public UnityEvent<DialogLine> OnDialogLine = new UnityEvent<DialogLine>();

        private int currentIndex = 0;
        private bool isDialogueActive = false;

        // === 메소드 ===
        public void StartDialogue()
        {
            // 패널이 파괴됐거나 씬이 변경됐을 때 자동 복구
            if (dialoguePanel == null || !dialoguePanel)
            {
                Debug.LogWarning("dialoguePanel이 유실되어 UI 재할당 시도");
                AutoFindOrSpawnUI();
                if (dialoguePanel == null || !dialoguePanel)
                {
                    Debug.LogError("dialoguePanel을 찾을 수 없습니다!");
                    return;
                }
            }

            if (dialogDB == null || dialogDB.lines.Count == 0) return;

            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            currentIndex = 0;
            Time.timeScale = 0f;
            ShowLine();
        }

        public void SetDialogDBByStageName(string stageName)
        {
            // Resources/DialogDB/Stage_1 이런 식으로 관리
            dialogDB = Resources.Load<DialogDB>($"DialogDB/{stageName}");
        }

        public void LoadDialogDBByStageID(int mainStageID, int subStageID)
        {
            string dialogDBName = $"Stage_{mainStageID}_{subStageID}";
            dialogDB = Resources.Load<DialogDB>($"DialogDB/{dialogDBName}");
            if (dialogDB == null)
                Debug.LogWarning($"DialogDB '{dialogDBName}'를 찾을 수 없습니다!");
        }

        private void AutoFindOrSpawnUI()
        {
            if (dialoguePanel != null)
            {
                AutoAssignUIFields(dialoguePanel);
                return;
            }

            // 기존 Canvas 찾기 (Scene/Hierarchy에서 아무 Canvas나 사용)
            Canvas canvas = FindObjectOfType<Canvas>();
            if (dialogueUIPrefab != null)
            {
                GameObject go;
                if (canvas != null)
                    go = Instantiate(dialogueUIPrefab, canvas.transform); // **Canvas 하위로**
                else
                    go = Instantiate(dialogueUIPrefab); // Fallback

                DontDestroyOnLoad(go);
                dialoguePanel = go;
                AutoAssignUIFields(go);
                return;
            }

            Debug.LogWarning("DialogueManager: dialogueUIPrefab 또는 Panel을 찾을 수 없습니다!");
        }

        private void AutoAssignUIFields(GameObject root)
        {
            // 비활성화 포함 모든 하위에서 탐색!
            speakerImage = root.GetComponentsInChildren<Image>(true)
                .FirstOrDefault(x => x.name == "SpeakerImage");
            speakerNameText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "SpeakerNameText");
            dialogueText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "DialogueText");
            nextButton = root.GetComponentsInChildren<Button>(true)
                .FirstOrDefault(x => x.name == "NextButton");
            // OnClick 자동 할당!
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners(); // 혹시 중복 방지
                nextButton.onClick.AddListener(OnClickNext); // 반드시 DialogueManager의 메서드 연결!
            }

            if (speakerImage == null) Debug.LogWarning("speakerImage 자동할당 실패");
            if (speakerNameText == null) Debug.LogWarning("speakerNameText 자동할당 실패");
            if (dialogueText == null) Debug.LogWarning("dialogueText 자동할당 실패");
            if (nextButton == null) Debug.LogWarning("nextButton 자동할당 실패");
        }

        private void ShowLine()
        {
            if (currentIndex >= dialogDB.lines.Count)
            {
                EndDialogue();
                return;
            }

            var line = dialogDB.lines[currentIndex];

            if (speakerImage == null || speakerNameText == null || dialogueText == null)
            {
                Debug.LogError("DialogueManager UI 필드 연결 안됨!");
                return;
            }

            // UI 갱신
            speakerImage.sprite = line.speakerSprite;
            if (line.speakerSprite == null)
                speakerImage.gameObject.SetActive(false);
            else
                speakerImage.gameObject.SetActive(true);

            speakerNameText.text = line.speaker;
            dialogueText.text = line.content;

            OnDialogLine.Invoke(line);
        }

        public void OnClickNext()
        {
            if (!isDialogueActive) return;

            currentIndex++;
            ShowLine();
        }

        private void EndDialogue()
        {
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            Time.timeScale = 1f; // 일시정지 해제 (게임 재시작)
        }
    }
}
