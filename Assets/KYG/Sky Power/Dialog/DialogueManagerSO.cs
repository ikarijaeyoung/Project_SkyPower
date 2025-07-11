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
        private static DialogueManager instance; // 싱글톤 인스턴스
        [Header("Prefab")]
        public GameObject dialogueUIPrefab; // 대화 UI 프리팹 (Canvas 하위에 배치될 것)
        public static DialogueManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DialogueManager>(); // 씬에서 이미 있는지 확인
                    if (instance == null)
                    {
                        GameObject obj = new GameObject(nameof(DialogueManager)); // 이름 지정
                        instance = obj.AddComponent<DialogueManager>(); // 컴포넌트 추가
                        // 필요시: DontDestroyOnLoad(obj);
                    }
                }
                return instance; // 인스턴스 반환
            }
        }
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 방지
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject); // ★ 중요!
            AutoFindOrSpawnUI(); // UI 자동 할당 시도
        }

        // === 기존 필드 ===
        [Header("UI")]
        public GameObject dialoguePanel; // 대화 UI 패널 (비활성화 가능)
        public Image speakerImage; // 스피커 이미지 (예: Player, Boss 등)
        public TMP_Text speakerNameText; // 스피커 이름 텍스트 (예: "Player", "Boss")
        public TMP_Text dialogueText; // 대화 내용 텍스트
        public Button nextButton; // 다음 대사 버튼

        [Header("Data")]
        public DialogDB dialogDB; // 현재 대화 데이터베이스 (ScriptableObject)
        public UnityEvent<DialogLine> OnDialogLine = new UnityEvent<DialogLine>();

        private int currentIndex = 0;
        private bool isDialogueActive = false; // 현재 대화 활성화 여부

        // === 메소드 ===
        public void StartDialogue() // 대화 시작 메소드
        {
            // 패널이 파괴됐거나 씬이 변경됐을 때 자동 복구
            if (dialoguePanel == null || !dialoguePanel)
            {
                //Debug.LogWarning("dialoguePanel이 유실되어 UI 재할당 시도");
                AutoFindOrSpawnUI(); // UI 자동 할당 시도
                if (dialoguePanel == null || !dialoguePanel) 
                {
                    // Debug.LogError("dialoguePanel을 찾을 수 없습니다!");
                    return;
                }
            }

            if (dialogDB == null || dialogDB.lines.Count == 0) return;
            isDialogueActive = true; // 대화 활성화 플래그 설정
            dialoguePanel.SetActive(true); // 패널 활성화
            currentIndex = 0; // 대화 인덱스 초기화
            Debug.Log("여기 실행함.");
            Manager.Game.PausedGame();
            Time.timeScale = 0f; // 일시정지 (게임 멈춤)
            Debug.Log($"지금멈춤.{Time.timeScale}");
            ShowLine(); // 첫 번째 대사 표시
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
            //if (dialogDB == null)
                // Debug.LogWarning($"DialogDB '{dialogDBName}'를 찾을 수 없습니다!");
        }

        private void AutoFindOrSpawnUI() // UI 자동 할당 또는 생성
        {
            if (dialoguePanel != null) 
            {
                AutoAssignUIFields(dialoguePanel); // 이미 할당된 UI가 있다면 자동 할당
                return;
            }

            // 기존 Canvas 찾기 (Scene/Hierarchy에서 아무 Canvas나 사용)
            Canvas canvas = FindObjectOfType<Canvas>(); // 씬에서 Canvas 찾기
            if (dialogueUIPrefab != null)
            {
                GameObject go; // 대화 UI 프리팹 인스턴스화
                if (canvas != null)
                    go = Instantiate(dialogueUIPrefab, canvas.transform); // **Canvas 하위로**
                else
                    go = Instantiate(dialogueUIPrefab); // Fallback

                DontDestroyOnLoad(go); // 씬 전환 시에도 유지
                dialoguePanel = go; // dialoguePanel 필드에 할당
                AutoAssignUIFields(go); // UI 필드 자동 할당
                return;
            }

            // Debug.LogWarning("DialogueManager: dialogueUIPrefab 또는 Panel을 찾을 수 없습니다!");
        }

        private void AutoAssignUIFields(GameObject root)
        {
            // 비활성화 포함 모든 하위에서 탐색!
            speakerImage = root.GetComponentsInChildren<Image>(true) 
                .FirstOrDefault(x => x.name == "SpeakerImage"); // 스피커 이미지
            speakerNameText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "SpeakerNameText"); // 스피커 이름 텍스트
            dialogueText = root.GetComponentsInChildren<TMP_Text>(true)
                .FirstOrDefault(x => x.name == "DialogueText"); 
            nextButton = root.GetComponentsInChildren<Button>(true)
                .FirstOrDefault(x => x.name == "NextButton"); // 다음 대사 버튼
            // OnClick 자동 할당!
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners(); // 혹시 중복 방지
                nextButton.onClick.AddListener(OnClickNext); // 반드시 DialogueManager의 메서드 연결!
            }

            // if (speakerImage == null) Debug.LogWarning("speakerImage 자동할당 실패");
            // if (speakerNameText == null) Debug.LogWarning("speakerNameText 자동할당 실패");
            // if (dialogueText == null) Debug.LogWarning("dialogueText 자동할당 실패");
            // if (nextButton == null) Debug.LogWarning("nextButton 자동할당 실패");
        }

        private void ShowLine()
        {
            if (currentIndex >= dialogDB.lines.Count)
            {
                EndDialogue(); // 대화가 끝나면 종료
                return; 
            }

            var line = dialogDB.lines[currentIndex]; // 현재 대사 가져오기

            if (speakerImage == null || speakerNameText == null || dialogueText == null)
            {
                Debug.LogError("DialogueManager UI 필드 연결 안됨!");
                return;
            }

            // UI 갱신
            speakerImage.sprite = line.speakerSprite; // 스피커 이미지 설정
            if (line.speakerSprite == null)
                speakerImage.gameObject.SetActive(false); // 이미지가 없으면 비활성화
            else
                speakerImage.gameObject.SetActive(true); // 이미지가 있으면 활성화

            speakerNameText.text = line.speaker; // 스피커 이름 설정
            dialogueText.text = line.content; // 대화 내용 설정

            OnDialogLine.Invoke(line); // 이벤트 호출 (대사 변경 알림)
        }

        public void OnClickNext()
        {
            if (!isDialogueActive) return; // 대화가 활성화되지 않았으면 무시

            currentIndex++; // 다음 대사로 이동
            ShowLine(); // 다음 대사 표시
        }

        private void EndDialogue()
        {
            isDialogueActive = false; // 대화 비활성화 플래그 설정
            dialoguePanel.SetActive(false); // 대화 UI 비활성화
            Manager.Game.ResumeGame(); // 게임 재시작
            Time.timeScale = 1f; // 일시정지 해제 (게임 재시작)
        }
    }
}
