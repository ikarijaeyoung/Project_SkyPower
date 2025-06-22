using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioManager", menuName = "Managers/AudioManager")]
public class AudioManagerSO : ScriptableObject
{
    // 런타임에서 주입받는 AudioSource들
    [HideInInspector] public AudioSource bgmSource;
    [HideInInspector] public AudioSource sfxSource;

    public void Init(AudioSource bgm, AudioSource sfx) // AudioSource를 외부에서 주입
    {
        bgmSource = bgm;
        sfxSource = sfx;
    }

    // 배경음악 재생
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // 효과음 재생
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // 배경음악 정지
    public void StopBGM()
    {
        bgmSource?.Stop();
    }
}
