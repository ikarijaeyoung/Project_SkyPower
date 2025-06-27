using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CsvToSO : MonoBehaviour
{
    [SerializeField] private CsvTable table;
    private CharictorHasCsv charictorData;

    private void Start()
    {
        CsvReader.Read(table);
        MakeCharictor();
    }

    private void MakeCharictor()
    {
        for (int i = 1; i < table.Table.GetLength(0); i++)
        {
            charictorData = ScriptableObject.CreateInstance<CharictorHasCsv>();

            charictorData.name = table.GetData(i, 0);
            charictorData.step = int.Parse(table.GetData(i, 1));
            charictorData.bullet = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/LJ2/Prefabs/{table.GetData(i, 2)}.Prefab", typeof(GameObject));
            charictorData.element = table.GetData(i, 3);
            //charictorData.image = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/LJ2/Prefabs/{table.GetData(i, 7)}.Prefab", typeof(Sprite));


            string assetPath = $"Assets/LJ2/Scripts/Charictor/{charictorData.name}.asset";
            AssetDatabase.CreateAsset(charictorData, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
