#if UNITY_EDITOR
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
        public CsvTable table; // CSV 데이터 테이블을 에디터에서 할당

        private const string assetPath = "Assets/KYG/Sky Power/Item/"; // 생성될 SO 저장 위치
        private const string prefabPath = "Assets/KYG/Sky Power/Prefabs/";

        [ContextMenu("CSV로부터 아이템 SO 생성")]
        public void CreateItemsFromCSV()
        {
            if (table == null)
            {
                Debug.LogError("CSV 테이블이 할당되지 않았습니다.");
                return;
            }

            CsvReader.Read(table); // (필요하다면) 테이블 새로고침

            for (int i = 1; i < table.Table.GetLength(0); i++) // 0번은 헤더이므로 1부터 시작
            {
                var item = ScriptableObject.CreateInstance<ItemData>();

                // 데이터 파싱 및 예외 처리
                if (!int.TryParse(table.GetData(i, 0), out item.id))
                {
                    Debug.LogError($"ID 파싱 실패: {table.GetData(i, 0)} (라인 {i})");
                    continue;
                }
                item.itemName = table.GetData(i, 1);
                if (!int.TryParse(table.GetData(i, 2), out item.itemTime))
                    Debug.LogWarning($"itemTime 파싱 실패: {table.GetData(i, 2)} (라인 {i})");
                if (!int.TryParse(table.GetData(i, 3), out item.value))
                    Debug.LogWarning($"value 파싱 실패: {table.GetData(i, 3)} (라인 {i})");
                if (!int.TryParse(table.GetData(i, 4), out item.itemEffect))
                    Debug.LogWarning($"itemEffect 파싱 실패: {table.GetData(i, 4)} (라인 {i})");

                string prefabName = table.GetData(i, 5);
                if (!string.IsNullOrEmpty(prefabName))
                    item.itemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}{prefabName}.prefab");
                else
                    item.itemPrefab = null;

                item.description = table.GetData(i, 6);

                // 파일명 및 중복 삭제 처리
                string fileName = $"{assetPath}{item.itemName}_{item.id}.asset";
                if (AssetDatabase.LoadAssetAtPath<ItemData>(fileName) != null)
                    AssetDatabase.DeleteAsset(fileName);

                AssetDatabase.CreateAsset(item, fileName);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("아이템 SO 생성 완료!");
        }
    }
}
#endif
