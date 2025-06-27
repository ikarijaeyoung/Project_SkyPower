using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{


    [System.Serializable]
    [CreateAssetMenu(fileName = "NewAdioData", menuName = "Audio/Data")] // 사운드 정보 ScriptableObject로
    public class AudioData : ScriptableObject
    {
        public string clipName; // 사운드 이름
        public AudioClip clipSource; // 실제 AudioClip 에셋
        [Range(0f, 1f)] // 기본 볼륨
        public float volume = 1.0f;
        public bool loop = false; // 반복 재생 여부
    }
}
