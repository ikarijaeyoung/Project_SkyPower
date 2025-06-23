using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

public class SaveDataSample : SaveData
{
    [field: SerializeField] public int Hp {  get; set; }

    public SaveDataSample() 
    { 
        Hp = 100;
    }

    public SaveDataSample(int hp)
    {
        Hp = hp;
    }
}

public class DataSaveTest : MonoBehaviour
{
    private SaveDataSample jsonSave;
    private SaveDataSample jsonLoad;

    private void Start()
    {
        Save();
        Load();
        Delete();
        Debug.Log(jsonLoad.Hp.ToString());
    }

    private void Save()
    {
        jsonSave = new(50);

        DataSaveController.Save(jsonSave);
    }

    private void Load()
    {
        jsonLoad = new();

        DataSaveController.Load(ref jsonLoad);
        Debug.Log($"Hp = {jsonLoad.Hp}");
    }

    private void Delete()
    {
        DataSaveController.Delete(jsonLoad);
        
    }
}


