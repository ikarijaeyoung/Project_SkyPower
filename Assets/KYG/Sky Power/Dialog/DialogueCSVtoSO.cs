#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace KYG.SkyPower
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

            for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
            {
                // 1. 빈줄 혹은 주석(앞이 # 등) 무시
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] t = lines[i].Split(',');

                // 2. 필드 개수 부족 방지 (예: 6 미만이면 건너뜀)
                if (t.Length < 4)
                {
                    Debug.LogWarning($"줄 {i + 1}: 필드 수 부족 (발견: {t.Length}). 내용: \"{lines[i]}\". 건너뜁니다.");
                    continue;
                }

                // 3. 숫자 파싱 방지코드
                int idVal;
                if (!int.TryParse(t[0].Trim(), out idVal))
                {
                    Debug.LogWarning($"줄 {i + 1}: id 파싱 실패. 내용: \"{lines[i]}\". 건너뜁니다.");
                    continue;
                }

                int nextVal = 0;
                string nextField = t[3].Trim();
                if (nextField != "END" && !int.TryParse(nextField, out nextVal))
                {
                    Debug.LogWarning($"줄 {i + 1}: nextID 파싱 실패. 내용: \"{lines[i]}\". 건너뜁니다.");
                    continue;
                }

                var line = new DialogLine()
                {
                    id = idVal,
                    speaker = t[1].Trim(),
                    portrait = Resources.Load<Sprite>($"Portraits/{t[2].Trim()}"),
                    text = t[3].Trim(),
                    //sound = Resources.Load<AudioClip>($"Sounds/{t[4].Trim()}"),
                    //nextID = nextField == "END" ? 0 : nextVal
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
