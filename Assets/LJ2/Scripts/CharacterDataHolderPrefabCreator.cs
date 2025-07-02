using UnityEngine;
using UnityEditor;
using System.Linq;
using LJ2;

public class CharacterDataHolderPrefabCreator
{
    [MenuItem("Tools/CharacterDataHolder 프리팹 자동생성")]
    public static void CreatePrefabsForAllCharacterData()
    {
        // 모든 CharacterData ScriptableObject 에셋 경로 찾기  
        string[] guids = AssetDatabase.FindAssets("t:CharacterData", new[] { "Assets/LJ2/Scripts/Charictor" });
        var saveDir = "Assets/LJ2/Prefabs/CharacterDataHolders";
        if (!AssetDatabase.IsValidFolder(saveDir))
            AssetDatabase.CreateFolder("Assets/LJ2/Prefabs", "CharacterDataHolders");

        foreach (var guid in guids)
        {
            var data = AssetDatabase.LoadAssetAtPath<CharacterData>(AssetDatabase.GUIDToAssetPath(guid));
            if (data == null) continue;

            GameObject go = new GameObject($"{data.name}");
            var holder = go.AddComponent<LJ2.CharactorController>();  
            holder.characterData = data;
            // 필요한 경우 추가 컴포넌트 설정
            var parry = go.AddComponent<Parrying>();  // Parrying 컴포넌트 추가
            holder.parrying = parry;  // CharactorController에 Parrying 컴포넌트 연결



            string prefabPath = $"{saveDir}/{data.name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
        }

        Debug.Log("각 CharacterData별로 CharacterDataHolder 프리팹 자동생성 완료");
    }
}
