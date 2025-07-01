using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        public DialogDataSO dialogData; // 에디터에서 할당 or Resources.Load

        DialogLine currentLine;
        int currentId = 0; // 대화 시작 id(0이나 원하는 값)

        public override void Init()
        {
            if (dialogData != null)
                dialogData.Init();
        }

        public void StartDialogue(int startId = 0)
        {
            currentId = startId;
            ShowLine(currentId);
        }

        void ShowLine(int id)
        {
            currentLine = dialogData.GetLine(id);
            if (currentLine == null)
            {
                Debug.Log("대화 종료!");
                // UI 숨김, 이벤트 콜백 등
                return;
            }

            // 1. 텍스트, 화자, 초상화 등 UI에 표시
            Debug.Log($"{currentLine.speaker}: {currentLine.text}");

            // 2. Portrait: UIManager 등과 연결 (예시)
            // var portrait = Resources.Load<Sprite>(currentLine.portraitKey);
            // portraitImage.sprite = portrait;

            // 3. 보이스: AudioManager 등과 연결
            // AudioManager.Instance.PlayVoice(currentLine.voiceKey);

            // 4. 분기 선택지(선택지 버튼 표시)
            if (currentLine.isBranch)
            {
                ShowBranchOptions(currentLine.nextIds);
            }
            else
            {
                // "다음" 버튼 or 입력 대기
                // UI에서 NextLine() 호출
            }
        }

        public void NextLine()
        {
            if (currentLine == null) return;
            if (currentLine.nextIds.Count > 0)
                ShowLine(currentLine.nextIds[0]);
            else
                Debug.Log("대화 종료");
        }

        void ShowBranchOptions(List<int> nextIds)
        {
            for (int i = 0; i < nextIds.Count; i++)
            {
                var nextLine = dialogData.GetLine(nextIds[i]);
                Debug.Log($"선택지 {i + 1}: {nextLine.text}");
                // 실전: 버튼 UI 생성/할당 → OnClick에 SelectBranch(i) 연결
            }
        }

        public void SelectBranch(int branchIndex)
        {
            if (currentLine.isBranch && branchIndex < currentLine.nextIds.Count)
                ShowLine(currentLine.nextIds[branchIndex]);
        }
    }
}
