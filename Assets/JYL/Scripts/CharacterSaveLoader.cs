using LJ2;
using System;
using UnityEngine;
using KYG_skyPower;

namespace JYL
{
    public class CharacterSaveLoader : MonoBehaviour
    {
        public CharactorController[] charactorController;
        public CharactorController mainController;
        public CharactorController sub1Controller;
        public CharactorController sub2Controller;
        public EquipController equipController;
        private string charPrefabPath = "CharacterPrefabs";
        void Update() { }
        public void GetCharPrefab()
        {
            equipController = gameObject.GetOrAddComponent<EquipController>();
            equipController.Init();
            equipController.UpdateEquipInfoBySave();
            //ĳ���� ������ ���� ��������
            charactorController = Resources.LoadAll<CharactorController>(charPrefabPath);
            foreach (CharactorController cont in charactorController)
            {
                cont.SetParameter(equipController.weapon.equipValue,equipController.armor.equipValue);
                switch(cont.partySet)
                {
                    case PartySet.Main:
                        mainController = cont;
                        break;
                    case PartySet.Sub1:
                        sub1Controller = cont;
                        break;
                    case PartySet.Sub2:
                        sub2Controller = cont;
                        break;
                }
            }
            // ���� ���Ķ���� ��.
            Array.Sort(charactorController, (a, b) => a.partySet.CompareTo(b.partySet)); // �߰����� ���ĵ� ������.

        }
    }
}
