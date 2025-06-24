using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    [CreateAssetMenu(fileName = "InputManager", menuName = "Managers/InputManager")]
    public class InputManagerSO : ScriptableObject
    {
        public KeyCode pauseKey = KeyCode.Escape;

        // 일시 정지 키 입력 확인
        public bool IsPausePressed()
        {
            return Input.GetKeyDown(pauseKey);
        }
    }
}