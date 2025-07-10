using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class EquipmentInvenManager : MonoBehaviour
    {
        public static EquipmentInvenManager Instance { get; private set; }

        public EquipmentTableSO equipmentTableSO; // �ڷᰡ �ִ� SO���� ����Ʈ

        // ĳ���ͺ� ���� ����
        private Dictionary<int, CharacterEquipmentSlots> characterEquipSlots = new Dictionary<int, CharacterEquipmentSlots>();

        //public List<EquipmentController> inventory = new List<EquipmentDataSO>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(this);
        }

        // Ÿ�Ժ�(����/��/�Ǽ�����) ����Ʈ ��ȯ
        //public List<EquipmentDataSO> GetItemList(string equipType)
        //{
        //    //return equipmentTableSO.equipmentList.FindAll(e => e.Equip_Type == equipType);
        //}

        // ĳ���� ��� ����
        public void EquipItem(int characterId, EquipmentDataSO equipment)
        {
            if (!characterEquipSlots.ContainsKey(characterId))
                characterEquipSlots[characterId] = new CharacterEquipmentSlots();

            //switch (equipment.Equip_Type)
            //{
            //    case "weapon": characterEquipSlots[characterId].weapon = equipment; break;
            //    case "armor": characterEquipSlots[characterId].armor = equipment; break;
            //    case "accessory": characterEquipSlots[characterId].accessory = equipment; break;
            //}
        }

        // ĳ���� ���� ���� ��ȯ
        public CharacterEquipmentSlots GetEquippedItems(int characterId)
        {
            if (!characterEquipSlots.ContainsKey(characterId))
                characterEquipSlots[characterId] = new CharacterEquipmentSlots();
            return characterEquipSlots[characterId];
        }

        //public void AddToInventory(EquipmentDataSO equip)
        //{
        //    ����1, ����1
        //    if (!inventory.Contains(equip)) inventory.Add(equip);
        //}
        //public void RemoveFromInventory(EquipmentDataSO equip)
        //{
        //    if (inventory.Contains(equip)) inventory.Remove(equip);
        //}
    }

    [System.Serializable]
    public class CharacterEquipmentSlots
    {
        public EquipmentDataSO weapon;
        public EquipmentDataSO armor;
        public EquipmentDataSO accessory;
    }
}
