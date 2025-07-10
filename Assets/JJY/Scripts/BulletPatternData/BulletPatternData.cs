using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;

public abstract class BulletPatternData : ScriptableObject
{
    public abstract IEnumerator Shoot(Transform[] firePoints, ObjectPool pool,int attackPower);
}
