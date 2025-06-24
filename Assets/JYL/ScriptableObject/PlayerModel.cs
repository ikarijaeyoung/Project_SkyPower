using UnityEngine;

namespace JYL
{
    [CreateAssetMenu(fileName = "NewPlayerModel", menuName = "ScriptableObject/Player")]
    [System.Serializable]
    public class PlayerModel : ScriptableObject
    {
        [Header("Set Value")]
        public int id = 0;
        public string playerName = "name";
        public FireType fireType = FireType.Normal;
        [Range(0.1f, 100)] public float playerPower = 1;
        [Range(0.1f, 50)]public float playerSpeed = 5;
        [Range(0.1f, 50)]public float fireSpeed = 15;
        [Range(0.1f, 50)]public float firePower = 1;
    }
    public enum FireType { Normal, Lazer, Missile }
}



