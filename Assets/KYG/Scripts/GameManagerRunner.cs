using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerRunner : MonoBehaviour
{
    public GameManagerSO gameManager;

    void Start()
    {
        // 시작 시 게임 재개 상태로 초기화
        gameManager.ResumeGame();
    }
}
