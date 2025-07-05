using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KYG_skyPower
{
public class EquipmentCSVtoSO
{
    [MenuItem("Tools/CSV/Convert Equipment Table to SO")]
    public static void Convert()
    {
        string csvPath = "Assets/KYG/Sky Power/Item/Resources/Equipment_Table.csv";
        string soFolderPath = "Assets/KYG/Sky Power/Item/Equipments/"; // 장비 SO 저장 폴더
        string tableSOPath = "Assets/KYG/Sky Power/Item/Equipment/EquipmentTableSO.asset"; // 테이블 SO 경로

        if (!Directory.Exists(soFolderPath))
            Directory.CreateDirectory(soFolderPath);

        // 기존 장비 SO 삭제 (클린하게 만들고 싶을 때)
        var oldFiles = Directory.GetFiles(soFolderPath, "*.asset");
        foreach (var file in oldFiles) AssetDatabase.DeleteAsset(file);

        var lines = File.ReadAllLines(csvPath);
        if (lines.Length <= 1)
        {
            Debug.LogError("CSV 파일이 비어있습니다.");
            return;
        }

        var tableSO = ScriptableObject.CreateInstance<EquipmentTableSO>();
        tableSO.equipmentList = new List<EquipmentDataSO>();

        for (int i = 1; i < lines.Length; i++) // 0번 라인은 헤더
        {
            var tokens = lines[i].Split(',');
            if (string.IsNullOrWhiteSpace(lines[i]) || tokens.Length < 19) continue;

            var dataSO = ScriptableObject.CreateInstance<EquipmentDataSO>();

            int.TryParse(tokens[0], out dataSO.Equip_Id);
            dataSO.Equip_Grade = tokens[1];
            dataSO.Equip_Name = tokens[2];
            dataSO.Equip_Type = tokens[3];
            dataSO.Equip_Set_Type = tokens[4];
            dataSO.Equip_Img = tokens[5];
            int.TryParse(tokens[6], out dataSO.Equip_Level);
            int.TryParse(tokens[7], out dataSO.Equip_Maxlevel);
            int.TryParse(tokens[8], out dataSO.Equip_Upgrade_Default);
            int.TryParse(tokens[9], out dataSO.Equip_Upgrade_Plus);
            dataSO.Stat_Type = tokens[10];
            int.TryParse(tokens[11], out dataSO.Base_Value);
            int.TryParse(tokens[12], out dataSO.Per_Level);
            dataSO.Effect_Trigger = tokens[13];
            dataSO.Effect_Timing = tokens[14];
            dataSO.Effect_Type = tokens[15];
            int.TryParse(tokens[16], out dataSO.Effect_Value);
            int.TryParse(tokens[17], out dataSO.Effect_Time);
            int.TryParse(tokens[18], out dataSO.Effect_Chance);
            dataSO.Effect_Desc = tokens.Length > 19 ? tokens[19] : "";

            // 개별 SO 저장
            string assetName = $"Equipment_{dataSO.Equip_Id}_{dataSO.Equip_Name}.asset";
            string assetPath = Path.Combine(soFolderPath, assetName);
            AssetDatabase.CreateAsset(dataSO, assetPath);
            tableSO.equipmentList.Add(dataSO);
        }

        // 테이블 SO 저장
        AssetDatabase.CreateAsset(tableSO, tableSOPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("장비 CSV → SO 변환 완료!");
    }
}
}