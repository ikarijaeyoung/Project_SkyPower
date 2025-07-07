using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace KYG.SkyPower.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public DialogDB dialogDB;                         // SO로 등록
        public UnityEvent<DialogLine> OnDialogLine;       // UI/사운드/애니에서 구독

        private Dictionary<int, DialogLine> lineDict;     // ID로 빠르게 검색
        private int currentID;                            // 현재 대사 ID
        public bool IsActive { get; private set; }        // 대화 진행중?

        void Awake()
        {
            lineDict = new Dictionary<int, DialogLine>();
            foreach (var line in dialogDB.lines)
            {
                if (line != null)
                    lineDict[line.id] = line;
            }
        }

        public void StartDialog(int startID)
        {
            currentID = startID;
            IsActive = true;
            ShowLine();
        }

        public void Next()
        {
            if (!IsActive) return;
            if (!lineDict.ContainsKey(currentID)) { EndDialog(); return; }

            var line = lineDict[currentID];
            OnDialogLine?.Invoke(line);

            if (line.nextID == 0 || !lineDict.ContainsKey(line.nextID))
                EndDialog();
            else
                currentID = line.nextID;
        }

        void ShowLine()
        {
            if (!lineDict.ContainsKey(currentID)) { EndDialog(); return; }
            OnDialogLine?.Invoke(lineDict[currentID]);
        }

        void EndDialog()
        {
            IsActive = false;
            OnDialogLine?.Invoke(null);
        }
    }
}
