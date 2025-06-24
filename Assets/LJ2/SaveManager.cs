using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

namespace LJ2
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager instance;
        public static SaveManager Instance { get { return instance; } }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public PlayerSave player;

        public void Save(int index)
        {
            DataSaveController.Save(player.saveDataSample, index);
        }

        public void Load(int index)
        {
            DataSaveController.Load(ref player.saveDataSample, index);
        }

        public void Delete(int index)
        {
            DataSaveController.Delete(player.saveDataSample, index);
        }
    }
}
