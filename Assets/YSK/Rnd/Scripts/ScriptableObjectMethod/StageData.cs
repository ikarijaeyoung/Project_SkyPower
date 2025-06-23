using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YSK;

namespace YSK
{
    [CreateAssetMenu(menuName = "Stage/StageData")]
    public class StageData : ScriptableObject
    {
        public int stageID;
        public string stageName;
        public string sceneName;
        public float duration;
        public List<GameObject> mapPrefabs;

    }
}
