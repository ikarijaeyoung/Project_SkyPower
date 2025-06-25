using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvTest : MonoBehaviour
{
    [SerializeField] private CsvTable table;

    private void Start()
    {
        CsvReader.Read(table);

        Debug.Log(table.GetData(1, 1));
    }
}
