using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    public class AudioManagerRunner : MonoBehaviour // // AudioSource를 ScriptableObject에 연결하는 러너
    {
        public AudioManagerSO audioManager;
        public AudioSource bgmSource;
        public AudioSource sfxSource;

        void Awake()
        {
            audioManager.Init(bgmSource, sfxSource); // 런타임 AudioSource 주입
        }
    }
}