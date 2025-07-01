using KYG_skyPower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{


    public class InventorySample : MonoBehaviour
    {
        public ItemManagerSO itemManagerSO;         // 에디터에서 할당
        public InventoryManagerSO inventoryManagerSO; // 에디터에서 할당

        void Start()
        {
            // 1. 아이템 추가 (ID로)
            inventoryManagerSO.AddItemById(itemManagerSO, 1, 5);

            // 2. 아이템 추가 (이름으로)
            var item = itemManagerSO.GetItemByName("체력포션");
            if (item != null)
                inventoryManagerSO.AddItem(item, 2);

            // 3. 아이템 사용
            bool used = inventoryManagerSO.UseItem(item, 1);
            Debug.Log($"체력포션 사용 성공? {used}");

            // 4. 인벤토리 전체 출력
            foreach (var slot in inventoryManagerSO.inventory)
            {
                Debug.Log($"보유: {slot.itemData.itemName} x {slot.count}");
            }
        }
    }
}
