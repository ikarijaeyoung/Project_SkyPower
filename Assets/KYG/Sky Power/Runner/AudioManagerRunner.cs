using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    public class AudioManagerRunner : MonoBehaviour
    {
        public AudioManagerSO audioManagerSO; // 에디터 연결 필수

        [Header("AudioSource")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;

        void Awake() // 초기화
        {
            if (audioManagerSO != null)
                audioManagerSO.Init();
        }

        void Start() // 기본 사운드 재생
        {
            PlaySFX(audioManagerSO.defaultSFX);
            PlayBGM(audioManagerSO.defaultBGM);
        }

        public void PlayClip(string name, Vector3 pos) // 3D용 위치기반 사운드
        {
            var data = audioManagerSO.GetAudioData(name);
            if (data == null || data.clipSource == null)
            {
                Debug.LogWarning($"오디오 데이터 못 찾음: {name}");
                return;
            }

            GameObject go = new GameObject($"AudioClip_{name}"); // 1회용
            go.transform.position = pos;
            var source = go.AddComponent<AudioSource>();
            source.clip = data.clipSource;
            source.volume = data.volume;
            source.loop = data.loop;
            source.spatialBlend = 1;
            source.Play();

            if (!data.loop) // 루프가 아닐 경우, 재생 종료 후 오브젝트 자동 삭제
                Destroy(go, data.clipSource.length);
        }

        private void PlayBGM(AudioData bgm) // BGM 재생
        {
            if (bgm == null || bgm.clipSource == null) return;
            if (bgmSource.isPlaying && bgmSource.clip == bgm.clipSource) return;

            bgmSource.clip = bgm.clipSource;
            bgmSource.volume = bgm.volume;
            bgmSource.loop = bgm.loop;
            bgmSource.Play();
        }

        private void PlaySFX(AudioData SFX) // SFX 재생
        {
            if (SFX == null || SFX.clipSource == null) return;
            if (sfxSource.isPlaying && sfxSource.clip == SFX.clipSource) return;

            sfxSource.clip = SFX.clipSource;
            sfxSource.volume = SFX.volume;
            sfxSource.loop = SFX.loop;
            sfxSource.Play();
        }
    }
}
