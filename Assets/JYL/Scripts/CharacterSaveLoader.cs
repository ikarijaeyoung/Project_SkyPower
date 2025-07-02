using LJ2;
using UnityEngine;

namespace JYL
{
    public class CharacterSaveLoader : MonoBehaviour
    {
        public CharactorController[] charactorController;

        private string charPrefabPath = "CharacterPrefabs";

        void OnEnable()
        {
            GetCharPrefab();
        }

        void Update() { }
        public void GetCharPrefab()
        {
            //캐릭터 프리팹 전부 가져오기
            charactorController = Resources.LoadAll<CharactorController>(charPrefabPath);
            foreach (var cont in charactorController)
            {
                cont.SetParameter();
            }
            // 전부 셋파라매터 함.
        }
    }
}
