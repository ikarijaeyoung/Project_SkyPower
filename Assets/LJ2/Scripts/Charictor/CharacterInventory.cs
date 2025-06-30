using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

public class CharacterInventory : MonoBehaviour
{
    [SerializeField] public List<CharacterSave> characters = new List<CharacterSave>();
    
    public void AddCharacter(int id)
    {
        // Check if the character already exists in the inventory
        for (int i = 0; i < characters.Count; i++) 
        {
            if (characters[i].id == id)
            {
                if(characters[i].step < 4) // Assuming step 3 is the maximum
                {
                    var temp = characters[i];
                    temp.step++; // Increment step if character already exists
                    characters[i] = temp; // Update the character in the list
                }
                
                return; // Exit if character already exists
            }

        }

        characters.Add(new CharacterSave(id));
    }

}

public struct CharacterSave
{
    [SerializeField] public int id;
    
    [SerializeField] public int level;

    [SerializeField] public int step;
    public CharacterSave(int id)
    {
        this.id = id;
        this.level = 1; // Default level
        this.step = 0; // Default step
    }
}