using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "AudioDataBase", menuName = "Audio/DataBase")] // ������Ʈ ���� ����� ������ ����Ʈ�� ����
    public class AudioDataBase : ScriptableObject
    {
        public List<AudioData> audioList; // AudioData ����Ʈȭ
    }
}