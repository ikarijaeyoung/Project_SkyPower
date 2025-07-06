using KYG.SkyPower.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace KYG.SkyPower.Dialogue
{
    /// <summary>
    /// 다이얼로그 UI를 관리하는 컴포넌트입니다.
    /// </summary>
    /// <remarks>
    /// 이 컴포넌트는 DialogueManagerSO와 연결되어 다이얼로그의 상태에 따라 UI를 업데이트합니다.
    /// </remarks>

public class DialogueUI : MonoBehaviour
{
    public TMP_Text speakerText;
    public TMP_Text contentText;
    public GameObject rootPanel; // 다이얼로그 UI 전체

    void OnEnable()
    {
        DialogueManagerSO.Instance.OnLineUpdated.AddListener(UpdateUI);
        DialogueManagerSO.Instance.OnDialogueStarted.AddListener(OpenUI);
        DialogueManagerSO.Instance.OnDialogueEnded.AddListener(CloseUI);
    }

    void OnDisable()
    {
        DialogueManagerSO.Instance.OnLineUpdated.RemoveListener(UpdateUI);
        DialogueManagerSO.Instance.OnDialogueStarted.RemoveListener(OpenUI);
        DialogueManagerSO.Instance.OnDialogueEnded.RemoveListener(CloseUI);
    }

    void UpdateUI(string speaker, string content)
    {
        rootPanel.SetActive(true);
        speakerText.text = speaker;
        contentText.text = content;
    }
    void OpenUI() => rootPanel.SetActive(true);
    void CloseUI() => rootPanel.SetActive(false);

    public void OnNextButton() => DialogueManagerSO.Instance.NextLine();
}
}