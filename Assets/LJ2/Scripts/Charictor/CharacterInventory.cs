using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KYG_skyPower;

[System.Serializable]
public class CharacterInventory
{
    [SerializeField] public List<CharacterSave> characters;

    public CharacterInventory()
    {
        // Initialize the character list
        characters = new List<CharacterSave>();
    }
    public void AddCharacter(int id)
    {
        Debug.Log($"여기 들어옴{id} 이거 추가중");
        // Check if the character already exists in the inventory
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].id == id && characters[i].level > 0)
            {
                if (characters[i].step < 4)
                {
                    var temp = characters[i];
                    temp.step++;
                    characters[i] = temp; // Update the character in the list
                    return; // Exit if character already exists
                }
                else
                {
                    Manager.Game.CurrentSave.gold += 10;
                    return; // Exit if character has reached maximum fragle level
                }
            }
            //else if (characters[i].id == id && characters[i].level <= 0) 
            //{
            //    var temp = characters[i];
            //    temp.level = 1;
            //    characters[i] = temp;
            //    return;
            //}
        }
        characters.Add(new CharacterSave(id)); // id가 아예 없는 경우임
    }
    public void GachaAdd(int id)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].id == id && characters[i].level > 0)
            {
                if (characters[i].step < 4)
                {
                    var temp = characters[i];
                    temp.step++;
                    characters[i] = temp; // Update the character in the list
                    return; // Exit if character already exists
                }
                else
                {
                    Manager.Game.CurrentSave.gold += 10;
                    return; // Exit if character has reached maximum fragle level
                }
            }
            else if (characters[i].id == id && characters[i].level <= 0)
            {
                var temp = characters[i];
                temp.level = 1;
                characters[i] = temp;
                return;
            }
        }
    }
}
    [System.Serializable]
    public struct CharacterSave
    {
        [SerializeField] public int id;
        [SerializeField] public int level;

        [SerializeField] public int step;
        [SerializeField] public PartySet partySet;
        public CharacterSave(int id)
        {
            this.id = id;
            switch (id)
            {
                case 10009:
                    partySet = PartySet.Main;
                    level = 1;
                    break;
                case 10015:
                    partySet = PartySet.Sub1;
                    level = 1;
                    break;
                case 10027:
                    partySet = PartySet.Sub2;
                    level = 1;
                    break;
                default:
                    level = -1;// Default level // 소유 시, 1레벨로 변경
                    partySet = PartySet.None;
                    break;
            }
            this.step = 0; // Default step
        }
    }
    public enum PartySet { Main, Sub1, Sub2, None }