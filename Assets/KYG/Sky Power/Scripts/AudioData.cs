using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{


    [System.Serializable]
    [CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Data")] // ���� ���� ScriptableObject��
    public class AudioData : ScriptableObject
    {
        public string clipName; // ���� �̸�
        public AudioClip clipSource; // ���� AudioClip ����
        [Range(0f, 1f)] // �⺻ ����
        public float volume = 1.0f;
        public bool loop = false; // �ݺ� ��� ����
    }
}
