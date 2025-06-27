using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(menuName = "Game/DialogDataSO")] // DialogData 전체 관리하는 SO
    public class DialogDataSO : ScriptableObject
    {
        public List<DialogLine> lines = new List<DialogLine>();

        private Dictionary<int, DialogLine> dict; // ID로 조회
        public void Init() // lines -> dict 변환
        {
            dict = new Dictionary<int, DialogLine>();
            foreach (var l in lines) dict[l.id] = l;
        }
        public DialogLine GetLine(int id) // ID로 대사 한줄 조회
        {
            if (dict == null || dict.Count != lines.Count) Init();
            return dict.TryGetValue(id, out var line) ? line : null;
        }
    }
}