using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KYG.SkyPower.Dialogue
{
    // 대화 데이터 스크립터블 오브젝트
    // 대화 내용, 화자, 초상화 등 필요한 정보를 담는다.
    // 에디터에서 쉽게 수정 가능하도록 한다.

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueDataSO : ScriptableObject
{
    [System.Serializable]
    public struct Line
    {
        public string speaker;
        [TextArea] public string content;
        // 필요시 초상화, 이펙트 등 추가
    }
        public List<Line> lines = new List<Line>();
    }
}