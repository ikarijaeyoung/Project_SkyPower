using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Set DB")]
        [SerializeField] private AudioDataBase audioDB;

        [Header("Set AudioSource")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource SFXSource;

        [Header("Set AudioClip")]
        [SerializeField] private AudioData defaultBGM;

        private Dictionary<string, AudioData> audioDict;

        protected override void OnAwake()
        {
            base.OnAwake();

            if (audioDB == null)
                Debug.LogWarning("[AudioManager] AudioDataBase 누락!");
            if (bgmSource == null)
                Debug.LogWarning("[AudioManager] BGM AudioSource 누락!");
            if (SFXSource == null)
                Debug.LogWarning("[AudioManager] SFX AudioSource 누락!");

            audioDict = new Dictionary<string, AudioData>();
            if (audioDB != null)
            {
                foreach (AudioData data in audioDB.audioList)
                {
                    if (data != null && !audioDict.ContainsKey(data.clipName))
                        audioDict.Add(data.clipName, data);
                }
            }
        }

        public override void Init()
        {
            // 필요시 오디오매니저 초기화 코드
        }

        // --- 이름(string)으로 BGM/SFX 재생 ---
        public void PlayBGM(string name)
        {
            if (!TryGetAudioData(name, out var data)) return;
            PlayBGM(data);
        }
        public void PlaySFX(string name, bool oneShot = false)
        {
            if (!TryGetAudioData(name, out var data)) return;
            PlaySFX(data, oneShot);
        }

        // --- AudioData 직접 재생 ---
        public void PlayBGM(AudioData bgm)
        {
            if (bgm == null || bgmSource == null) return;
            if (bgmSource.isPlaying && bgmSource.clip == bgm.clipSource) return;

            bgmSource.clip = bgm.clipSource;
            bgmSource.volume = bgm.volume;
            bgmSource.loop = bgm.loop;
            bgmSource.Play();
        }

        private void PlaySFX(AudioData sfx, bool oneShot = false)
        {
            if (sfx == null || SFXSource == null) return;
            if (oneShot)
                SFXSource.PlayOneShot(sfx.clipSource, sfx.volume);
            else
            {
                SFXSource.clip = sfx.clipSource;
                SFXSource.volume = sfx.volume;
                SFXSource.loop = sfx.loop;
                SFXSource.Play();
            }
        }

        // --- 3D 위치 사운드 재생 ---
        public void PlayClip(string name, Vector3 pos)
        {
            if (!TryGetAudioData(name, out var data)) return;
            PlayClip(data, pos);
        }
        public void PlayClip(AudioData data, Vector3 pos)
        {
            if (data == null || data.clipSource == null) return;
            GameObject go = new GameObject($"AudioClip_{data.clipName}");
            go.transform.position = pos;
            AudioSource audioClip = go.AddComponent<AudioSource>();
            audioClip.clip = data.clipSource;
            audioClip.volume = data.volume;
            audioClip.loop = data.loop;
            audioClip.spatialBlend = 1;
            audioClip.Play();
            if (!data.loop)
                Destroy(go, data.clipSource.length);
        }

        // --- 내부 함수 ---
        private bool TryGetAudioData(string name, out AudioData data)
        {
            if (string.IsNullOrEmpty(name))
            {
                // Debug.LogWarning($"[AudioManager] 사운드 이름 없음!{name}");
                data = null;
                return false;
            }
            if (audioDict != null && audioDict.TryGetValue(name, out data) && data != null) return true;
            // Debug.LogError($"[AudioManager] 사운드 데이터 없음: {name}");
            data = null;
            return false;
        }
    }
}