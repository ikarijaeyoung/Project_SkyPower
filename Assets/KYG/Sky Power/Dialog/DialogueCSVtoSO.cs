using KYG.SkyPower.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;


namespace KYG.SkyPower.Dialogue
{
    // CSV 파일을 읽어 ScriptableObject로 변환하는 에디터 스크립트
    // CSV 파일은 "speaker, content" 형식으로 되어 있어야 합니다.
    // 예시: "Player, Hello World!"

public class DialogueCSVtoSO
{
    [MenuItem("Tools/CSV/Convert Dialogue CSV to SO")]
    public static void Convert()
    {
        // 1. 경로 설정 (필요시 경로 수정)
        string csvPath = "Assets/Dialogues/Dialogue_Stage1.csv";
        string soPath = "Assets/Dialogues/Dialogue_Stage1_SO.asset";

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
            return;
        }

        // 2. CSV 읽기
        var lines = File.ReadAllLines(csvPath);

        // 3. DialogueDataSO 생성
        var dialogueSO = ScriptableObject.CreateInstance<DialogueDataSO>();
        dialogueSO.lines = new List<DialogueDataSO.Line>();

        // 4. CSV 파싱 (1줄씩)
        for (int i = 1; i < lines.Length; i++) // 0번째는 헤더
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var tokens = lines[i].Split(',');

            if (tokens.Length < 2) continue; // speaker, content
            DialogueDataSO.Line line = new DialogueDataSO.Line
            {
                speaker = tokens[0].Trim(),
                content = tokens[1].Trim().Replace("\\n", "\n")
            };
            dialogueSO.lines.Add(line);
        }

        // 5. SO 에셋 생성/저장
        AssetDatabase.CreateAsset(dialogueSO, soPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Dialogue SO 생성 완료: {soPath}");
    }
}
}