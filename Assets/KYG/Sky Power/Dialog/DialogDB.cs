using KYG.SkyPower;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG.SkyPower
{
    [System.Serializable]
    public class DialogLine
    {
        public int id;                      // ??? ???????
        public string speaker;              // ????
        public Sprite portrait;             // ????
        [TextArea] public string text;      // ??? ????
        public AudioClip sound;             // ????(????)
        public int nextID;                  // ???? ??? id (0 ??? -1?? ????)
    }
}
[CreateAssetMenu(fileName = "DialogDB", menuName = "Dialog/DialogDB")]
public class DialogDB : ScriptableObject
{
    public List<DialogLine> lines;
}
