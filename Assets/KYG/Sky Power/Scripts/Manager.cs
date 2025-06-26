using KYG;
using KYG_skyPower;
using LJ2;
using YSK;
using JYL;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace KYG_skyPower
{


    public static class Manager
    {
        public static SaveManager Save => SaveManager.Instance;
        public static GameManager Game => GameManager.Instance;
        public static InputManager Input => InputManager.Instance;
        public static GameSceneManager GameScene => GameSceneManager.Instance;

        //public static StageManager Stage => StageManager.Instance;
        //public static CharacterManager Character => CharacterManager.Instance;
        //public static EnemyManager Enemy => EnemyManager.Instance;
        //public static UIManager UI => UIManager.Instance;
        public static ScoreManager Score => ScoreManager.Instance;
        public static DialogueManager Dialogue => DialogueManager.Instance;

        public static AudioManagerSO Audio => AudioManagerSO.Instance;

        public static SceneChangerManagerSO SceneChanger => SceneChangerManagerSO.Instance;


        /*void Awake()
        {
            SaveManager.Instance.Init();
            GameManager.Instance.Init();
            //CharacterManager.Instance.Init();
            //EnemyManager.Instance.Init();
            //UIManager.Instance.Init();
           //AudioManager.Instance.Init();
            //DialogueManager.Instance.Init();
        }*/

        public static void InitAll()
        {
            Audio.Init();
            SceneChanger.Init();

            //Save.Init();
            Game.Init();
            Input.Init();
            //GameScene.Init();
            //Character.Init();
            //Enemy.Init();
            //UI.Init();

            Score.Init();
            Dialogue.Init();


        }
    }
}

