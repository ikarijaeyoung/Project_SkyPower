using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{


    public class ScoreUI : MonoBehaviour
    {
        public GameManagerSO gameManager;
        public TMPro.TextMeshPro scoreText;

        // Update is called once per frame
        void Update()
        {
            scoreText.text = $"Score: {gameManager.score}";
        }
    }
}