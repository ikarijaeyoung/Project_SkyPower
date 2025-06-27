using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class CsvTest : MonoBehaviour
{
    [SerializeField] private CsvTable table;
    private CharictorDataTest charictorData;

    private void Start()
    {
        CsvReader.Read(table);
        MakeCharictor();
    }

    private void MakeCharictor()
    {
        for (int i = 1; i < table.Table.GetLength(0); i++)
        {
            charictorData = ScriptableObject.CreateInstance<CharictorDataTest>();

            charictorData.name = table.GetData(i, 0);
            charictorData.level = int.Parse(table.GetData(i, 1));
            charictorData.Hp = int.Parse(table.GetData(i, 2));
            charictorData.exp = int.Parse(table.GetData(i, 3));
            charictorData.attackPower = int.Parse(table.GetData(i, 4));
            charictorData.attackSpeed = int.Parse(table.GetData(i, 5));
            charictorData.moveSpeed = int.Parse(table.GetData(i, 6));
            charictorData.bulletPrefab = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/LJ2/Prefabs/{table.GetData(i,7)}.Prefab", typeof(GameObject));

            string assetPath = $"Assets/LJ2/Scripts/Charictor/{charictorData.name}.asset";
            AssetDatabase.CreateAsset(charictorData, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
