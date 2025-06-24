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
    private SaveDataSample jsonLoad = new();

    private void Start()
    {
        
    }

    public void Save()
    {
        jsonSave = new(50);

        DataSaveController.Save(jsonSave, 0);
    }

    public void Load()
    {
        DataSaveController.Load(ref jsonLoad, 0);
        Debug.Log($"Hp = {jsonLoad.Hp}");
    }

    public void Delete()
    {
        DataSaveController.Delete(jsonLoad, 0);
        Debug.Log(jsonLoad.Hp.ToString());

    }
}


