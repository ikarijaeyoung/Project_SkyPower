using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{


[System.Serializable]
public class DialogLine
{
    public int id;
    public string speaker; // "Player", "Boss", "Supporter" µî
    public string content;
    public Sprite speakerSprite;
}

    [CreateAssetMenu(menuName = "Dialog/DialogDB")]
    public class DialogDB : ScriptableObject
    {
        public string stageName;   // ¿¹: "Stage_1"
        public List<DialogLine> lines = new List<DialogLine>();
    }
}