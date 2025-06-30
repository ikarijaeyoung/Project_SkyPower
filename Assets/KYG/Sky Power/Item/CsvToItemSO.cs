using IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KYG_skyPower
{
    public class CsvToItemSO : MonoBehaviour
    {
        [Header("CSV 데이터 테이블")]
        public CsvTable table; // 직접 에디터에서 할당

        private const string assetPath = "Assets/KYG/Sky Power/Item/"; // 저장 경로
        private const string prefabPath = "Assets/KYG/Sky Power/Prefabs/";

        [ContextMenu("CSV로부터 아이템 SO 생성")]
        public void CreateItemsFromCSV()
        {
            if (table == null)
            {
                Debug.LogError("CSV 테이블이 할당되지 않았습니다.");
                return;
            }

            

            CsvReader.Read(table); // 필요하다면

            for (int i = 1; i < table.Table.GetLength(0); i++) // 0번은 헤더라면 1부터
            {
                ItemData item = ScriptableObject.CreateInstance<ItemData>();

                if (!int.TryParse(table.GetData(i, 0), out item.id))
                {
                    Debug.LogError($"ID 파싱 실패: {table.GetData(i, 0)} (라인 {i})");
                    continue;
                }
                item.itemName = table.GetData(i, 1);
                int.TryParse(table.GetData(i, 2), out item.itemTime);
                int.TryParse(table.GetData(i, 3), out item.value);
                int.TryParse(table.GetData(i, 4), out item.itemEffect);

                string prefabName = table.GetData(i, 5);
                if (!string.IsNullOrEmpty(prefabName))
                    item.itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}{prefabName}.prefab");
                else
                    item.itemPrefab = null;

                item.description = table.GetData(i, 6);

                // 저장 경로 설정
                string fileName = $"{assetPath}{item.itemName}_{item.id}.asset";

                
                
                    AssetDatabase.DeleteAsset(fileName);

                AssetDatabase.CreateAsset(item, fileName);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("아이템 SO 생성 완료!");
        }
    }
}

