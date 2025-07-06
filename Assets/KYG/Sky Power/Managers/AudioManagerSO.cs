using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "AudioManagerSO", menuName = "Manager/AudioManager")]
    public class AudioManagerSO : SOSingleton<AudioManagerSO>
    {
        [Header("사운드 데이터베이스")]
        public AudioDataBase audioDB;

        [Header("디폴트 BGM / SFX")]
        public AudioData defaultBGM;

        [Header("풀 크기")]
        public int sfxPoolSize = 35;

        private Dictionary<string, AudioData> audioDict;

        [System.NonSerialized] private AudioSource bgmSource;
        [System.NonSerialized] private List<AudioSource> sfxPool;

        public static class Sound
        {
            public static void PlayBGM(string name) => Instance.PlayBGM(name);
            public static void PlaySFXOneShot(string name) => Instance.PlaySFX(name, true); // forceOneShot 기본값 true
            public static void FadeBGM(string name, float time = 1f) => Instance.FadeBGM(name, time);
            public static void StopBGM() => Instance.StopBGM();
        }

        public override void Init()
        {
            // 오디오 데이터 딕셔너리 초기화
            if (audioDict == null)
            {
                audioDict = new Dictionary<string, AudioData>();
                if (audioDB == null)
                {
                    Debug.LogError("[AudioManagerSO] AudioDB 연결 필요!");
                    return;
                }
                foreach (var data in audioDB.audioList)
                {
                    if (!audioDict.ContainsKey(data.clipName) && data.clipSource != null)
                        audioDict.Add(data.clipName, data);
                }
            }

            // BGMSource 오브젝트/컴포넌트 준비
            if (bgmSource == null || bgmSource.gameObject == null)
            {
                var go = GameObject.Find("BGMSource");
                if (go == null)
                {
                    go = new GameObject("BGMSource");
                    GameObject.DontDestroyOnLoad(go);
                }
                bgmSource = go.GetComponent<AudioSource>();
                if (bgmSource == null)
                    bgmSource = go.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }

            // SFX 풀 재초기화
            if (sfxPool == null)
                sfxPool = new List<AudioSource>();
            else
                sfxPool.Clear();

            // SFXPool 오브젝트 준비
            GameObject sfxGo = GameObject.Find("SFXPool");
            if (sfxGo == null)
            {
                sfxGo = new GameObject("SFXPool");
                GameObject.DontDestroyOnLoad(sfxGo);
            }

            var audioSources = sfxGo.GetComponents<AudioSource>();
            for (int i = 0; i < sfxPoolSize; i++)
            {
                AudioSource src = (i < audioSources.Length) ? audioSources[i] : sfxGo.AddComponent<AudioSource>();
                sfxPool.Add(src);
            }
        }

        public AudioData GetAudioData(string name)
        {
            if (audioDict == null) Init();
            if (!audioDict.TryGetValue(name, out var data))
            {
                Debug.LogWarning($"[AudioManagerSO] 오디오 데이터 미존재: {name}");
                return null;
            }
            return data;
        }

        /// <summary>
        /// 항상 PlayOneShot으로 효과음 재생 (중첩, 반복 재생 완벽 지원)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="forceOneShot"></param>
        public void PlaySFX(string name, bool forceOneShot = true)
        {
            var data = GetAudioData(name);
            if (data == null || data.clipSource == null) return;
            var src = GetAvailableSFXSource();
            if (forceOneShot)
            {
                src.PlayOneShot(data.clipSource, data.volume);
            }
            else
            {
                src.clip = data.clipSource;
                src.volume = data.volume;
                src.loop = data.loop;
                src.Play();
            }
        }

        /// <summary>
        /// PlayOneShot만 강제로 사용 (UI/클릭/일회성 효과음용)
        /// </summary>
        public void PlaySFXOneShot(string name)
        {
            Instance.PlaySFX(name, true);
        }

        private AudioSource GetAvailableSFXSource()
        {
            if (sfxPool == null || sfxPool.Count == 0) Init();
            foreach (var src in sfxPool)
                if (src != null && !src.isPlaying)
                    return src;

            // 모두 사용중이면 동적으로 추가 (풀 오버플로)
            var sfxGo = sfxPool[0].gameObject;
            var extra = sfxGo.AddComponent<AudioSource>();
            sfxPool.Add(extra);
            return extra;
        }

        public void FadeBGM(string name, float fadeTime = 1f)
        {
            if (bgmSource == null) Init();
            AudioManagerSO_CoroutineRunner.Instance.StartCoroutine(FadeBGM_Coroutine(name, fadeTime));
        }

        private IEnumerator FadeBGM_Coroutine(string name, float fadeTime)
        {
            float startVol = bgmSource.volume;
            for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVol, 0, t / fadeTime);
                yield return null;
            }
            bgmSource.Stop();

            var data = GetAudioData(name);
            if (data != null && data.clipSource != null)
            {
                bgmSource.clip = data.clipSource;
                bgmSource.volume = 0;
                bgmSource.loop = data.loop;
                bgmSource.Play();

                for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
                {
                    bgmSource.volume = Mathf.Lerp(0, data.volume, t / fadeTime);
                    yield return null;
                }
                bgmSource.volume = data.volume;
            }
        }

        public void PlayBGM(string name)
        {
            var data = GetAudioData(name);
            if (data == null || data.clipSource == null) return;
            bgmSource.clip = data.clipSource;
            bgmSource.volume = data.volume;
            bgmSource.loop = data.loop;
            bgmSource.Play();
        }

        public void StopBGM() => bgmSource?.Stop();
    }
}