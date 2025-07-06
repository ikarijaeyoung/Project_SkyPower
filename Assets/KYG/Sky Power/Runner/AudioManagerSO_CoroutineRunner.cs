using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class AudioManagerSO_CoroutineRunner : MonoBehaviour
    {
        private static AudioManagerSO_CoroutineRunner _instance;
        public static AudioManagerSO_CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("AudioManagerSO_CoroutineRunner");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<AudioManagerSO_CoroutineRunner>();
                }
                return _instance;
            }
        }
    }
}
