using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingState : IGameState
{
    public void Enter()
    {
        Debug.Log("게임 시작!");
        // 타이머 시작, 스테이지 시작 등
    }

    public void Update()
    {
        // 보스 등장 조건 체크, 클리어 조건 등
    }

    public void Exit()
    {
        Debug.Log("게임 플레이 종료");
        // 점수 정리, 리소스 해제 등
    }
}
