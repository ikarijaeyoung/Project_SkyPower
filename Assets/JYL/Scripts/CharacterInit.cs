using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;
using KYG_skyPower;

namespace JYL
{
    public class CharacterInit : MonoBehaviour
    {
        [SerializeField] CsvTable table;
        void Start()
        {
            CsvReader.Read(table);

        }

        void Update()
        {

        }

        public void InitCharacterInfo()
        {
            for(int i =2;i<table.Table.GetLength(0);i++)
            {
                int id = int.Parse(table.GetData(i, 0));
                Manager.Game.saveFiles[Manager.Game.currentSaveIndex].characterInventory.AddCharacter(id);
            }
        }

    }
}

