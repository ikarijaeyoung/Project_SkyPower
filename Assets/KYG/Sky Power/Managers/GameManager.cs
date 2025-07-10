using IO;
using JYL;

using LJ2;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace KYG_skyPower
{

    /*
    
    �̱��� ��� ���� �Ŵ���

    �ʿ� ���
    ���� ���ھ�
    ���� ���� ����
    ���� �Ͻ�����,���� �簳 ���

    �߰� ���
    �̺�Ʈ ��� �ڵ�� Ȯ�强 ���
    ���̺� �����͸� �迭�� ���� �Ŵ����� ������ �;� �Ѵ�
    ���� ���۽� ���̺� ������ ������ �ͼ� ������ �ִ´�

    */

    
    public class GameManager : Singleton<GameManager>
    {
        public UnityEvent onGameOver, onPause, onResume, onGameClear;
        

        public GameData[] saveFiles = new GameData[3]; // ���̺� ���� 3��

        public int currentSaveIndex { get; private set; } = 0;

        public GameData CurrentSave => saveFiles[currentSaveIndex]; // ���̺� ���� �ε����� �迭

        public bool isGameOver { get; private set; } // ���� ����
        public bool isPaused { get; private set; } // ���� �Ͻ� ����

        public bool isGameCleared { get; private set; } // ���� Ŭ����

        public int selectWorldIndex=0;
        public int selectStageIndex=0;

        //[SerializeField] private int defaultPlayerHP = 5;
        //public int playerHp { get; private set; } // �÷��̾ ���� ���� ������ ���߿� �߰� ���� ���� �ּ� ó��

        public override void Init() // ���� ���۽� ���̺� ������ �ε�
        {
            ResetSaveRef();
        }
        public void ResetSaveRef()
        {
            for (int i = 0; i < saveFiles.Length; i++)
            {
                saveFiles[i] = new GameData();
                SaveManager.Instance.GameLoad(ref saveFiles[i], i + 1); // �ε��� 1����
            }
        }

        public void SelectSaveFile(int index)
        {
            if (index >= 0 && index < saveFiles.Length)
                currentSaveIndex = index;
        }

        /*private void Awake() // �̱��� ����
        {
            if (Instance != null && Instance != this) // ���� �ٸ� Instance ������ �� Instance
            {
                Destroy(gameObject); // ����
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ������Ʈ �ı����� �ʰ� ����

        }*/

        void Start()
        {
            AudioManager.Instance.PlayBGM("StarMenu_BGM");
            DialogueManager.Instance.StartDialogue();
        }

        public void ResetState()
        {
            isGameOver = false;
            isGameCleared = false;
            isPaused = false;
        }

        public void SetGameOver()
        {
            if (isGameOver) return;
            isGameOver = true; // ���� ������ true��
            Time.timeScale = 0f; // �ð� ���� ���
            onGameOver?.Invoke();
            UIManager.Instance.ShowPopUp<StageClearPopUp>();
        }

        public void SetGameClear()
        {
            if (isGameCleared || isGameOver) return;
            isGameCleared = true;
            Time.timeScale = 1f;
            Debug.Log("���� Ŭ���� ���·� ����");
            onGameClear?.Invoke();
            UIManager.Instance.ShowPopUp<StageClearPopUp>();
        }

        public void PausedGame()
        {
            if (isPaused || isGameOver) return;
            isPaused = true;
            Time.timeScale = 0f; // ��ü ���� ����
            Debug.Log($"���ݸ���.{Time.timeScale}");
            onPause?.Invoke();
        }

        public void ResumeGame()
        {
            if (!isPaused || isGameOver) return;
            isPaused = false;
            Time.timeScale = 1f; // ���� �ð� ����ȭ
            onResume?.Invoke();
        }

        public void ResetStageIndex()
        {
            selectWorldIndex = 0;
            selectStageIndex = 0;
        }

        public void SaveGameProgress()
        {
            Manager.Save.GameSave(CurrentSave, currentSaveIndex);
        }
        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(isPaused) ResumeGame();
                else PausedGame();
            }
        }*/ // ESC Ű�Է����� �Ͻ����� ��� ���÷� �ۼ�
    }
}

