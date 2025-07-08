using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{


[System.Serializable] 
public class DialogLine 
{
        public int id; // 대사 ID, 이름, 설명
        public string speaker; // "Player", "Boss", "Supporter" 등
        public string content; // 대사 내용
        public Sprite speakerSprite; // 스피커의 이미지 (예: Player, Boss 등)
    }

    [CreateAssetMenu(menuName = "Dialog/DialogDB")]
    public class DialogDB : ScriptableObject
    {
        public string stageName;   // 예: "Stage_1"
        public List<DialogLine> lines = new List<DialogLine>(); 
    }
}