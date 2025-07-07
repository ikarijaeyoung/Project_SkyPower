using System.Collections.Generic;
using UnityEngine;

namespace KYG.SkyPower.Dialogue
{
    [System.Serializable]
    public class DialogLine
    {
        public int id;                      // 대사 고유번호
        public string speaker;              // 화자명
        public Sprite portrait;             // 초상화
        [TextArea] public string text;      // 대사 텍스트
        public AudioClip sound;             // 음성(선택)
        public int nextID;                  // 다음 대사 id (0 또는 -1은 종료)
    }

    [CreateAssetMenu(fileName = "DialogDB", menuName = "Dialog/DialogDB")]
    public class DialogDB : ScriptableObject
    {
        public List<DialogLine> lines;
    }
}
