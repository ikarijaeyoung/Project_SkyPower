using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;


namespace KYG_skyPower
{


    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
    public class ItemData : ScriptableObject
    {
        public int id;
        public string itemName;
        public int itemTime; // 지속시간
        public int value; // 증가량
        public int itemEffect;
        public GameObject itemPrefab;
        public string description; // 아이템 설명
        

    }
}
