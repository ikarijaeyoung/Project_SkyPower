using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "AudioDataBase", menuName = "Audio/DataBase")] // 프로젝트 전반 오디오 정보를 리스트로 저장
    public class AudioDataBase : ScriptableObject
    {
        public List<AudioData> audioList; // AudioData 리스트화
    }
}