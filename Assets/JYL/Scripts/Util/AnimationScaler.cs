using UnityEngine;
using UnityEditor;

public class AnimationScaler : MonoBehaviour
{
    [MenuItem("Tools/Scale Animation Clip")]
    static void ScaleAnimationClip()
    {
        AnimationClip clip = Selection.activeObject as AnimationClip;

        if (clip == null)
        {
            Debug.LogError("애니메이션 클립을 선택하세요.");
            return;
        }

        float scaleFactor = 12f / 35f; // 카메라 높이 비율

        // position 관련 모든 경로 추출
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in bindings)
        {
            if (binding.propertyName.Contains("m_LocalPosition"))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    var key = curve.keys[i];
                    key.value *= scaleFactor;
                    curve.MoveKey(i, key);
                }

                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
        }

        Debug.Log("애니메이션 위치 크기 조정 완료!");
    }
}