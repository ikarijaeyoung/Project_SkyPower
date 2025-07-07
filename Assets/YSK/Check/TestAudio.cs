using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestAudio : MonoBehaviour
{
    private bool check = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        KYG_skyPower.AudioManagerSO.Instance.PlayBGM("StartMenu_BGM");


        string currentSceneName = SceneManager.GetActiveScene().name;

    Debug.Log("Current Scene: " + currentSceneName);

        if (currentSceneName == "aTitleScene_JYL" && !check )
            {
            Debug.Log("Title Scene Detected, Playing BGM");
            check = true;
            KYG_skyPower.AudioManagerSO.Instance.PlayBGM("StartMenu_BGM");

        }
        
    }
}
