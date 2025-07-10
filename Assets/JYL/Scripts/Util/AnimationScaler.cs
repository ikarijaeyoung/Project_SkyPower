using UnityEngine;
using UnityEditor;

public class AnimationScaler : MonoBehaviour
{
#if UNITY_EDITOR  

    [MenuItem("Tools/Scale Animation Clip")]
    static void ScaleAnimationClip()
    {
        AnimationClip clip = Selection.activeObject as AnimationClip;

        if (clip == null)
        {
            Debug.LogError("�ִϸ��̼� Ŭ���� �����ϼ���.");
            return;
        }

        float scaleFactor = 12f / 35f; // ī�޶� ���� ����

        // position ���� ��� ��� ����
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

        Debug.Log("�ִϸ��̼� ��ġ ũ�� ���� �Ϸ�!");
    }

#endif
}