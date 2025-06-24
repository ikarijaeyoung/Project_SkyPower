using UnityEngine;

namespace JYL
{
    [CreateAssetMenu(fileName = "NewPlayerModel", menuName = "ScriptableObject/Player")]
    public class PlayerModel : ScriptableObject
    {
        [Header("Set References")]
        [SerializeField] GameObject playerPrefab;

        [Header("Set Value")]
        [SerializeField] int id = 0;
        [SerializeField] string playerName = "name";
        [SerializeField] FireType fireType = FireType.Normal;
        [Range(0.1f, 100)][SerializeField] float playerPower = 1;
        [Range(0.1f, 50)][SerializeField] float playerSpeed = 5;
        [Range(0.1f, 50)][SerializeField] float fireSpeed = 15;
        [Range(0.1f, 50)][SerializeField] float firePower = 1;



    }
    public enum FireType { Normal, Lazer, Missile }
}



