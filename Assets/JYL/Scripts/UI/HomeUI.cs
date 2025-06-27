using System;
using UnityEngine;
using TMPro;

namespace JYL
{
    public class HomeUI : BaseUI
    {
        void Start()
        {
            GetUI<TMP_Text>("OpenMenuText").text = "Open Menu";
            GetEvent("MenuBtn").Click += data => UIManager.Instance.ShowPopUp<MenuPopUp>();
        }

        void Update()
        {

        }
    }
}


