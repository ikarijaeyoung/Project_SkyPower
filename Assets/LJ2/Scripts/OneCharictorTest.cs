using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OneCharictorTest : MonoBehaviour
{
    [SerializeField] private CsvTable table;
    [SerializeField] private CsvTable secondTable;
    public CharictorDataTest charictorData;

    private List<CsvTable> csvTables;

    private void Start()
    {
        csvTables = new List<CsvTable>();
        csvTables.Add(table);
        csvTables.Add(secondTable);

        MakeCharictor();
    }

    private void MakeCharictor()
    {

        for (int i = 0; i < csvTables.Count; i++) 
        {
            CsvReader.Read(csvTables[i]);
            charictorData = ScriptableObject.CreateInstance<CharictorDataTest>();

            charictorData.name = csvTables[i].GetData(0, 0);
            charictorData.level = int.Parse(csvTables[i].GetData(1, 0));
            charictorData.Hp = int.Parse(csvTables[i].GetData(1, 1));
            charictorData.exp = int.Parse(csvTables[i].GetData(1, 2));
            charictorData.attackPower = int.Parse(csvTables[i].GetData(1, 3));
            charictorData.attackSpeed = int.Parse(csvTables[i].GetData(1, 4));
            charictorData.moveSpeed = int.Parse(csvTables[i].GetData(1, 5));

            string assetPath = $"Assets/LJ2/Scripts/Charictor/{charictorData.name}.asset";
            AssetDatabase.CreateAsset(charictorData, assetPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
