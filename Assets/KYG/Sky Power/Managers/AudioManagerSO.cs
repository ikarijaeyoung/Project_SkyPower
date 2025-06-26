using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioManagerSO", menuName = "Manager/AudioManager")]
public class AudioManagerSO : ScriptableObject
{
    private static AudioManagerSO instance;

    public static AudioManagerSO Instance // 싱글톤 패턴
    {
        get
        {
            if (!instance)
                instance = Resources.Load<AudioManagerSO>("AudioManagerSO")
                    return instance;
        }
    }

    [Header("사운드 데이터베이스")]
    public AudioDataBase audioDB;

    [Header("디폴트 BGM / SFX")] // 게임 시작시 기본 사운드
    public AudioData defaultBGM;
    public AudioData defaultSFX;

    private Dictionary<string, AudioData> audioDict; // 이름으로 데이터 찾는 딕셔너리

    public void Init() // 오디오 DB 딕셔러리로 초기화
    {
        audioDict = new Dictionary<string, AudioData>();
        foreach (var data in audioDB.audioList)
        {
            if (!audioDict.ContainsKey(data.clipName))
                audioDict.Add(data.clipName, data);
        }
    }

    public AudioData GetAudioData(string name) // 이름으로 오디오DB 반환 
    {
        if (audioDict == null) Init();
        audioDict.TryGetValue(name, out AudioData data);
        return data;
    }
}
