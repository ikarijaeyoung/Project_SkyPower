using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(menuName = "Manager/ItemManagerSO")]
    public class ItemManagerSO : ScriptableObject
    {
        [Header("아이템 데이터 SO 리스트 (수동 or 자동 할당)")]
        public List<ItemData> allItems = new List<ItemData>();

        // ID로 검색
        public ItemData GetItemById(int id)
        {
            return allItems.Find(item => item != null && item.id == id);
        }

        // 이름으로 검색
        public ItemData GetItemByName(string name)
        {
            return allItems.Find(item => item != null && item.itemName == name);
        }

#if UNITY_EDITOR
        // 에디터 자동 등록 기능 (폴더 내의 모든 ItemData SO 등록)
        [ContextMenu("폴더에서 자동으로 아이템 SO 모두 등록")]
        public void AutoCollectAllItems()
        {
            allItems.Clear();
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/KYG/Sky Power/Item" });
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                ItemData item = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item != null && !allItems.Contains(item))
                    allItems.Add(item);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"[ItemManagerSO] {allItems.Count}개 SO 자동 등록 완료!");
        }
#endif
    }
}
