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
        // Initialize character inventory
        characterInventory = new CharacterInventory();
    }
}
