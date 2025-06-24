using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public SaveDataSample saveDataSample;

    public void NewData()
    {
        saveDataSample = new SaveDataSample();
    }
}
