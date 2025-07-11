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
        // === �̱��� ������ ===
        private static DialogueManager instance; // �̱��� �ν��Ͻ�
        [Header("Prefab")]
        public GameObject dialogueUIPrefab; // ��ȭ UI ������ (Canvas ������ ��ġ�� ��)
        public static DialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DialogueManager>(); // ������ �̹� �ִ��� Ȯ��
                    if (instance == null)
                    {
                        GameObject obj = new GameObject(nameof(DialogueManager)); // �̸� ����
                        instance = obj.AddComponent<DialogueManager>(); // ������Ʈ �߰�
                        // �ʿ��: DontDestroyOnLoad(obj);
                    }
                }
                return instance; // �ν��Ͻ� ��ȯ
            }
        }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ����
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject); // �� �߿�!
            AutoFindOrSpawnUI(); // UI �ڵ� �Ҵ� �õ�
        }

        // === ���� �ʵ� ===
        [Header("UI")]
        public GameObject dialoguePanel; // ��ȭ UI �г� (��Ȱ��ȭ ����)
        public Image speakerImage; // ����Ŀ �̹��� (��: Player, Boss ��)
        public TMP_Text speakerNameText; // ����Ŀ �̸� �ؽ�Ʈ (��: "Player", "Boss")
        public TMP_Text dialogueText; // ��ȭ ���� �ؽ�Ʈ
        public Button nextButton; // ���� ��� ��ư

        [Header("Data")]
        public DialogDB dialogDB; // ���� ��ȭ �����ͺ��̽� (ScriptableObject)
        public UnityEvent<DialogLine> OnDialogLine = new UnityEvent<DialogLine>();

        private int currentIndex = 0;
        private bool isDialogueActive = false; // ���� ��ȭ Ȱ��ȭ ����

        // === �޼ҵ� ===
        public void StartDialogue() // ��ȭ ���� �޼ҵ�
        {
            // �г��� �ı��ưų� ���� ������� �� �ڵ� ����
            if (dialoguePanel == null || !dialoguePanel)
            {
                //Debug.LogWarning("dialoguePanel�� ���ǵǾ� UI ���Ҵ� �õ�");
                AutoFindOrSpawnUI(); // UI �ڵ� �Ҵ� �õ�
                if (dialoguePanel == null || !dialoguePanel) 
                {
                    // Debug.LogError("dialoguePanel�� ã�� �� �����ϴ�!");
                    return;
                }
            }

            if (dialogDB == null || dialogDB.lines.Count == 0) return;
            isDialogueActive = true; // ��ȭ Ȱ��ȭ �÷��� ����
            dialoguePanel.SetActive(true); // �г� Ȱ��ȭ
            currentIndex = 0; // ��ȭ �ε��� �ʱ�ȭ
            Debug.Log("���� ������.");
            Manager.Game.PausedGame();
            Time.timeScale = 0f; // �Ͻ����� (���� ����)
            Debug.Log($"���ݸ���.{Time.timeScale}");
            ShowLine(); // ù ��° ��� ǥ��
        }

        public void SetDialogDBByStageName(string stageName) 
        {
            // Resources/DialogDB/Stage_1 �̷� ������ ����
            dialogDB = Resources.Load<DialogDB>($"DialogDB/{stageName}"); 
        }

        public void LoadDialogDBByStageID(int mainStageID, int subStageID)
        {
            string dialogDBName = $"Stage_{mainStageID}_{subStageID}";
            dialogDB = Resources.Load<DialogDB>($"DialogDB/{dialogDBName}");
            //if (dialogDB == null)
                // Debug.LogWarning($"DialogDB '{dialogDBName}'�� ã�� �� �����ϴ�!");
        }

        private void AutoFindOrSpawnUI() // UI �ڵ� �Ҵ� �Ǵ� ����
        {
            if (dialoguePanel != null) 
            {
                AutoAssignUIFields(dialoguePanel); // �̹� �Ҵ�� UI�� �ִٸ� �ڵ� �Ҵ�
                return;
            }

            // ���� Canvas ã�� (Scene/Hierarchy���� �ƹ� Canvas�� ���)
            Canvas canvas = FindObjectOfType<Canvas>(); // ������ Canvas ã��
            if (dialogueUIPrefab != null)
            {
                GameObject go; // ��ȭ UI ������ �ν��Ͻ�ȭ
                if (canvas != null)
                    go = Instantiate(dialogueUIPrefab, canvas.transform); // **Canvas ������**
                else
                    go = Instantiate(dialogueUIPrefab); // Fallback

                DontDestroyOnLoad(go); // �� ��ȯ �ÿ��� ����
                dialoguePanel = go; // dialoguePanel �ʵ忡 �Ҵ�
                AutoAssignUIFields(go); // UI �ʵ� �ڵ� �Ҵ�
                return;
            }

            // Debug.LogWarning("DialogueManager: dialogueUIPrefab �Ǵ� Panel�� ã�� �� �����ϴ�!");
        }

        private void AutoAssignUIFields(GameObject root)
        {
            // ��Ȱ��ȭ ���� ��� �������� Ž��!
            speakerImage = root.GetComponentsInChildren<Image>(true) 
                .FirstOrDefault(x => x.name == "SpeakerImage"); // ����Ŀ �̹���
            speakerNameText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "SpeakerNameText"); // ����Ŀ �̸� �ؽ�Ʈ
            dialogueText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "DialogueText"); 
            nextButton = root.GetComponentsInChildren<Button>(true)
                .FirstOrDefault(x => x.name == "NextButton"); // ���� ��� ��ư
            // OnClick �ڵ� �Ҵ�!
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners(); // Ȥ�� �ߺ� ����
                nextButton.onClick.AddListener(OnClickNext); // �ݵ�� DialogueManager�� �޼��� ����!
            }

            // if (speakerImage == null) Debug.LogWarning("speakerImage �ڵ��Ҵ� ����");
            // if (speakerNameText == null) Debug.LogWarning("speakerNameText �ڵ��Ҵ� ����");
            // if (dialogueText == null) Debug.LogWarning("dialogueText �ڵ��Ҵ� ����");
            // if (nextButton == null) Debug.LogWarning("nextButton �ڵ��Ҵ� ����");
        }

        private void ShowLine()
        {
            if (currentIndex >= dialogDB.lines.Count)
            {
                EndDialogue(); // ��ȭ�� ������ ����
                return; 
            }

            var line = dialogDB.lines[currentIndex]; // ���� ��� ��������

            if (speakerImage == null || speakerNameText == null || dialogueText == null)
            {
                Debug.LogError("DialogueManager UI �ʵ� ���� �ȵ�!");
                return;
            }

            // UI ����
            speakerImage.sprite = line.speakerSprite; // ����Ŀ �̹��� ����
            if (line.speakerSprite == null)
                speakerImage.gameObject.SetActive(false); // �̹����� ������ ��Ȱ��ȭ
            else
                speakerImage.gameObject.SetActive(true); // �̹����� ������ Ȱ��ȭ

            speakerNameText.text = line.speaker; // ����Ŀ �̸� ����
            dialogueText.text = line.content; // ��ȭ ���� ����

            OnDialogLine.Invoke(line); // �̺�Ʈ ȣ�� (��� ���� �˸�)
        }

        public void OnClickNext()
        {
            if (!isDialogueActive) return; // ��ȭ�� Ȱ��ȭ���� �ʾ����� ����

            currentIndex++; // ���� ���� �̵�
            ShowLine(); // ���� ��� ǥ��
        }

        private void EndDialogue()
        {
            isDialogueActive = false; // ��ȭ ��Ȱ��ȭ �÷��� ����
            dialoguePanel.SetActive(false); // ��ȭ UI ��Ȱ��ȭ
            Manager.Game.ResumeGame(); // ���� �����
            Time.timeScale = 1f; // �Ͻ����� ���� (���� �����)
        }
    }
}
