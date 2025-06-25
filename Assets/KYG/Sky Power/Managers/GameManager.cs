using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


namespace KYG_skyPower
{

    /*
    
    싱글톤 기반 게임 매니저

    필요 기능
    게임 스코어
    게임 오버 여부
    게임 일시정지,게임 재개 기능

    추가 기능
    이벤트 기반 코드로 확장성 고려

    */

    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        //[Header("게임 상태")]

        public int score { get; private set; } // 현재 게임 스코어
        public bool isGameOver { get; private set; } // 게임 오버
        public bool isPaused { get; private set; } // 게임 일시 정지

        //[SerializeField] private int defaultPlayerHP = 5;
        //public int playerHp { get; private set; } // 플레이어에 붙을 수도 있지만 나중에 추가 될지 몰라 주석 처리

        [Header("이벤트")]
        public UnityEvent onGameOver; // 게임 오버 시 호출
        public UnityEvent<int> onScoreChanged; // 점수 변경 시 호출(파라미터: 변경된 점수)
        public UnityEvent onPause;  // 일시정지 시 호출
        public UnityEvent onResume;  // 일시정지 해제 시 호출

        private void Awake() // 싱글톤 패턴
        {
            if (Instance != null && Instance != this) // 만약 다른 Instance 있으면 본 Instance
            {
                Destroy(gameObject); // 삭제
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // 게임 오브젝트 파괴되지 않게 제한
            Init(); // 게임 상태 초기화
        }


        public void Init()
        {
            score = 0;
            isGameOver = false;
            isPaused = false;
            Time.timeScale = 1f; // 다시 시간 흐르게
            //playerHp = defaultPlayerHp;
        }

        public void AddScore(int value) // 점수 추가
        {
            if (isGameOver) return; // 게임오버 상태에서 점수 변동 X
            score += value;
            onScoreChanged?.Invoke(score); // UI, 이펙트 등 이벤트와 연동 가능
        }

        public void SetGameOver()
        {
            isGameOver = true; // 게임 오버가 true면
            Time.timeScale = 0f; // 시간 정지 기능
            onGameOver?.Invoke();
            Debug.Log("게임 오버");            
        }

        public void PausedGame()
        {
            if (isPaused || isGameOver) return;
            isPaused = true;
            Time.timeScale = 0f; // 전체 게임 정지
            onGameOver?.Invoke();
            Debug.Log("일시 정지");
        }

        public void ResumeGame()
        {
            if (!isPaused || isGameOver) return;
            isPaused = false;
            Time.timeScale = 1f; // 게임 시간 정상화
            onGameOver?.Invoke();
            Debug.Log("게임 재개");
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(isPaused) ResumeGame();
                else PausedGame();
            }
        }*/ // ESC 키입력으로 일시정지 기능 예시로 작성
    }
}

