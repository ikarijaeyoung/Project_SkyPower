using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

namespace LJ2
{
    public class SaveManager : MonoBehaviour
    {
        public void Save(SaveData target, int index)
        {
            DataSaveController.Save(target, index);
        }

        public void Load(SaveData target, int index)
        {
            //DataSaveController.Load(ref target, index);
        }
        public void Delete(SaveData target, int index)
        {
            DataSaveController.Delete(target, index);
        }
    }
}
