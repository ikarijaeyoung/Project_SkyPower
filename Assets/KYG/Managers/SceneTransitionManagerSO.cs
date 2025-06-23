using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "SceneManager", menuName = "Managers/SceneManager")]
public class SceneTransitionManagerSO : ScriptableObject
{
    // 이름으로 씬 로딩
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    // 현재 씬 리로드
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
