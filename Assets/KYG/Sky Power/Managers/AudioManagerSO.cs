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
                Debug.LogWarning("[AudioManager] AudioDataBase ����!");
            if (bgmSource == null)
                Debug.LogWarning("[AudioManager] BGM AudioSource ����!");
            if (SFXSource == null)
                Debug.LogWarning("[AudioManager] SFX AudioSource ����!");

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
            // �ʿ�� ������Ŵ��� �ʱ�ȭ �ڵ�
        }

        // --- �̸�(string)���� BGM/SFX ��� ---
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

        // --- AudioData ���� ��� ---
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

        // --- 3D ��ġ ���� ��� ---
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

        // --- ���� �Լ� ---
        private bool TryGetAudioData(string name, out AudioData data)
        {
            if (string.IsNullOrEmpty(name))
            {
                // Debug.LogWarning($"[AudioManager] ���� �̸� ����!{name}");
                data = null;
                return false;
            }
            if (audioDict != null && audioDict.TryGetValue(name, out data) && data != null) return true;
            // Debug.LogError($"[AudioManager] ���� ������ ����: {name}");
            data = null;
            return false;
        }
    }
}