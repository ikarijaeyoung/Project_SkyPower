using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltTest : MonoBehaviour
{
    public GameObject shield;

    public GameObject ultBullet;
    public GameObject ultLaser;
    public GameObject ultAll;

    public Coroutine ultRoutine;
    public YieldInstruction ultDelay;



    public void UseUlt()
    {
        if(ultRoutine != null)
        {
            
        }
        else
        {
        }

    }
    

    private IEnumerator ShieldCotoutine()
    { 
        shield.SetActive(true);
        
        yield return ultDelay;
        shield.SetActive(false);
        yield break;

    }




}
