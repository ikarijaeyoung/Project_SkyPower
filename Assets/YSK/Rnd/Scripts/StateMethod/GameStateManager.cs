using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [Header("State Management")]
    [SerializeField] private GameState currentGameState = GameState.MainMenu;
    [SerializeField] private StageProgressState currentStageState = StageProgressState.NotStarted;
    [SerializeField] private PlayerState currentPlayerState = PlayerState.Alive;
    
    [Header("Stage Management")]
    [SerializeField] private int currentStageID = 1;
    [SerializeField] private int maxStageID = 10;
    
    [Header("References")]
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StageTransition stageTransition;
    
    // 이벤트
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<StageProgressState> OnStageStateChanged;
    public static event Action<PlayerState> OnPlayerStateChanged;
    public static event Action<int> OnStageChanged;
    
    // 프로퍼티
    public GameState CurrentGameState => currentGameState;
    public StageProgressState CurrentStageState => currentStageState;
    public PlayerState CurrentPlayerState => currentPlayerState;
    public int CurrentStageID => currentStageID;
    
    private void Awake()
    {
        InitializeReferences();
    }
    
    private void Start()
    {
        // GameSceneManager가 먼저 초기화되도록 약간의 지연
        Invoke(nameof(InitializeGame), 0.1f);
    }
    
    private void InitializeReferences()
    {
        if (stageManager == null)
            stageManager = FindObjectOfType<StageManager>();
            
        if (stageTransition == null)
            stageTransition = FindObjectOfType<StageTransition>();
    }
    
    private void InitializeGame()
    {
        // 항상 메인메뉴로 시작
        SetGameState(GameState.MainMenu);
        SetStageState(StageProgressState.NotStarted);
        SetPlayerState(PlayerState.Alive);
        
        Debug.Log("게임이 메인메뉴 상태로 초기화되었습니다.");
    }
    
    #region Game State Management
    
    /// <summary>
    /// 게임 상태를 변경합니다.
    /// </summary>
    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState) return;
        
        GameState previousState = currentGameState;
        currentGameState = newState;
        
        OnGameStateChanged?.Invoke(newState);
        
        // 상태별 처리
        HandleGameStateChange(previousState, newState);
    }
    
    private void HandleGameStateChange(GameState previousState, GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                if (previousState == GameState.MainMenu || previousState == GameState.StageSelect)
                {
                    StartStage(currentStageID);
                }
                break;
                
            case GameState.StageComplete:
                HandleStageComplete();
                break;
                
            case GameState.GameOver:
                HandleGameOver();
                break;
                
            case GameState.Paused:
                PauseGame();
                break;
        }
    }
    
    #endregion
    
    #region Stage Management
    
    /// <summary>
    /// 스테이지를 시작합니다.
    /// </summary>
    public void StartStage(int stageID)
    {
        Debug.Log($"GameStateManager.StartStage 호출: 스테이지 {stageID}");
        
        if (stageID < 1 || stageID > maxStageID)
        {
            Debug.LogError($"Invalid stage ID: {stageID}");
            return;
        }
        
        currentStageID = stageID;
        SetStageState(StageProgressState.InProgress);
        SetGameState(GameState.Playing);
        
        // 스테이지 전환 - 게임 시작으로 표시
        if (stageTransition != null)
        {
            Debug.Log("StageTransition을 통한 게임 시작");
            stageTransition.StartStageTransition(stageID, true);
        }
        else
        {
            Debug.LogWarning("StageTransition이 null입니다!");
        }
        
        // UI 텍스트 업데이트
        GameSceneManager gameSceneManager = FindObjectOfType<GameSceneManager>();
        if (gameSceneManager != null)
        {
            gameSceneManager.UpdateStageText(stageID);
        }
        
        OnStageChanged?.Invoke(stageID);
    }
    
    /// <summary>
    /// 다음 스테이지로 진행합니다.
    /// </summary>
    public void NextStage()
    {
        if (currentStageID < maxStageID)
        {
            StartStage(currentStageID + 1);
        }
        else
        {
            // 모든 스테이지 완료
            SetGameState(GameState.StageComplete);
        }
    }
    
    /// <summary>
    /// 이전 스테이지로 돌아갑니다.
    /// </summary>
    public void PreviousStage()
    {
        if (currentStageID > 1)
        {
            StartStage(currentStageID - 1);
        }
    }
    
    /// <summary>
    /// 스테이지 완료를 처리합니다.
    /// </summary>
    private void HandleStageComplete()
    {
        SetStageState(StageProgressState.Completed);
        
        // 잠시 후 다음 스테이지로 자동 진행
        Invoke(nameof(AutoNextStage), 2f);
    }
    
    private void AutoNextStage()
    {
        if (currentGameState == GameState.StageComplete)
        {
            NextStage();
        }
    }
    
    #endregion
    
    #region Player State Management
    
    /// <summary>
    /// 플레이어 상태를 변경합니다.
    /// </summary>
    public void SetPlayerState(PlayerState newState)
    {
        if (currentPlayerState == newState) return;
        
        currentPlayerState = newState;
        OnPlayerStateChanged?.Invoke(newState);
        
        HandlePlayerStateChange(newState);
    }
    
    private void HandlePlayerStateChange(PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Dead:
                HandlePlayerDeath();
                break;
                
            case PlayerState.Invincible:
                StartInvincibility();
                break;
        }
    }
    
    private void HandlePlayerDeath()
    {
        SetGameState(GameState.GameOver);
        SetStageState(StageProgressState.Failed);
    }
    
    private void StartInvincibility()
    {
        // 무적 시간 설정
        Invoke(nameof(EndInvincibility), 3f);
    }
    
    private void EndInvincibility()
    {
        if (currentPlayerState == PlayerState.Invincible)
        {
            SetPlayerState(PlayerState.Alive);
        }
    }
    
    #endregion
    
    #region Stage State Management
    
    /// <summary>
    /// 스테이지 진행 상태를 변경합니다.
    /// </summary>
    public void SetStageState(StageProgressState newState)
    {
        if (currentStageState == newState) return;
        
        currentStageState = newState;
        OnStageStateChanged?.Invoke(newState);
    }
    
    #endregion
    
    #region Game Control
    
    /// <summary>
    /// 게임을 일시정지합니다.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    /// <summary>
    /// 게임을 재개합니다.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SetGameState(GameState.Playing);
    }
    
    /// <summary>
    /// 게임을 일시정지/재개 토글합니다.
    /// </summary>
    public void TogglePause()
    {
        if (currentGameState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (currentGameState == GameState.Playing)
        {
            PauseGame();
            SetGameState(GameState.Paused);
        }
    }
    
    /// <summary>
    /// 게임 오버를 처리합니다.
    /// </summary>
    private void HandleGameOver()
    {
        // 게임 오버 UI 표시
        // 재시작 옵션 제공
    }
    
    /// <summary>
    /// 게임을 재시작합니다.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        currentStageID = 1;
        SetGameState(GameState.Playing);
        SetStageState(StageProgressState.InProgress);
        SetPlayerState(PlayerState.Alive);
    }
    
    #endregion
    
    #region Public Methods for External Systems
    
    /// <summary>
    /// 플레이어가 목표 지점에 도달했을 때 호출
    /// </summary>
    public void OnPlayerReachedGoal()
    {
        SetStageState(StageProgressState.Completed);
        SetGameState(GameState.StageComplete);
    }
    
    /// <summary>
    /// 플레이어가 사망했을 때 호출
    /// </summary>
    public void OnPlayerDied()
    {
        SetPlayerState(PlayerState.Dead);
    }
    
    /// <summary>
    /// 파워업 아이템을 획득했을 때 호출
    /// </summary>
    public void OnPowerUpCollected()
    {
        SetPlayerState(PlayerState.PowerUp);
    }
    
    #endregion
}
