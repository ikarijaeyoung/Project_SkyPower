using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IO;

public partial class GameData : SaveData
{
    public string playerName;

    public CharacterInventory characterInventory;

    public GameData()
    {
        // Set the file name to the type name of this class
        // = GetType().Name;
        
        // Initialize character inventory
        characterInventory = new CharacterInventory();
    }
}
