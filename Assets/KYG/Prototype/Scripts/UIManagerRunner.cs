using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KYG
{
    public class UIManagerRunner : MonoBehaviour
    {
        public UIManagerSO uiManager;
        public GameObject pausePanel;

        void Awake()
        {
            uiManager.Init(pausePanel);
        }
    }
}

