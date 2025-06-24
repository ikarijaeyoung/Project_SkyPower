using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IO
{
    public static class DataSaveController
    {
        private static SaveHandle saveHandle = new SaveHandle();

        public static void Save<T>(T target) where T : SaveData
        {
            saveHandle.Save(target);
        }

        public static void Load<T>(ref T target) where T : SaveData, new()
        {
            saveHandle.Load(ref target);
        }
    }
}
