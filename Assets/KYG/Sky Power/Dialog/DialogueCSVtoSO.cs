#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace KYG.SkyPower.Dialogue
{
    public class DialogCSVToSO
    {
        [MenuItem("Tools/Dialog/CSV to SO")]
        public static void Convert()
        {
            string path = "Assets/Dialog/dialog.csv";
            string[] lines = File.ReadAllLines(path);

            DialogDB db = ScriptableObject.CreateInstance<DialogDB>();
            db.lines = new List<DialogLine>();

            for (int i = 1; i < lines.Length; i++) // 첫 줄 헤더
            {
                string[] t = lines[i].Split(',');

                var line = new DialogLine()
                {
                    id = int.Parse(t[0]),
                    speaker = t[1],
                    portrait = Resources.Load<Sprite>($"Portraits/{t[2]}"),   // 폴더 및 네이밍 일치 확인
                    text = t[3],
                    sound = Resources.Load<AudioClip>($"Sounds/{t[4]}"),
                    nextID = t[5] == "END" ? 0 : int.Parse(t[5])
                };
                db.lines.Add(line);
            }
            AssetDatabase.CreateAsset(db, "Assets/Dialog/DialogDB.asset");
            AssetDatabase.SaveAssets();
            Debug.Log("DialogDB.asset 생성 완료");
        }
    }
}
#endif
