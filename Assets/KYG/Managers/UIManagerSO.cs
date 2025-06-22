using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManager", menuName = "Managers/UIManager")]
public class UIManagerSO : ScriptableObject
{
    [HideInInspector] public GameObject pauseUI;

    // UI 패널 객체 연결
    public void Init(GameObject pausePanel)
    {
        pauseUI = pausePanel;
    }

    // 일시 정지 UI On/Off
    public void ShowPauseUI(bool show)
    {
        if (pauseUI != null)
            pauseUI.SetActive(show);
    }
}