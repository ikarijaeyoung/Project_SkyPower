using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG_skyPower
{
    [CreateAssetMenu(fileName = "AudioManagerSO", menuName = "Manager/AudioManager")] 
    public class AudioManagerSO : SOSingleton<AudioManagerSO> 
    {
        [Header("사운드 데이터베이스")] 
        public AudioDataBase audioDB;      // 오디오 데이터베이스

        [Header("디폴트 BGM / SFX")]
        public AudioData defaultBGM;     // 게임 시작시 기본 BGM

        [Header("풀 크기")]
        public int sfxPoolSize = 10; // SFX 풀 크기 (미사용, 추후 구현 예정)

        private Dictionary<string, AudioData> audioDict; // 이름으로 오디오 데이터 찾기 위한 딕셔너리
        private AudioSource bgmSource;
        private List<AudioSource> sfxPool = new List<AudioSource>(); // SFX 재생용 오디오 소스 풀 (미사용, 추후 구현 예정)

        public static class Sound
        {
            public static void PlayBGM(string name) => Instance.PlayBGM(name); 
            public static void PlaySFX(string name) => Instance.PlaySFX(name); 
            public static void FadeBGM(string name, float time = 1f) => Instance.FadeBGM(name, time); 
            public static void StopBGM() => Instance.StopBGM(); 
        }

        public override void Init()
        {
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
            if (bgmSource == null)
            {
                var go = new GameObject("BGMSource");
                GameObject.DontDestroyOnLoad(go);
                bgmSource = go.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }
            if (sfxPool.Count == 0)
            {
                var go = new GameObject("SFXPool");
                GameObject.DontDestroyOnLoad(go);
                for (int i = 0; i < sfxPoolSize; i++)
                    sfxPool.Add(go.AddComponent<AudioSource>());
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

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var src in sfxPool)
                if (!src.isPlaying) return src;

            // 모든 소스 사용중이면 동적으로 추가
            var sfxGo = sfxPool[0].gameObject;
            var extra = sfxGo.AddComponent<AudioSource>(); 
            sfxPool.Add(extra); // 새 오디오 소스 생성
            return extra;
        }

        public void PlaySFX(string name) 
        {
            var data = GetAudioData(name); 
            if (data == null || data.clipSource == null) return;
            var src = GetAvailableSFXSource();
            src.clip = data.clipSource;
            src.volume = data.volume;
            src.loop = data.loop;
            src.Play();
        }

        public void PlaySFX_OneShot(string name)
        {
            var data = GetAudioData(name); 
            if (data == null || data.clipSource == null) return;
            GetAvailableSFXSource().PlayOneShot(data.clipSource, data.volume);
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

        public void StopBGM() => bgmSource.Stop();
    }
}