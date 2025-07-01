using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(menuName = "Manager/InventoryManagerSO")]
    public class InventoryManagerSO : ScriptableObject
    {
        [Header("보유 아이템 리스트")]
        public List<InventorySlot> inventory = new List<InventorySlot>();

        // 아이템 추가 (SO 기준)
        public void AddItem(ItemData itemData, int count = 1)
        {
            var slot = inventory.Find(x => x.itemData == itemData);
            if (slot != null)
                slot.count += count;
            else
                inventory.Add(new InventorySlot { itemData = itemData, count = count });
        }

        // 아이템 추가 (ID 기준, 아이템 매니저 필요)
        public void AddItemById(ItemManagerSO itemManager, int id, int count = 1)
        {
            var item = itemManager.GetItemById(id);
            if (item != null)
                AddItem(item, count);
            else
                Debug.LogError($"[InventoryManagerSO] ID {id}번 아이템을 찾을 수 없습니다!");
        }

        // 아이템 수량 조회
        public int GetCount(ItemData itemData)
        {
            var slot = inventory.Find(x => x.itemData == itemData);
            return slot != null ? slot.count : 0;
        }

        // 아이템 소모
        public bool UseItem(ItemData itemData, int count = 1)
        {
            var slot = inventory.Find(x => x.itemData == itemData);
            if (slot != null && slot.count >= count)
            {
                slot.count -= count;
                if (slot.count <= 0)
                    inventory.Remove(slot);
                return true;
            }
            return false;
        }

        // 전체 클리어
        public void ClearInventory()
        {
            inventory.Clear();
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public ItemData itemData;
        public int count;
    }
}