using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

public class DataSaveTest : MonoBehaviour
{
    private SaveDataSample jsonSave;
    private SaveDataSample jsonLoad = new();

    private void Start()
    {
        
    }

    public void Save(int index)
    {
        //jsonSave = new(50);

        DataSaveController.Save(jsonSave, index);
    }

    public void Load(int index)
    {
        DataSaveController.Load(ref jsonLoad, index);
        //Debug.Log($"Hp = {jsonLoad.Hp}");
    }

    public void Delete(int index)
    {
        DataSaveController.Delete(jsonLoad, index);
        //Debug.Log(jsonLoad.Hp.ToString());

    }
}


