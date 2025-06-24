using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IO
{
    public static class DataSaveController
    {
        private static SaveHandle saveHandle = new SaveHandle();

        public static void Save<T>(T target, int index) where T : SaveData
        {
            saveHandle.Save(target, index);
        }

        public static void Load<T>(ref T target, int index) where T : SaveData, new()
        {
            saveHandle.Load(ref target, index);
        }

        public static void Delete<T>(T target, int index) where T : SaveData
        {
            saveHandle.Delete(target, index);
        }
    }
}
