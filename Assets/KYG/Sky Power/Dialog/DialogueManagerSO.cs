using KYG.SkyPower.Dialogue;
using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace KYG.SkyPower.Dialogue
{
    // DialogueManagerSO: 대사 관리 싱글톤
    // - 대사 데이터, 현재 진행중인 대사 인덱스, 대사 재생 상태 관리
    // - UIManager, GameManager 등에서 구독 가능
    // - 대사 시작, 종료, 현재 대사 업데이트 이벤트 제공

[CreateAssetMenu(fileName = "DialogueManagerSO", menuName = "Manager/DialogueManager")]
public class DialogueManagerSO : SOSingleton<DialogueManagerSO>
{
    [Header("진행중 대사 데이터")]
    public DialogueDataSO currentDialogue;
    public int currentLineIndex;
    public bool isDialoguePlaying;

    // UIManager, GameManager 등에서 구독 가능
    public UnityEvent<string, string> OnLineUpdated = new UnityEvent<string, string>();
    public UnityEvent OnDialogueStarted = new UnityEvent();
    public UnityEvent OnDialogueEnded = new UnityEvent();

    public void StartDialogue(DialogueDataSO dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialoguePlaying = true;
        OnDialogueStarted?.Invoke();
        ShowCurrentLine();
    }

    public void ShowCurrentLine()
    {
        if (!currentDialogue || currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }
        var line = currentDialogue.lines[currentLineIndex];
        OnLineUpdated?.Invoke(line.speaker, line.content);
    }

    public void NextLine()
    {
        if (!isDialoguePlaying) return;
        currentLineIndex++;
        ShowCurrentLine();
    }

    public void EndDialogue()
    {
        isDialoguePlaying = false;
        OnDialogueEnded?.Invoke();
    }
}
}